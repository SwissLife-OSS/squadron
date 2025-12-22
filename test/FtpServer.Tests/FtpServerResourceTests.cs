using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using FluentAssertions;
using FluentFTP;
using Xunit;

namespace Squadron;

public class FtpServerResourceTests(FtpServerResource ftpServerResource) : IClassFixture<FtpServerResource>
{
    private const string SampleFileName = "SampleFile.txt";

    [Fact]
    public async Task UploadFile_DownloadedFileMatchLocal()
    {
        // Arrange
        using Stream sampleFileStream = GetEmbeddedResource(SampleFileName);

        var localFileContent = ToByteArray(sampleFileStream);
        sampleFileStream.Position = 0;

        // Act
        await ftpServerResource.UploadAsync(sampleFileStream, SampleFileName, "/", default);

        //Assert
        byte[] downloadedFile =
            await ftpServerResource.DownloadAsync(SampleFileName, "/", default);

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