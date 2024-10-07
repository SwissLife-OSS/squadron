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
            var password = Guid.NewGuid().ToString("N");
            var localdata = PrepareLocalDirectory();

            builder
                .Name("typesense")
                .Image("typesense/typesense:27.1")
                .InternalPort(8108)
                .Password(password)
                .AddCmd("--api-key", password)
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
