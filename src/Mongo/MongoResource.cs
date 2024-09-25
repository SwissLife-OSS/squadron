using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Driver;
using Xunit;

namespace Squadron
{
    /// <inheritdoc/>
    public class MongoResource : MongoResource<MongoDefaultOptions> { }

    /// <summary>
    /// Represents a mongo database resource that can be used by unit tests.
    /// </summary>
    /// <seealso cref="IDisposable"/>
    public class MongoResource<TOptions> :
        ContainerResource<TOptions>,
        IAsyncLifetime,
        IComposableResource
        where TOptions : ContainerResourceOptions, new()
    {
        private MongoClient _client;

        /// <inheritdoc cref="IAsyncLifetime"/>
        public override async Task InitializeAsync()
        {
            await base.InitializeAsync();
            ConnectionString =
                $"mongodb://{Manager.Instance.Address}:{Manager.Instance.HostPort}";

            _client = GetClient();
            await Initializer.WaitAsync(new MongoStatus(_client));

            if (ResourceOptions is MongoInitOptions initOptions)
            {
                await CreateDatabase(initOptions.GetOptions());
            }
        }

        private MongoClient GetClient()
        {
            return new MongoClient(new MongoClientSettings
            {
                ConnectionMode = ConnectionMode.Direct,
                ReadConcern = ReadConcern.Majority,
                WriteConcern = WriteConcern.Acknowledged,
                Server = new MongoServerAddress(Manager.Instance.Address, Manager.Instance.HostPort),
                ConnectTimeout = TimeSpan.FromSeconds(5),
                ServerSelectionTimeout = TimeSpan.FromSeconds(5),
                SocketTimeout = TimeSpan.FromSeconds(5)
            });
        }

        public override Dictionary<string, string> GetComposeExports()
        {
            var internalConnectionString =
                $"mongodb://{Manager.Instance.Name}:{Settings.InternalPort}";

            Dictionary<string, string> exports = base.GetComposeExports();
            exports.Add("CONNECTIONSTRING", ConnectionString);
            exports.Add("CONNECTIONSTRING_INTERNAL", internalConnectionString);
            return exports;
        }

        /// <summary>
        /// Gets the mongo database client that is already
        /// initialized to use the mongo instance of the
        /// repository test environment.
        /// </summary>
        /// <value>The mongo database client.</value>
        public virtual IMongoClient Client => _client;

        /// <summary>
        /// Gets the external mongo database connection string that is exposed to the host.
        /// </summary>
        public string ConnectionString { get; private set; }

        /// <summary>
        /// Creates a new test databases with generated name.
        /// </summary>
        /// <returns>
        /// Returns the newly created test database.
        /// </returns>
        public IMongoDatabase CreateDatabase()
        {
            return CreateDatabase(new CreateDatabaseOptions());
        }

        /// <summary>
        /// Creates a new test databases with generated name by creating collections from files
        /// </summary>
        /// <param name="files">The import files.</param>
        /// <returns>
        /// Returns the newly created test database.
        /// </returns>
        public Task<IMongoDatabase> CreateDatabase(params FileInfo[] files)
        {
            return CreateDatabase(new CreateDatabaseFromFilesOptions
            {
                Files = files,
            });
        }

        /// <summary>
        /// Creates a new test databases with specified name
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>
        /// Returns the newly created test database.
        /// </returns>
        public IMongoDatabase CreateDatabase(string name)
        {
            return CreateDatabase(new CreateDatabaseOptions
            {
                DatabaseName = name
            });
        }

        /// <summary>
        /// Creates a new test databases with specified name by creating collections from files
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="files">The import files.</param>
        /// <returns>
        /// Returns the newly created test database.
        /// </returns>
        public Task<IMongoDatabase> CreateDatabase(string name, params FileInfo[] files)
        {
            return CreateDatabase(new CreateDatabaseFromFilesOptions
            {
                Files = files,
                DatabaseOptions = new CreateDatabaseOptions
                {
                    DatabaseName = name
                }
            });
        }

        /// <summary>
        /// Creates a new databases with specified <paramref name="options"/>.
        /// </summary>
        /// <param name="options">
        /// The database creation options. Default values will be used if not specified.
        /// </param>
        /// <returns>
        /// Returns the created database.
        /// </returns>
        public virtual IMongoDatabase  CreateDatabase(
            CreateDatabaseOptions options)
        {
            return Client.GetDatabase(options.DatabaseName);
        }

        /// <summary>
        /// Creates a new databases with specified <paramref name="options"/>.
        /// </summary>
        /// <param name="options">
        ///  The database creation options
        /// </param>
        /// <returns>
        /// Returns the created database.
        /// </returns>
        public async Task<IMongoDatabase> CreateDatabase(
            CreateDatabaseFromFilesOptions options)
        {
            IMongoDatabase database = CreateDatabase(
                options.DatabaseOptions);

            foreach (FileInfo fileInfo in options.Files)
            {
                var fileCreationOptions = new CreateCollectionFromFileOptions
                {
                    File = fileInfo,
                    Destination = options.Destination,
                    CustomImportArgs = options.CustomImportArgs,
                    CollectionOptions = new CreateCollectionOptions
                    {
                        CollectionName = Path.GetFileNameWithoutExtension(fileInfo.FullName),
                        DatabaseOptions = new CreateDatabaseOptions
                        {
                            DatabaseName = database.DatabaseNamespace.DatabaseName
                        }
                    }
                };

                await CreateCollectionFromFileInternalAsync(database, fileCreationOptions);
            }

            return database;
        }

        protected async Task<bool> DatabaseExists(string name)
        {
            return (await Client.ListDatabaseNamesAsync()).ToList()
                .Any(x => x == name);
        }

        /// <summary>
        /// Creates a new test collection from file with specified <paramref name="options"/>.
        /// inside of the generated database.
        /// </summary>
        /// <typeparam name="T">The document type.</typeparam>
        /// <param name="options">The collection creation options.</param>
        /// <returns>
        /// Returns the newly created collection.
        /// </returns>
        public async Task<IMongoCollection<T>> CreateCollectionFromFileAsync<T>(
            CreateCollectionFromFileOptions options)
        {
            IMongoDatabase database = CreateDatabase(
                options.CollectionOptions.DatabaseOptions);

            await CreateCollectionFromFileInternalAsync(database, options);

            return database
                .GetCollection<T>(options.CollectionOptions.CollectionName);
        }

        /// <summary>
        /// Creates a new test collection from file with specified <paramref name="options"/>.
        /// inside of the given <paramref name="database"/>
        /// </summary>
        /// <typeparam name="T">The document type.</typeparam>
        /// <param name="database">The target database</param>
        /// <param name="options">The collection creation options.</param>
        /// <returns>
        /// Returns the newly created collection.
        /// </returns>
        public async Task<IMongoCollection<T>> CreateCollectionFromFileAsync<T>(
            IMongoDatabase database,
            CreateCollectionFromFileOptions options)
        {
            options = options ?? new CreateCollectionFromFileOptions();
            database = database ?? CreateDatabase(options.CollectionOptions.DatabaseOptions);

            await CreateCollectionFromFileInternalAsync(database, options);

            return database
                .GetCollection<T>(options.CollectionOptions.CollectionName);
        }

        private async Task CreateCollectionFromFileInternalAsync(
            IMongoDatabase database,
            CreateCollectionFromFileOptions options)
        {
            options.CollectionOptions.DatabaseOptions = new CreateDatabaseOptions
            {
                DatabaseName = database.DatabaseNamespace.DatabaseName
            };
            await DeployAndImportAsync(options);
        }

        private async Task DeployAndImportAsync(
                CreateCollectionFromFileOptions options)
        {
            var copyContext = new CopyContext(
                options.File.FullName,
                Path.Combine(options.Destination, options.File.Name));

            await Manager.CopyToContainerAsync(copyContext);

            await Manager.InvokeCommandAsync(new MongoImportCommand(
                    copyContext.Destination,
                    options.CollectionOptions.DatabaseOptions.DatabaseName,
                    options.CollectionOptions.CollectionName,
                    options.CustomImportArgs)
                .ToContainerExecCreateParameters());
        }

        /// <summary>
        /// Creates a new test collection with dafault options
        /// inside of a generated database.
        /// </summary>
        /// <typeparam name="T">The document type.</typeparam>
        /// <returns>
        /// Returns the newly created collection.
        /// </returns>
        public IMongoCollection<T> CreateCollection<T>()
        {
            var options = new CreateCollectionOptions();
            IMongoDatabase database = CreateDatabase(
                options.DatabaseOptions);

            return CreateCollection<T>(database, options);
        }

        /// <summary>
        /// Creates a new test collection with default options
        /// inside of the given <paramref name="database"/>
        /// </summary>
        /// <typeparam name="T">The document type.</typeparam>
        /// <param name="database">The target database.</param>
        /// <returns>
        /// Returns the newly created collection.
        /// </returns>
        public IMongoCollection<T> CreateCollection<T>(
            IMongoDatabase database)
        {
            var options = new CreateCollectionOptions();
            return CreateCollection<T>(database, options);
        }

        /// <summary>
        /// Creates a new test collection with specified <paramref name="options"/>.
        /// inside of generated database.
        /// </summary>
        /// <typeparam name="T">The document type.</typeparam>
        /// <param name="options">The collection creation options.</param>
        /// <returns>
        /// Returns the newly created collection.
        /// </returns>
        public IMongoCollection<T> CreateCollection<T>(
            CreateCollectionOptions options)
        {
            IMongoDatabase database = CreateDatabase(
                options.DatabaseOptions);

            return CreateCollection<T>(database, options);
        }

        /// <summary>
        /// Creates a new test collection with specified <paramref name="options"/>.
        /// inside of the given <paramref name="database"/>
        /// </summary>
        /// <typeparam name="T">The document type.</typeparam>
        /// <param name="database">The target database.</param>
        /// <param name="options">The collection creation options.</param>
        /// <returns>
        /// Returns the newly created collection.
        /// </returns>
        public IMongoCollection<T> CreateCollection<T>(
            IMongoDatabase database,
            CreateCollectionOptions options)
        {
            options = options ?? new CreateCollectionOptions();
            database = database ?? CreateDatabase(options.DatabaseOptions);
            return database.GetCollection<T>(options.CollectionName);
        }

        /// <summary>
        /// Creates a collection with the give name and using a random database name
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public IMongoCollection<T> CreateCollection<T>(string name)
        {
            IMongoDatabase db =  CreateDatabase(new CreateDatabaseOptions());
            db.CreateCollection(name);
            return db.GetCollection<T>(name);
        }
    }
}

