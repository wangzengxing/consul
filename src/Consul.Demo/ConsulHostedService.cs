using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Consul.Demo
{
    public class ConsulHostedService : IHostedService
    {
        private ConsulOptions _consulOptions;
        private ConsulClient _consulClient;

        public ConsulHostedService(IOptionsMonitor<ConsulOptions> optionsMonitor)
        {
            _consulOptions = optionsMonitor.CurrentValue;
            _consulClient = new ConsulClient(x =>
            {
                x.Address = new Uri(_consulOptions.Address);
            });
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            var registration = new AgentServiceRegistration()
            {
                ID = _consulOptions.ServiceID,
                Name = _consulOptions.ServiceName,// 服务名
                Address = _consulOptions.ServiceAddress, // 服务绑定IP
                Port = _consulOptions.ServicePort, // 服务绑定端口
                Check = new AgentServiceCheck()
                {
                    DeregisterCriticalServiceAfter = TimeSpan.FromSeconds(5),//服务启动多久后注册
                    Interval = TimeSpan.FromSeconds(10),//健康检查时间间隔
                    HTTP = _consulOptions.ServiceHealthCheck,//健康检查地址
                    Timeout = TimeSpan.FromSeconds(5)
                }
            };
            // 服务注册
            return _consulClient.Agent.ServiceRegister(registration);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            // 应用程序终止时，服务取消注册
            return _consulClient.Agent.ServiceDeregister(_consulOptions.ServiceID);
        }
    }
}
