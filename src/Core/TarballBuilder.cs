using System.IO;
using ICSharpCode.SharpZipLib.Tar;

namespace Squadron
{
    public static class TarballBuilder
    {
        public static Stream CreateTarball(string directory)
        {
            var tarball = new MemoryStream();
            var files = Directory.GetFiles(directory, "*.*", SearchOption.AllDirectories);

            using var archive = new TarOutputStream(tarball)
            {
                //Prevent the TarOutputStream from closing the underlying memory stream when done
                IsStreamOwner = false
            };

            foreach (var file in files)
            {
                string tarName = file.Substring(directory.Length).Replace('\\', '/').TrimStart('/');

                var entry = TarEntry.CreateTarEntry(tarName);
                using FileStream fileStream = File.OpenRead(file);
                entry.Size = fileStream.Length;
                archive.PutNextEntry(entry);

                byte[] localBuffer = new byte[32 * 1024];
                while (true)
                {
                    int numRead = fileStream.Read(localBuffer, 0, localBuffer.Length);
                    if (numRead <= 0)
                        break;

                    archive.Write(localBuffer, 0, numRead);
                }

                archive.CloseEntry();
            }
            archive.Close();

            tarball.Position = 0;

            return tarball;
        }
    }
}
