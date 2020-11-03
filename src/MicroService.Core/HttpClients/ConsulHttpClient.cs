using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using MicroService.Consul.LoadBalances;
using MicroService.Consul.Discovery;

namespace MicroService.Consul.HttpClients
{
    /// <summary>
    /// consul httpclient扩展
    /// </summary>
    public class ConsulHttpClient
    {
        private readonly IServiceDiscovery serviceDiscovery;
        private readonly ILoadBalance loadBalance;
        private readonly HttpClient httpClient;

        public ConsulHttpClient(IServiceDiscovery serviceDiscovery,
                                    ILoadBalance loadBalance, HttpClient httpClient)
        {
            this.serviceDiscovery = serviceDiscovery;
            this.loadBalance = loadBalance;
            this.httpClient = httpClient;
        }

        /// <summary>
        /// Get方法
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// param name="ServiceSchme">服务名称:(http/https)</param>
        /// <param name="ServiceName">服务名称</param>
        /// <param name="serviceLink">服务路径</param>
        /// <returns></returns>
        public async Task<T> GetAsync<T>(string serviceshcme, string serviceName, string serviceLink)
        {
            if (string.IsNullOrEmpty(serviceshcme))
            {
                throw new ArgumentNullException(nameof(serviceshcme));
            }

            if (string.IsNullOrEmpty(serviceName))
            {
                throw new ArgumentNullException(nameof(serviceName));
            }

            if (string.IsNullOrEmpty(serviceLink))
            {
                throw new ArgumentNullException(nameof(serviceLink));
            }

            // 1、获取服务
            IList<ServiceUrl> serviceUrls = await serviceDiscovery.Discovery(serviceName);

            // 2、负载均衡服务
            ServiceUrl serviceUrl = loadBalance.Select(serviceUrls);

            // 3、建立请求
            Console.WriteLine($"请求路径：{serviceshcme} +'://'+{serviceUrl.Url} + {serviceLink}");
            // HttpResponseMessage response = await httpClient.GetAsync(serviceUrl.Url + serviceLink);
            HttpResponseMessage response = await httpClient.GetAsync(serviceshcme + "://" + serviceUrl.Url + serviceLink);

            // 3.1json转换成对象
            if (response.StatusCode == HttpStatusCode.OK)
            {
                string json = await response.Content.ReadAsStringAsync();

                return JsonConvert.DeserializeObject<T>(json);
            }
            else
            {
                // 3.2、进行自定义异常处理，这个地方进行了降级处理
                throw new Exception($"{serviceName}服务调用错误:{response.Content.ReadAsStringAsync()}");
            }
        }

        public Task<T> GetAsync<T>(string serviceName, string serviceLink)
        {
            return GetAsync<T>("http", serviceName, serviceLink);
        }

        /// <summary>
        /// Post方法
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// param name="ServiceSchme">服务名称:(http/https)</param>
        /// <param name="ServiceName">服务名称</param>
        /// <param name="serviceLink">服务路径</param>
        /// <param name="paramData">服务参数</param>
        /// <returns></returns>
        public T Post<T>(string serviceshcme, string serviceName, string serviceLink, object paramData = null)
        {
            // 1、获取服务
            IList<ServiceUrl> serviceUrls = serviceDiscovery.Discovery(serviceName).Result;

            // 2、负载均衡服务
            ServiceUrl serviceUrl = loadBalance.Select(serviceUrls);

            // 3、建立请求
            Console.WriteLine($"请求路径：{serviceshcme} +'://'+{serviceUrl.Url} + {serviceLink}");

            // 3.1 转换成json内容
            HttpContent hc = new StringContent(JsonConvert.SerializeObject(paramData), Encoding.UTF8, "application/json");

            // HttpResponseMessage response = await httpClient.GetAsync(serviceUrl.Url + serviceLink);
            HttpResponseMessage response = httpClient.PostAsync(serviceshcme + "://" + serviceUrl.Url + serviceLink, hc).Result;

            // 3.1json转换成对象
            if (response.StatusCode == HttpStatusCode.OK || response.StatusCode == HttpStatusCode.Created)
            {
                string json = response.Content.ReadAsStringAsync().Result;

                return JsonConvert.DeserializeObject<T>(json);
            }
            else
            {
                // 3.2、进行自定义异常处理，这个地方进行了降级处理
                throw new Exception($"{serviceName}服务调用错误:{response.Content.ReadAsStringAsync()}");
            }
        }
    }
}
