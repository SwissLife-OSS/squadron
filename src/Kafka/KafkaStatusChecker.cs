using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Squadron
{
    public class KafkaStatusChecker : IResourceStatusProvider
    {
        public Task<Status> IsReadyAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
