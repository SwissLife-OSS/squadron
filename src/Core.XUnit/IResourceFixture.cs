using Xunit;

namespace Squadron
{
    public interface IResourceFixture<T> : IClassFixture<XUnitResource<T>>
        where T : ISquadronAsyncLifetime, new()
    {
    }
}
