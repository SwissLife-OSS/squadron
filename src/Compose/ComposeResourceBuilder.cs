using System;
using System.Collections.Generic;

namespace Squadron
{
    public class ComposeResourceBuilder
    {
        public static ComposeResourceBuilder New() => new ComposeResourceBuilder();

        private List<ComposableResourceSettings> _settings =
            new List<ComposableResourceSettings>();

        public ComposeResourceSettings Build()
        {
            var settings = new ComposeResourceSettings();
            settings.Resources = new List<ComposableResourceSettings>(_settings);

            return settings;
        }

        public ComposeResourceBuilder AddContainer<TResourceOptions>(

            string name,
            TResourceOptions options,
            Action<IComposableResource> onStarted = null,
            params ComposeResourceLink[] links
            )
            where TResourceOptions : ContainerResourceOptions, IComposableResourceOption

        {
            _settings.Add(new ComposableResourceSettings
            {
                Type = ComposableResourceType.Container,
                ContainerOptions = options,
                Name = name,
                Links = new List<ComposeResourceLink>(links),
                OnStarted = onStarted
            });
            return this;
        }
    }

}
