using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Squadron
{
    public interface ISquadronAsyncLifetime
    {
        Task InitializeAsync();
        Task DisposeAsync();
    }

}
