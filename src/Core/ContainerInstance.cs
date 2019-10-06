using System.Collections.Generic;

namespace Squadron
{
    public class ContainerInstance
    {
        public string Id { get; set; }

        public string Address { get; set; }

        public string IpAddress { get; set; }

        public int HostPort { get; set; }
        public bool IsRunning { get; internal set; }

        public IList<string> Logs { get; set; } = new List<string>();
    }
}
