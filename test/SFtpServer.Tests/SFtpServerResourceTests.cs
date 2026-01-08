using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

namespace Squadron;

public class SFtpServerResourceTests(SFtpServerResource sftpServerResource) : IClassFixture<SFtpServerResource>
{
    private const string SampleFileName = "SampleFile.txt";

    [Fact]
    public async Task UploadFile_DownloadedFileMatchLocal()
    {
        // Arrange
        using Stream sampleFileStream = GetEmbeddedResource(SampleFileName);

        var localFileContent = ToByteArray(sampleFileStream);
        sampleFileStream.Position = 0;
        SFtpServerConfiguration configuration = sftpServerResource.FtpServerConfiguration;

        // Act
        sftpServerResource.Upload(sampleFileStream, SampleFileName, configuration.Directory);

        //Assert
        byte[] downloadedFile =
            sftpServerResource.Download(SampleFileName, configuration.Directory);

        downloadedFile.Should().BeEquivalentTo(localFileContent);
    }

    private Stream GetEmbeddedResource(string fileName)
    {
        var assembly = Assembly.GetExecutingAssembly();
        var resourceName = $"Squadron.SFtpServer.Tests.{fileName}";

        return assembly.GetManifestResourceStream(resourceName)
            ?? throw new InvalidOperationException(
                $"Embedded resource '{resourceName}' not found. Available resources: " +
                string.Join(", ", assembly.GetManifestResourceNames()));
    }

    private byte[] ToByteArray(Stream stream)
    {
        using (MemoryStream memoryStream = new MemoryStream())
        {
            stream.CopyTo(memoryStream);

            return memoryStream.ToArray();
        }
    }
}