using System;
using System.Threading.Tasks;
using MySqlConnector;
using Xunit;

namespace Squadron;

/// <summary>
/// Represents a MariaDB resource that can be used by unit tests.
/// </summary>
public class MariaDBResource: MySqlResource<MariaDBDefaultOptions>
{
}