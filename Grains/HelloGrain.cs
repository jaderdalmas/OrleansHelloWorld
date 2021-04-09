using GrainInterfaces;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Providers;
using System;
using System.Threading.Tasks;

namespace Grains
{
  [StorageProvider(ProviderName = "File")]
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
      logger.LogInformation($"\n [SayHello] counter = {++_counter} | Time: {DateTime.UtcNow.ToFileTimeUtc()} | greeting = '{greeting}'");
      return Task.FromResult($"\n Client: '{greeting}' | Grain: Hello {_counter} times! | Time: {DateTime.UtcNow.ToFileTimeUtc()}");
    }
  }
}
