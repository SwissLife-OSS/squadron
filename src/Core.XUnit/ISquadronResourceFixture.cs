using Xunit;

namespace Squadron
{
    public interface ISquadronResourceFixture<T> : IClassFixture<SquadronResource<T>>
        where T : ISquadronAsyncLifetime, new()
    {
    }
}
