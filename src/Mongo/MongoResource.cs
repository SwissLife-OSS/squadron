using System;
using System.Threading.Tasks;
using MongoDB.Driver;
using Xunit;

namespace Squadron
{
    /// <summary>
    /// Represents a mongo database resource that can be used by unit tests.
    /// </summary>
    /// <seealso cref="IDisposable"/>
    public class MongoResource
        : ResourceBase<MongoImageSettings>, IAsyncLifetime
    {
        /// <inheritdoc cref="IAsyncLifetime"/>
        public async Task InitializeAsync()
        {
            await StartContainerAsync();

            ConnectionString = $"mongodb://{Settings.ContainerAddress}:{Settings.HostPort}";
            Client = new MongoClient(new MongoClientSettings
            {
                ConnectionMode = ConnectionMode.Direct,
                ReadConcern = ReadConcern.Majority,
                WriteConcern = WriteConcern.Acknowledged,
                Server = new MongoServerAddress(Settings.ContainerAddress, (int)Settings.HostPort),
                ConnectTimeout = TimeSpan.FromSeconds(5),
                ServerSelectionTimeout = TimeSpan.FromSeconds(5),
                SocketTimeout = TimeSpan.FromSeconds(5)
            });

            await Initializer.WaitAsync(
                new MongoStatus(Client));
        }

        /// <summary>
        /// Gets the mongo database client that is already 
        /// initialized to use the mongo instance of the 
        /// repository test environment.
        /// </summary>
        /// <value>The mongo database client.</value>
        public IMongoClient Client { get; private set; }

        /// <summary>
        /// Gets the mongo database connection string.
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
        /// Creates a new test databases with specified <paramref name="options"/>.
        /// </summary>
        /// <param name="options">The database creation options. Default values will be used if not specified.</param>
        /// <returns>
        /// Returns the newly created test database.
        /// </returns>
        public IMongoDatabase CreateDatabase(
            CreateDatabaseOptions options)
        {
            return Client.GetDatabase(options.DatabaseName);
        }

        /// <summary>
        /// Creates a new test collection from file.
        /// </summary>
        /// <typeparam name="T">The document type.</typeparam>
        /// <returns>
        /// Returns the newly created collection.
        /// </returns>
        public async Task<IMongoCollection<T>> CreateCollectionFromFileAsync<T>()
        {
            var options = new CreateCollectionFromFileOptions();
            IMongoDatabase database = CreateDatabase();

            return await CreateCollectionFromFileAsync<T>(
                database, options);
        }

        /// <summary>
        /// Creates a new test collection from file.
        /// inside of the given <paramref name="database"/>
        /// </summary>
        /// <typeparam name="T">The document type.</typeparam>
        /// <param name="database">The target database</param>
        /// <returns>
        /// Returns the newly created collection.
        /// </returns>
        public async Task<IMongoCollection<T>> CreateCollectionFromFileAsync<T>(
            IMongoDatabase database)
        {
            var options = new CreateCollectionFromFileOptions();

            return await CreateCollectionFromFileInternalAsync<T>(
                database, options);
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

            return await CreateCollectionFromFileInternalAsync<T>(
                database, options);
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

            return await CreateCollectionFromFileInternalAsync<T>(database, options);
        }

        private async Task<IMongoCollection<T>> CreateCollectionFromFileInternalAsync<T>(
            IMongoDatabase database,
            CreateCollectionFromFileOptions options)
        {
            await MongoUtils.DeployAndImport(
                options, Settings);

            return database
                .GetCollection<T>(options.CollectionOptions.CollectionName);
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

            return database
                .GetCollection<T>(options.CollectionName);
        }

        /// <inheritdoc cref="IAsyncLifetime"/>
        public async Task DisposeAsync()
        {
            await StopContainerAsync();
        }
    }
}
