using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using FluentAssertions;
using FluentFTP;
using Xunit;

namespace Squadron
{
    public class FtpServerResourceTests : IClassFixture<FtpServerResource>
    {
        private const string SampleFileName = "SampleFile.txt";
        private readonly FtpServerResource _ftpServerResource;

        public FtpServerResourceTests(FtpServerResource ftpServerResource)
        {
            _ftpServerResource = ftpServerResource;
        }

        [Fact]
        public async Task UploadFile_DownloadedFileMatchLocal()
        {
            // Arrange
            using Stream sampleFileStream = GetEmbeddedResource(SampleFileName);

            var localFileContent = ToByteArray(sampleFileStream);
            sampleFileStream.Position = 0;

            // Act
            await _ftpServerResource.UploadAsync(sampleFileStream, SampleFileName, "/", default);

            //Assert
            byte[] downloadedFile =
                await _ftpServerResource.DownloadAsync(SampleFileName, "/", default);

            downloadedFile.Should().BeEquivalentTo(localFileContent);
        }

        private Stream GetEmbeddedResource(string fileName)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = $"Squadron.{fileName}";

            return assembly.GetManifestResourceStream(resourceName);
        }

        private byte[] ToByteArray(Stream stream)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                stream.CopyToAsync(memoryStream);

                return memoryStream.ToArray();
            }
        }
    }
}
