using GrainInterfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Streams;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace API.Controllers
{
  [ApiController, Route("[controller]")]
  public class HelloWorldController : ControllerBase
  {
    private readonly ILogger _logger;
    private readonly IClusterClient _client;
    public HelloWorldController(ILogger<HelloWorldController> logger, IClusterClient client)
    {
      _logger = logger;

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

      var response = await grain.SayHello("Testing");
      Console.WriteLine($"{response}");

      var stream = _client.GetStreamProvider(AppConst.SMSProvider)
        .GetStream<string>(key, AppConst.PSHello);
      //await stream.SubscribeAsync(OnNextAsync);

      for (int i = 1; i < 10; i++)
        await stream.OnNextAsync($"Good morning, {i}!");

      return;
    }

    private Task OnNextAsync(string item, StreamSequenceToken token = null)
    {
      _logger.LogInformation($"OnNextAsync: item: {item}, token = {token}");
      return Task.CompletedTask;
    }
  }
}
