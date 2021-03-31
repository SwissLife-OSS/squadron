using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Amazon.S3;
using Amazon.S3.Model;

namespace Squadron
{
    public class S3Status : IResourceStatusProvider
    {
        private readonly string _host;
        private readonly string _accessKey;
        private readonly string _secretKey;

        /// <summary>
        /// Initializes a new instance of the <see cref="S3Status" /> class.
        /// </summary>
        /// <param name="host">Host</param>
        /// <param name="accessKey">S3 Access key</param>
        /// <param name="secretKey">S3 Secret key</param>
        public S3Status(string host, string accessKey, string secretKey)
        {
            _host = host;
            _accessKey = accessKey;
            _secretKey = secretKey;
        }


        public async Task<Status> IsReadyAsync(CancellationToken cancellationToken)
        {

            var config = new AmazonS3Config();
            config.ServiceURL = _host;
            config.ForcePathStyle = true;

            using (var s3Client = new AmazonS3Client(_accessKey, _secretKey, config))
            {
                try
                {
                    ListBucketsResponse result = await s3Client.ListBucketsAsync();

                    return new Status
                    {
                        IsReady = true,
                        Message =
                        $"Success. Owner: {result.Owner.DisplayName}, id: {result.Owner.Id}."
                    };
                }
                catch (Exception ex)
                {
                    return new Status
                    {
                        IsReady = false,
                        Message = ex.Message
                    };
                }
            }
        }
    }
}
