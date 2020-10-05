using System.Threading.Tasks;
using Xunit;

namespace Squadron
{

    public class XUnitResource<T> : IAsyncLifetime
        where T : ISquadronAsyncLifetime, new()
    {
        public XUnitResource()
        {
            Resource = new T();
        }

        public T Resource { get; }

        public async Task InitializeAsync()
        {
            await Resource.InitializeAsync();

        }

        public async Task DisposeAsync()
        {
            await Resource.DisposeAsync();
        }
    }
}
