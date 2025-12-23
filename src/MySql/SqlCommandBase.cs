using System.Collections.Generic;

namespace Squadron;

internal class SqlCommandBase
{
    protected string[] GetCommandArray(
        string query,
        ContainerResourceSettings settings)
    {
        return GetCommand(query, settings).ToArray();
    }

    private List<string> GetCommand(
        string query,
        ContainerResourceSettings settings)
    {
        return new List<string>
        {
            "mysql",
            "-u",
            "root",
            $"-p{settings.Password}",
            "-e",
            query
        };
    }
}