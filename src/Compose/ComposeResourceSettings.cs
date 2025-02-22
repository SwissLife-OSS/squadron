using System;
using System.Collections.Generic;

namespace Squadron;

public class ComposeResourceSettings
{
    public string Identifier { get; } = Guid.NewGuid().ToString("N");
    public IReadOnlyList<string> GlobalEnvionmentVariables { get; internal set; }

    public IReadOnlyList<ComposableResourceSettings> Resources { get; set; }

    internal ComposeResourceSettings(
        IReadOnlyList<string> globalEnvionmentVariables,
        IReadOnlyList<ComposableResourceSettings> resources)
    {
        GlobalEnvionmentVariables = globalEnvionmentVariables ??
                                    throw new ArgumentNullException(nameof(globalEnvionmentVariables));
        Resources = resources ?? throw new ArgumentNullException(nameof(resources));
    }
}