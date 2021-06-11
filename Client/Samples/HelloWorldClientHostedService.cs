using EventStore;
using EventStore.Client;
using Interfaces;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Runtime;
using Orleans.Streams;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

namespace Client
{
  public class HelloWorldClientHostedService : IHostedService
  {
    private readonly ILogger _logger;
    private readonly IClusterClient _orleans;
    private readonly EventStoreClient _eventStore;

    public HelloWorldClientHostedService(ILogger<HelloWorldClientHostedService> logger, IClusterClient orleans, EventStoreClient eventStore)
    {
      _logger = logger;
      _orleans = orleans;
      _eventStore = eventStore;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
      //await Simple();
      //await Stream();
      await EventStore();
    }

    [SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "Old Version")]
    private async Task Simple()
    {
      // IHello
      var friend = _orleans.GetGrain<IHello>(0);
      var response = await friend.SayHello($"Good morning!");
      Console.WriteLine($"\n\n{response}\n\n");

      //IHelloArchive
      var g = _orleans.GetGrain<IHelloArchive>(0);
      response = await g.SayHello("Good day!");
      Console.WriteLine($"{response}");

      response = await g.SayHello("Good bye!");
      Console.WriteLine($"{response}");

      var greetings = await g.GetGreetings();
      Console.WriteLine($"\nArchived greetings: {Utils.EnumerableToString(greetings)}");
    }

    [SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "Old Version")]
    private async Task Stream()
    {
      var grain = _orleans.GetGrain<IHello>(0);
      var key = grain.GetGrainIdentity().PrimaryKey;

      var stream = _orleans.GetStreamProvider(InterfaceConst.SMSProvider)
        .GetStream<string>(key, InterfaceConst.PSHello);
      //await stream.SubscribeAsync(OnNextAsync);

      for (int i = 1; i < 10; i++)
        await stream.OnNextAsync($"Good morning, {i}!");
    }
    [SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "Testing")]
    private Task OnNextAsync(string item, StreamSequenceToken token = null)
    {
      _logger.LogInformation($"OnNextAsync: item: {item}, token = {token}");
      return Task.CompletedTask;
    }

    private async Task EventStore()
    {
      await _eventStore.SoftDeleteAsync(InterfaceConst.PSHello, StreamState.Any);

      var events = new List<EventData>();
      for (int i = 1; i < 10; i++)
      {
        var evt = $"Good morning, {i}!";

        events.Add(evt.GetEvent());
      }

      await _eventStore.AppendToStreamAsync(
        InterfaceConst.PSHello,
        StreamState.Any,
        events
      );
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
  }
}
