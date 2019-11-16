using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Squadron.Samples.Generic
{
    public class XtraHeadersDelegateHandler : DelegatingHandler
    {
        public XtraHeadersDelegateHandler()
            : base( new HttpClientHandler() )
        {
        }

        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            request.Headers.Add("x-fancy", "foo");
            return base.SendAsync(request, cancellationToken);
        }
    }
}
