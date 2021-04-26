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
  public class PrimeController : ControllerBase
  {
    private readonly ILogger _logger;
    private readonly IClusterClient _client;
    public PrimeController(ILogger<HelloWorldController> logger, IClusterClient client)
    {
      _logger = logger;
      _client = client;
    }

    [HttpGet("{number}")]
    public async Task<bool> Get(int number = 101)
    {
      var grain = _client.GetGrain<IPrime>(0);
      return await grain.IsPrime(number);
    }

    [HttpGet]
    public async Task RunPrimes()
    {
      var grain = _client.GetGrain<IPrime>(0);
      var key = grain.GetGrainIdentity().PrimaryKey;

      var response = await grain.IsPrime(99);
      Console.WriteLine($"IsPrime: {response}");

      var stream = _client.GetStreamProvider(AppConst.SMSProvider)
        .GetStream<int>(key, AppConst.PSPrime);
      //await stream.SubscribeAsync(OnNextAsync);

      for (int mil = 0; mil < 1; mil++)
      {
        var tasks = new List<Task>();

        for (int dez = mil == 0 ? 100 : 0; dez < 1000; dez += 10)
        {
          var item = mil * 1000 + dez;

          tasks.Add(stream.OnNextAsync(item + 1));
          tasks.Add(stream.OnNextAsync(item + 3));
          tasks.Add(stream.OnNextAsync(item + 7));
          tasks.Add(stream.OnNextAsync(item + 9));
        }

        Task.WaitAll(tasks.ToArray());
      }
    }

    private Task OnNextAsync(int item, StreamSequenceToken token = null)
    {
      _logger.LogInformation($"OnNextAsync: item: {item}, token = {token}");
      return Task.CompletedTask;
    }
  }
}
