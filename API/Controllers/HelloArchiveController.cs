using GrainInterfaces;
using Microsoft.AspNetCore.Mvc;
using Orleans;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Controllers
{
  [ApiController, Route("[controller]")]
  public class HelloArchiveController : ControllerBase
  {
    private readonly IGrainFactory _client;
    public HelloArchiveController(IGrainFactory client)
    {
      _client = client;
    }

    [HttpGet("{greeting}")]
    public async Task<string> Get(string greeting = "HelloWorld!")
    {
      var grain = _client.GetGrain<IHelloArchive>(0);
      var result = await grain.SayHello(greeting);
      return string.IsNullOrWhiteSpace(result) ? null : result;
    }

    [HttpGet]
    [ProducesResponseType(200, Type = typeof(IEnumerable<string>))]
    [ProducesResponseType(204)]
    public async Task<IEnumerable<string>> Get()
    {
      var grain = _client.GetGrain<IHelloArchive>(0);
      var result = await grain.GetGreetings();
      return result?.Any() != true ? null : result;
    }
  }
}
