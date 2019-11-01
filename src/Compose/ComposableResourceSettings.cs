using System;
using System.Collections.Generic;
using System.Text;

namespace Squadron
{
    public class ComposableResourceSettings
    {
        public string Name { get; set; }

        public ComposableResourceType Type { get; set; }

        public IComposableResource Resource { get; set; }

        public IReadOnlyList<ComposeResourceLink> Links { get; set; }
            = new List<ComposeResourceLink>();

        public ContainerResourceOptions ContainerOptions { get; set; }

        public Action<IComposableResource> OnStarted { get; set; }
    }

}
