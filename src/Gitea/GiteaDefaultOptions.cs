using System;

namespace Squadron;

/// <summary>
/// Default Gitea resource options
/// </summary>
public class GiteaDefaultOptions : ContainerResourceOptions
{
    /// <summary>
    /// Configure resource options
    /// </summary>
    /// <param name="builder"></param>
    public override void Configure(ContainerResourceBuilder builder)
    {
        builder
            .Name("gitea")
            .Image("gitea/gitea:latest")
            .AddEnvironmentVariable("GITEA__security__INSTALL_LOCK=true")
            .Username("root")
            .Password("admin1234")
            .InternalPort(3000)
            .PreferLocalImage();
    }
}