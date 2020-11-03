using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MicroService.Consul.Registry
{
    /// <summary>
    /// 服务注册
    /// </summary>
   public interface IServiceRegistry
   {
        /// <summary>
        /// 注册服务
        /// </summary>
        void Register(ServiceRegistryOptions serviceNode);

        /// <summary>
        /// 撤销服务
        /// </summary>
        void Deregister(ServiceRegistryOptions serviceNode);
    }
}
