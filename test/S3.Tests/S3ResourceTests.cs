using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Amazon.S3.Model;
using FluentAssertions;
using Squadron;
using Xunit;

namespace S3.Tests;

public class S3ResourceTests(S3Resource s3Resource) : IClassFixture<S3Resource>
{
    [Fact]
    public async Task CreateBucket_Success()
    {
        // Arrange
        string bucketName = $"new-bucket-{Guid.NewGuid():N}";
        CancellationToken cancellationToken = TestContext.Current.CancellationToken;

        Amazon.S3.IAmazonS3 client = s3Resource.GetCLient();
        var putBucketRequest = new PutBucketRequest
        {
            BucketName = bucketName,
        };

        // Act
        await client.PutBucketAsync(putBucketRequest, cancellationToken);
        ListBucketsResponse result = await client.ListBucketsAsync(cancellationToken);

        // Assert
        result.Buckets.Should().ContainSingle(p => p.BucketName == bucketName);
    }

    [Fact]
    public async Task DownloadFile_ShouldMatchLocalFile()
    {
        // Arrange
        string bucketName = $"uploadtest-{Guid.NewGuid():N}";
        string objectKey = $"SampleFile-{Guid.NewGuid():N}.txt";
        const string fileName = "SampleFile.txt";
        CancellationToken cancellationToken = TestContext.Current.CancellationToken;

        Amazon.S3.IAmazonS3 client = s3Resource.GetCLient();

        await client.PutBucketAsync(
            new PutBucketRequest { BucketName = bucketName },
            cancellationToken);

        using Stream fileStream = OpenEmbeddedResourceStream(fileName);

        byte[] localFile = await StreamToByteArrayAsync(fileStream, cancellationToken);

        fileStream.Position = 0;

        await client.PutObjectAsync(
            new PutObjectRequest
            {
                InputStream = fileStream,
                BucketName = bucketName,
                Key = objectKey
            },
            cancellationToken);

        // Act
        GetObjectResponse fileResponse = await client.GetObjectAsync(
            new GetObjectRequest
            {
                Key = objectKey,
                BucketName = bucketName
            },
            cancellationToken);

        byte[] result = await StreamToByteArrayAsync(fileResponse.ResponseStream, cancellationToken);

        // Assert
        result.Should().BeEquivalentTo(localFile);
    }

    private async Task<byte[]> StreamToByteArrayAsync(
        Stream stream,
        CancellationToken cancellationToken)
    {
        using var memoryStream = new MemoryStream();
        await stream.CopyToAsync(memoryStream, cancellationToken);

        return memoryStream.ToArray();
    }

    private Stream OpenEmbeddedResourceStream(string fileName)
    {
        var assembly = Assembly.GetExecutingAssembly();
        var resourceName = $"Squadron.S3.Tests.{fileName}";

        return assembly.GetManifestResourceStream(resourceName)
            ?? throw new InvalidOperationException(
                $"Embedded resource '{resourceName}' not found. Available resources: " +
                string.Join(", ", assembly.GetManifestResourceNames()));
    }
}