using System;
using System.Collections.Generic;
using System.Text;

namespace MicroService.Consul.Discovery
{
    /// <summary>
    /// 服务发现配置
    /// </summary>
    public class ServiceDiscoveryOptions
    {
        /// <summary>
        /// 服务注册地址
        /// </summary>
        public string RegistryAddress { set; get; }
    }
}
