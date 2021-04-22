using GrainInterfaces;
using Microsoft.AspNetCore.Mvc;
using Orleans;
using System.Threading.Tasks;

namespace API.Controllers
{
  [ApiController, Route("[controller]")]
  public class HelloWorldController : ControllerBase
  {
    private readonly IGrainFactory _client;
    public HelloWorldController(IGrainFactory client)
    {
      _client = client;
    }

    [HttpGet("{greeting}")]
    public async Task<string> Get(string greeting = "HelloWorld!")
    {
      var grain = _client.GetGrain<IHello>(0);
      return await grain.SayHello(greeting);
    }
  }
}
