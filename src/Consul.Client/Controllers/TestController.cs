using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Consul.Client.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        private IHttpClientFactory _httpClientFactory;

        public TestController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var consulClient = new ConsulClient(c =>
            {
                //consul地址
                c.Address = new Uri("http://172.16.76.245:8500");
            });
            var services = await consulClient.Health.Service("Consul.Demo");

            var client = _httpClientFactory.CreateClient();
            var service = services.Response[0].Service;

            var url = $"http://{service.Address}:{service.Port}/api/values";
            var result = await client.GetAsync(url);
            var content = await result.Content.ReadAsStringAsync();

            return Ok(content);
        }
    }
}
