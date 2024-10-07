using System;
using System.Threading.Tasks;
using Xunit;

namespace Squadron
{
    /// <inheritdoc/>
    public class TypesenseResource : TypesenseResource<TypesenseDefaultOptions>
    {
    }
    
    /// <summary>
    /// Represents a Typesense resource that can be used by unit tests.
    /// </summary>
    /// <seealso cref="IDisposable"/>
    public class TypesenseResource<TOptions>
        : ContainerResource<TOptions>,
          IAsyncLifetime
        where TOptions : ContainerResourceOptions, new()
    { 
        public string BaseUrl { get; private set; }
        public int Port { get; private set; }
        public string ApiKey { get; set; }

        /// <inheritdoc cref="IAsyncLifetime"/>
        public override async Task InitializeAsync()
        {
            await base.InitializeAsync();
            BaseUrl = $"http://{Manager.Instance.Address}:{Manager.Instance.HostPort}";
            Port = Manager.Instance.HostPort;
            ApiKey = Settings.Password;

            await Initializer.WaitAsync(
                new TypesenseStatus(BaseUrl, ApiKey));
        }
    }
}
