using Interfaces;
using Interfaces.Model;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Streams.Core;
using System;
using System.Threading.Tasks;

namespace Grains
{
  [ImplicitStreamSubscription(InterfaceConst.PSHello)]
  public class HelloGrain : Grain, IHello
  {
    private readonly ILogger _logger;
    private readonly Observer<string> observer;
    private int _counter;

    public HelloGrain(ILogger<IHello> logger)
    {
      _logger = logger;
      observer = new Observer<string>(logger, (string greeting) => SayHello(greeting));

      _counter = 0;
    }

    public async Task OnSubscribed(IStreamSubscriptionHandleFactory handleFactory)
    {
      var handle = handleFactory.Create<string>();
      await handle.ResumeAsync(observer);
    }

    public Task<string> SayHello(string greeting)
    {
      var counter = $"{++_counter} times!";
      var time = $"Time: {DateTime.UtcNow.ToFileTimeUtc()}";

      _logger.LogInformation($"\n [SayHello] {counter} | greeting = '{greeting}' | {time}");
      return Task.FromResult($"\n Client: '{greeting}' | Grain: Hello {counter} | {time}");
    }
  }
}
