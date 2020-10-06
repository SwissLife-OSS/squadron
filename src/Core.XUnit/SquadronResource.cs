using System.Threading.Tasks;
using Xunit;

namespace Squadron
{
    public class SquadronResource<T> : IAsyncLifetime
        where T : ISquadronAsyncLifetime, new()
    {
        public SquadronResource()
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
