using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Amazon.S3.Model;
using FluentAssertions;
using Squadron;
using Xunit;

namespace S3.Tests
{
    public class S3ResourceTests : IClassFixture<S3Resource>
    {
        private readonly S3Resource _s3Resource;

        public S3ResourceTests(S3Resource s3Resource)
        {
            _s3Resource = s3Resource;
        }

        [Fact]
        public async Task CreateBucket_Success()
        {
            // Arrange
            const string bucketName = "new-bucket";

            Amazon.S3.IAmazonS3 client = _s3Resource.GetCLient();
            var putBucketRequest = new PutBucketRequest
            {
                BucketName = bucketName,

            };

            // Act
            await client.PutBucketAsync(putBucketRequest, default);
            ListBucketsResponse result = await client.ListBucketsAsync(default);

            // Assert
            result.Buckets.Should().ContainSingle(p => p.BucketName == bucketName);
        }

        [Fact]
        public async Task DownloadFile_ShouldMatchLocalFile()
        {
            // Arrange
            const string bucketName = "uploadtest";
            const string fileName = "SampleFile.txt";

            Amazon.S3.IAmazonS3 client = _s3Resource.GetCLient();

            await client.PutBucketAsync(
                new PutBucketRequest { BucketName = bucketName },
                default);

            using Stream fileStream = OpenEmbeddedResourceStream(fileName);

            byte[] localFile = await StreamToByteArrayAsync(fileStream);

            fileStream.Position = 0;

            await client.PutObjectAsync(
                new PutObjectRequest
                {
                    InputStream = fileStream,
                    BucketName = bucketName,
                    Key = fileName
                },
                default);

            // Act
            GetObjectResponse fileResponse = await client.GetObjectAsync(
                new GetObjectRequest
                {
                    Key = fileName,
                    BucketName = bucketName
                },
                default);

            byte[] result = await StreamToByteArrayAsync(fileResponse.ResponseStream);

            // Assert
            result.Should().BeEquivalentTo(localFile);
        }

        private async Task<byte[]> StreamToByteArrayAsync(Stream stream)
        {
            using var memoryStream = new MemoryStream();
            await stream.CopyToAsync(memoryStream);
            return memoryStream.ToArray();
        }

        private Stream OpenEmbeddedResourceStream(string fileName)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = $"Squadron.{fileName}";

            return assembly.GetManifestResourceStream(resourceName);
        }
    }
}
