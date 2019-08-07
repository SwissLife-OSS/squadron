using System;
using System.IO;
using System.Threading.Tasks;

namespace Squadron
{
    internal static class MongoUtils
    {
        internal static async Task DeployAndImport(
            CreateCollectionFromFileOptions options,
            IImageSettings settings)
        {
            var copyContext = new CopyContext(
                options.File.FullName,
                Path.Combine(options.Destination, options.File.Name));

            await Container.CopyTo(copyContext, settings);
            await Container.InvokeCommand(
                new MongoImportCommand(
                    copyContext.Destination,
                    options.CollectionOptions.DatabaseOptions.DatabaseName,
                    options.CollectionOptions.CollectionName,
                    options.CustomImportArgs),
                settings);
        }

        internal static string CreateName(string prefix)
        {
            return $"{prefix}_{Guid.NewGuid():N}";
        }
    }
}
