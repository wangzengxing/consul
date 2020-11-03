using MicroService.Consul.Registry;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MicroService.Consul.Registry
{
    /// <summary>
    /// 微服务注册发现使用扩展
    /// </summary>
   public static class MicroServiceConsulApplicationBuilderExtensions
   {
        public static IApplicationBuilder UseConsulRegistry(this IApplicationBuilder app)
        {
            // 1、从IOC容器中获取Consul服务注册配置
            var serviceRegistryOptions = app.ApplicationServices.GetRequiredService<IOptions<ServiceRegistryOptions>>().Value;

            // 2、获取应用程序生命周期
            var lifetime = app.ApplicationServices.GetRequiredService<IHostApplicationLifetime>();

            // 2.1 获取服务注册实例
            var serviceRegistry = app.ApplicationServices.GetRequiredService<IServiceRegistry>();

            // 3、获取服务地址
            var features = app.Properties["server.Features"] as FeatureCollection;
            var address = features.Get<IServerAddressesFeature>().Addresses.First();
            var uri = new Uri(address);

            // 4、注册服务
            serviceRegistryOptions.Id = Guid.NewGuid().ToString();
            serviceRegistryOptions.Address = uri.Host;
            serviceRegistryOptions.Port = uri.Port;
            serviceRegistryOptions.HealthCheckAddress = $"{uri.Scheme}://{uri.Host}:{uri.Port}{serviceRegistryOptions.HealthCheckAddress}";
            serviceRegistry.Register(serviceRegistryOptions);

            // 5、服务器关闭时注销服务
            lifetime.ApplicationStopping.Register(() =>
            {
                serviceRegistry.Deregister(serviceRegistryOptions);
            });

            return app;
        }
   }
}
