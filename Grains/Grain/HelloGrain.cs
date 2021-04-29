using Interfaces;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Streams;
using Orleans.Streams.Core;
using System;
using System.Threading.Tasks;

namespace Grains
{
  [ImplicitStreamSubscription(Interfaces.InterfaceConst.PSHello)]
  public class HelloGrain : Grain, IHello
  {
    private readonly ILogger _logger;
    private readonly HelloObserver observer;
    private int _counter;

    public HelloGrain(ILogger<IHello> logger)
    {
      _logger = logger;
      observer = new HelloObserver(logger, (string greeting) => SayHello(greeting));

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

  public class HelloObserver : IAsyncObserver<string>
  {
    private readonly ILogger logger;
    private readonly Func<string, Task> action;

    public HelloObserver(ILogger<IHello> logger, Func<string, Task> action)
    {
      this.logger = logger;
      this.action = action;
    }

    public Task OnCompletedAsync() => Task.CompletedTask;

    public Task OnErrorAsync(Exception ex)
    {
      logger.LogError(ex, ex.Message);
      return Task.CompletedTask;
    }

    public Task OnNextAsync(string item, StreamSequenceToken token = null) => action(item);
  }
}
