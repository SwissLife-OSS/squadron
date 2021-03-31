using System.Threading.Tasks;
using Amazon.S3;
using Xunit;

namespace Squadron
{
    /// <inheritdoc/>
    public class S3Resource : S3Resource<S3DefaultOptions> { }

    /// <summary>
    /// Represents a S3 resource that can be used by unit tests.
    /// </summary>
    public class S3Resource<TOptions>
        : ContainerResource<TOptions>,
          IAsyncLifetime
        where TOptions : S3DefaultOptions, new()
    {
        /// <summary>
        /// S3 host
        /// </summary>
        public string Host { get; private set; }
        /// <summary>
        /// S3 access key
        /// </summary>
        public string AccessKey { get; private set; }
        /// <summary>
        /// S3 Secret key
        /// </summary>
        public string SecretKey { get; private set; }

        public IAmazonS3 GetCLient()
        {
            var config = new AmazonS3Config();
            config.ServiceURL = Host;
            config.ForcePathStyle = true;

            return new AmazonS3Client(AccessKey, SecretKey, config);
        }

        /// <inheritdoc cref="IAsyncLifetime"/>
        public async override Task InitializeAsync()
        {
            await base.InitializeAsync();

            Host = $"http://{Manager.Instance.Address}:{Manager.Instance.HostPort}";
            AccessKey = Settings.Username;
            SecretKey = Settings.Password;

            await Initializer.WaitAsync(new S3Status(Host, AccessKey, SecretKey));
        }
    }
}
