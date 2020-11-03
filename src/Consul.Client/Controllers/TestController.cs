using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using MicroService.Consul.HttpClients;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace Consul.Client.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        private ConsulHttpClient _httpClient;

        public TestController(ConsulHttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var content = await _httpClient.GetAsync<Wrapper>("Consul.Service", "/api/values");
            return Ok(content.Data);
        }

        public class Wrapper
        {
            public string Data { get; set; }
        }
    }
}
