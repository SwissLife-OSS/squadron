using System.Threading.Tasks;
using Xunit;

namespace Squadron
{

    public class XUnitResource<T> : ISquadronAsyncLifetime
        where T : ISquadronAsyncLifetime, new()
    {
        public XUnitResource()
        {
            Resource = new T();
        }

        public T Resource { get; }

        public async Task InitializeAsync()
        {
            await Resource.InitializeAsync().ConfigureAwait(false);

        }

        public async Task DisposeAsync()
        {
            await Resource.DisposeAsync().ConfigureAwait(false);
        }
    }
}
