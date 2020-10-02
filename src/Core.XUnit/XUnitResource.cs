using System.Threading.Tasks;
using Xunit;

namespace Squadron
{

    public class XUnitResource<T> : IAsyncLifetime
        where T : ISquadronAsyncLifetime, new()
    {
        private readonly T _resource;

        public XUnitResource()
        {
            _resource = new T();
        }

        public T Resource => _resource;

        public async Task InitializeAsync()
        {
            await _resource.InitializeAsync();

        }

        public async Task DisposeAsync()
        {
            await _resource.DisposeAsync();
        }
    }
}
