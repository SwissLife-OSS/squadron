using System;
using System.IO;
using System.Linq;
using ICSharpCode.SharpZipLib.Tar;

namespace Squadron;

internal class TarArchiver : IDisposable
{
    private readonly string _archiveFileName;

    public TarArchiver(CopyContext CopyContext, bool overrideTargetName)
    {
        _archiveFileName = Path.GetTempFileName();

        using (Stream fileStream = File.Create(_archiveFileName))
        using (Stream tarStream = new TarOutputStream(fileStream))
        using (var tarArchive = TarArchive.CreateOutputTarArchive(tarStream))
        {
            var tarEntry = TarEntry.CreateEntryFromFile(CopyContext.Source);

            tarEntry.Name = overrideTargetName
                ? CopyContext.Destination.Split('\\', '/').Last()
                : CopyContext.Source.Split('\\', '/').Last();

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