using System;
using System.IO;
using System.Linq;
using ICSharpCode.SharpZipLib.Tar;

namespace Squadron
{
    internal class TarArchiver : IDisposable
    {
        private readonly string _archiveFileName;

        public TarArchiver(string sourceFile)
        {
            _archiveFileName = Path.GetTempFileName();

            using (Stream fileStream = File.Create(_archiveFileName))
            using (Stream tarStream = new TarOutputStream(fileStream))
            using (TarArchive tarArchive = TarArchive.CreateOutputTarArchive(tarStream))
            {
                TarEntry tarEntry = TarEntry.CreateEntryFromFile(sourceFile);
                tarEntry.Name = sourceFile.Split('\\', '/').Last();
                tarArchive.WriteEntry(tarEntry, true);
            }

            Stream = File.OpenRead(_archiveFileName);
        }

        public Stream Stream { get; }

        public void Dispose()
        {
            try
            {
                Stream?.Dispose();
                File.Delete(_archiveFileName);
            }
            catch
            {
                // ignored
            }
        }
    }
}
