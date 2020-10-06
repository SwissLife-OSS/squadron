using System.Threading.Tasks;

namespace Squadron
{
    public interface ISquadronAsyncLifetime
    {
        Task InitializeAsync();
        Task DisposeAsync();
    }
}
