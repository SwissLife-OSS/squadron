using System;
using System.Threading;
using System.Threading.Tasks;
using Neo4j.Driver;

namespace Squadron {
    /// <summary>
    /// Status checker for Neo4j
    /// </summary>
    /// <seealso cref="IResourceStatusProvider" />
    public class Neo4jStatus : IResourceStatusProvider {
        private readonly IDriver _driver;

        /// <summary>
        /// Initializes a new instance of the <see cref="Neo4jStatus"/> class.
        /// </summary>
        /// <param name="driver">Driver</param>
        public Neo4jStatus (IDriver driver) {
            _driver = driver;
        }

        public async Task<Status> IsReadyAsync (CancellationToken cancellationToken) {
            try {
                await _driver.VerifyConnectivityAsync ().ConfigureAwait (false);

                return new Status {
                    IsReady = true,
                        Message = "Ready"
                };
            } catch (Exception ex) {
                return new Status {
                    IsReady = false,
                        Message = ex.Message
                };
            }
        }
    }
}
