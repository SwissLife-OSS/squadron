using System;
using System.Threading.Tasks;
using MySqlConnector;
using Xunit;

namespace Squadron
{
    /// <inheritdoc/>
    public class MariaDBResource: MariaDBResource<MariaDBDefaultOptions>
    {

    }

    /// <summary>
    /// Represents a MySql resource that can be used by unit tests.
    /// </summary>
    /// <seealso cref="IDisposable"/>
    public class MariaDBResource<TOptions> : MySqlResource
    { 
    }
}
