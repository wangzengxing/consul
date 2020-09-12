using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace Consul.Demo
{
    public class ConsulOptions
    {
        public string Address { get; set; }
        public string ServiceName { get; set; }
        public string ServiceAddress { get; set; }
        public int ServicePort { get; set; }
        public string ServiceHealthCheck { get; set; }
        public string ServiceID { get; set; }
    }
}
