using GrainInterfaces;
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
    public async void RunHellos()
    {
      var grain = _client.GetGrain<IHello>(0);
      var key = grain.GetGrainIdentity().PrimaryKey;

      await grain.Consume();
      var stream = _client.GetStreamProvider(AppConst.SMSProvider)
        .GetStream<string>(key, AppConst.PSHello);

      for (int i = 1; i < 10; i++)
        await stream.OnNextAsync($"Good morning, {i}!");

      return;
    }
  }
}
