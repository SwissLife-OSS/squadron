using System;
using System.Collections.Generic;

namespace Squadron
{
    public class ComposableResourceBuilder<TResourceOptions>
        where TResourceOptions : ContainerResourceOptions, IComposableResourceOption, new()
    {
        private readonly string _name;
        private readonly ComposableResourceType _resourceType;
        private List<ComposeResourceLink> _links = new List<ComposeResourceLink>();
        private Action<IComposableResource> _onStarted = null;

        public static ComposableResourceBuilder<TResourceOptions> New(
            string name,
            ComposableResourceType resourceType)
                => new ComposableResourceBuilder<TResourceOptions>(name, resourceType);

        private ComposableResourceBuilder(string name, ComposableResourceType resourceType )
        {
            _name = name;
            _resourceType = resourceType;
        }

        public ComposableResourceBuilder<TResourceOptions> AddLink(
            string name,
            params EnvironmentVariableMapping[] mappings)
        {
            _links.Add(new ComposeResourceLink
            {
                Name = name,
                EnvironmentVariables = new List<EnvironmentVariableMapping>(mappings)
            });
            return this;
        }
        public ComposableResourceBuilder<TResourceOptions> WithOnStarted(
            Action<IComposableResource> onStarted)
        {
            _onStarted = onStarted;
            return this;
        }


        public ComposableResourceSettings Build()
        {
            var setting = new ComposableResourceSettings(
               _name,
               _resourceType,
               new List<ComposeResourceLink>(_links),
               new TResourceOptions(),
               _onStarted
               );

            return setting;
        }
    }


    public class ComposeResourceBuilder
    {
        public static ComposeResourceBuilder New() => new ComposeResourceBuilder();

        private List<ComposableResourceSettings> _settings =
            new List<ComposableResourceSettings>();

        private List<string> _globaEnvironmentVariables = new List<string>();

        public ComposeResourceSettings Build()
        {
            var settings = new ComposeResourceSettings();
            settings.Resources = new List<ComposableResourceSettings>(_settings);

            return settings;
        }

        public ComposableResourceBuilder<TResourceOptions> AddContainer<TResourceOptions>(
            string name
            )
            where TResourceOptions : ContainerResourceOptions, IComposableResourceOption, new()
        {

            return ComposableResourceBuilder<TResourceOptions>.New(
                name,
                ComposableResourceType.Container);

        }

        public ComposeResourceBuilder AddGlobalEnvironmentVariable(string name, string value)
        {
            _globaEnvironmentVariables.Add($"{name}={value}");
            return this;
        }
    }
}
