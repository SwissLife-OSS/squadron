using System.Collections.Generic;

namespace Squadron;

public class ComposeResourceLink
{
    public string Name { get; set; }

    public List<EnvironmentVariableMapping> EnvironmentVariables { get; set; }
}