using System;
using System.IO;

namespace Squadron
{
    /// <summary>
    /// Default RavenDB resource options
    /// </summary>
    public class TypesenseDefaultOptions : ContainerResourceOptions
    {
        /// <summary>
        /// Configure resource options
        /// </summary>
        /// <param name="builder"></param>
        public override void Configure(ContainerResourceBuilder builder)
        {
            var localdata = PrepareLocalDirectory();
            var apiKey = "secretKey";

            builder
                .Name("typesense")
                .Image("typesense/typesense:27.1")
                .InternalPort(8108)
                .Password(apiKey)
                .AddCmd("--api-key", apiKey)
                .AddCmd("--data-dir", "/data")
                .AddVolume($"{localdata}:/data")
                .PreferLocalImage();
        }

        private static string PrepareLocalDirectory()
        {
            var localdata = Path.GetTempPath();
            if (!Directory.Exists(localdata))
            {
                Directory.CreateDirectory(localdata);
            }

            return localdata;
        }
    }
}
