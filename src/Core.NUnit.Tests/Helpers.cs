using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using Docker.DotNet;

namespace Squadron
{
    internal class Helpers
    {
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

        public static DockerClient GetDockerClient =
            new DockerClientConfiguration(
                    Helpers.LocalDockerUri(),
                    null,
                    TimeSpan.FromMinutes(5))
                .CreateClient();
    }
}
