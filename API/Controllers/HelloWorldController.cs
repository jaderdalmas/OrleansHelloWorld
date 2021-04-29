using Interfaces;
using Microsoft.AspNetCore.Mvc;
using Orleans;
using System.Threading.Tasks;

namespace API.Controllers
{
  [ApiController, Route("[controller]")]
  public class HelloWorldController : ControllerBase
  {
    private readonly IClusterClient _client;
    public HelloWorldController(IClusterClient client)
    {
      _client = client;
    }

    [HttpGet("{greeting}")]
    public async Task<string> Get(string greeting = "HelloWorld!")
    {
      var grain = _client.GetGrain<IHello>(0);
      var result = await grain.SayHello(greeting);
      return string.IsNullOrWhiteSpace(result) ? null : result;
    }

    [HttpGet]
    public async Task RunHellos()
    {
      var grain = _client.GetGrain<IHello>(0);
      var key = grain.GetGrainIdentity().PrimaryKey;

      var stream = _client.GetStreamProvider(InterfaceConst.SMSProvider)
        .GetStream<string>(key, InterfaceConst.PSHello);

      for (int i = 1; i < 10; i++)
        await stream.OnNextAsync($"Good morning, {i}!");
    }
  }
}
