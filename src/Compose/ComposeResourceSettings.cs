using System.Collections.Generic;

namespace Squadron
{
    public class ComposeResourceSettings
    {
        public IReadOnlyList<string> GlobalEnvionmentVariables { get; private set; }

        public IReadOnlyList<ComposableResourceSettings> Resources { get; set; }
    }
}
