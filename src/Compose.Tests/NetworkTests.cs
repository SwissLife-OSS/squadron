using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Docker.DotNet;
using Docker.DotNet.Models;
using FluentAssertions;
using Xunit;

namespace Squadron
{
    public class NetworkTest : IClassFixture<NetworkCompositionResource>
    {
        private readonly NetworkCompositionResource _resource;
        private readonly IDockerClient _dockerClient;

        public NetworkTest(NetworkCompositionResource resource)
        {
            _resource = resource;
            _dockerClient = new DockerClientConfiguration(
                 LocalDockerUri(),
                 null,
                 TimeSpan.FromMinutes(5))
                .CreateClient();
        }

        [Fact]
        public async Task TwoContainer_Network_BothInSameNetwork()
        {
            MongoResource mongoResource = _resource.GetResource<MongoResource>("mongo");
            string connectionString = mongoResource.NetworkConnectionString;

            string containerName = GetNameFromConnectionString(connectionString);
            IList<ContainerListResponse> response = (await _dockerClient.Containers.ListContainersAsync(
                new ContainersListParameters()));

            ContainerListResponse container = response.Where(c => c.Names.Contains($"/{containerName}")).Single();

            string networkName = container.NetworkSettings.Networks.Keys.Where(n => n.Contains("squa_network")).Single();

            NetworkResponse network = (await _dockerClient.Networks.ListNetworksAsync()).Where(n => n.Name == networkName).SingleOrDefault();
            network.Should().NotBeNull();
        }

        private static Uri LocalDockerUri()
        {
            #if NET461
            return new Uri("npipe://./pipe/docker_engine");
            #else
            var isWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
            return isWindows ?
                new Uri("npipe://./pipe/docker_engine") :
                new Uri("unix:/var/run/docker.sock");
            #endif
        }

        private string GetNameFromConnectionString(string connectionString)
        {
            connectionString = connectionString.Replace("mongodb://", "");
            return connectionString.Split(':')[0];
        }
    }

    public class WebApp : GenericContainerOptions
    {
        public override void Configure(ContainerResourceBuilder builder)
        {
            base.Configure(builder);
            builder
                .Name("webapp")
                .InternalPort(80)
                .Image("nginx")
                .AddNetwork("demo-network");
        }
    }

    public class MongoDbContainer : MongoDefaultOptions
    {
        public override void Configure(ContainerResourceBuilder builder)
        {
            base.Configure(builder);
            builder.AddNetwork("demo-network");
        }
    }

    public class NetworkComposition : ComposeResourceOptions
    {
        public override void Configure(ComposeResourceBuilder builder)
        {
            builder.AddContainer<MongoDbContainer>("mongo");
            builder.AddContainer<WebApp>("webapp")
                .AddLink("mongo", new EnvironmentVariableMapping(
                    "Sample:Database:ConnectionString", "#CONNECTIONSTRING_INTERNAL#"
                ));
        }
    }

    public class NetworkCompositionResource : ComposeResource<NetworkComposition>
    {
    }
}
