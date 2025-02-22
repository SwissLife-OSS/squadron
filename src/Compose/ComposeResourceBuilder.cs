using System;
using System.Collections.Generic;
using System.Linq;

namespace Squadron;

public interface IComposableResourceBuilder
{
    ComposableResourceSettings Build();
}

public class ComposableResourceBuilder<TResourceOptions> : IComposableResourceBuilder
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

    private ComposableResourceBuilder(string name, ComposableResourceType resourceType)
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
        return new ComposableResourceSettings(
            _name,
            _resourceType,
            new List<ComposeResourceLink>(_links),
            new TResourceOptions(),
            _onStarted);
    }
}


public class ComposeResourceBuilder
{
    public static ComposeResourceBuilder New() => new ComposeResourceBuilder();

    private List<IComposableResourceBuilder> _settingsBuilder =
        new List<IComposableResourceBuilder>();

    private List<string> _globaEnvironmentVariables = new List<string>();

    public ComposeResourceSettings Build()
    {
        return new ComposeResourceSettings(
            new List<string>(_globaEnvironmentVariables),
            _settingsBuilder.Select(x => x.Build()).ToList());
    }

    public ComposableResourceBuilder<TResourceOptions> AddContainer<TResourceOptions>(
        string name)
        where TResourceOptions : ContainerResourceOptions, IComposableResourceOption, new()
    {

        var crb = ComposableResourceBuilder<TResourceOptions>.New(
            name,
            ComposableResourceType.Container);
        _settingsBuilder.Add(crb);
        return crb;
    }

    public ComposeResourceBuilder AddGlobalEnvironmentVariable(string name, string value)
    {
        _globaEnvironmentVariables.Add($"{name}={value}");
        return this;
    }
}