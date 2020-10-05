using System;
using System.Collections.Generic;
using System.Text;

namespace Squadron
{
    public interface ISquadronResourceFixture<T> where T : ISquadronAsyncLifetime
    {
    }
}
