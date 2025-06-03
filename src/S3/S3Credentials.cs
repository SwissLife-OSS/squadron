using System;
using System.Collections.Generic;
using System.Text;
using Amazon.Runtime;

namespace Squadron;

internal class S3Credentials(string accessKey, string secretKey) : AWSCredentials
{
    public override ImmutableCredentials GetCredentials()
    {
        return new ImmutableCredentials(
            accessKey,
            secretKey,
            string.Empty);
    }
}