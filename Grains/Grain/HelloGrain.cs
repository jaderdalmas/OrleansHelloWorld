using GrainInterfaces;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Providers;
using System;
using System.Threading.Tasks;

namespace Grains
{
  [StorageProvider(ProviderName = AppConst.Storage)]
  public class HelloGrain : Grain, IHello
  {
    private readonly ILogger logger;
    private int _counter;

    public HelloGrain(ILogger<HelloGrain> logger)
    {
      this.logger = logger;

      _counter = 0;
    }

    Task<string> IHello.SayHello(string greeting)
    {
      var counter = $"{++_counter} times!";
      var time = $"Time: {DateTime.UtcNow.ToFileTimeUtc()}";

      logger.LogInformation($"\n [SayHello] {counter} | greeting = '{greeting}' | {time}");
      return Task.FromResult($"\n Client: '{greeting}' | Grain: Hello {counter} | {time}");
    }
  }
}
