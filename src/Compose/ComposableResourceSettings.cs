using System;
using System.Collections.Generic;
using System.Text;

namespace Squadron
{
    public class ComposableResourceSettings
    {
        public ComposableResourceSettings(
            string name,
            ComposableResourceType type,
            IReadOnlyList<ComposeResourceLink> links,
            ContainerResourceOptions containerOptions,
            Action<IComposableResource> onStarted)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Type = type;
            Links = links;
            ContainerOptions = containerOptions ?? throw new ArgumentNullException(nameof(containerOptions));
            OnStarted = onStarted;
        }

        public string Name { get ; private set; }

        public ComposableResourceType Type { get; private set; }

        public IComposableResource Resource { get; private set; }

        public IReadOnlyList<ComposeResourceLink> Links { get; private set; }
            = new List<ComposeResourceLink>();

        public ContainerResourceOptions ContainerOptions { get; private set; }

        public Action<IComposableResource> OnStarted { get; private set; }
    }

}
