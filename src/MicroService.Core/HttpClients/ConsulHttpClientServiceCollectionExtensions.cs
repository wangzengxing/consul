using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using MicroService.Consul.LoadBalances;
using Microsoft.Extensions.Configuration;

namespace MicroService.Consul.HttpClients
{
    /// <summary>
    /// HttpClientFactory conusl下的扩展
    /// </summary>
    public static class ConsulHttpClientServiceCollectionExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="ConsulHttpClient"></typeparam>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static IServiceCollection AddConsulHttpClient(this IServiceCollection services, IConfiguration configuration)
        {
            // 1、注册consul
            services.AddConsulDiscovery(configuration);

            // 2、注册服务负载均衡
            services.AddSingleton<ILoadBalance, RandomLoadBalance>();

            // 3、注册httpclient
            services.AddHttpClient<ConsulHttpClient>();

            return services;
        }
    }
}
