using System;
using System.Collections.Generic;
using System.Text;
using Amazon.Runtime;

namespace Squadron
{
    internal class S3Credentials : AWSCredentials
    {
        private readonly string _accessKey;
        private readonly string _secretKey;

        public S3Credentials(string accessKey, string secretKey)
        {
            _accessKey = accessKey;
            _secretKey = secretKey;
        }
        public override ImmutableCredentials GetCredentials()
        {
            return new ImmutableCredentials(
                _accessKey,
                _secretKey,
                string.Empty);
        }
    }
}
