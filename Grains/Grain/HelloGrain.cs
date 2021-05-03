using EventStore.Client;
using Interfaces;
using Interfaces.Model;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Streams.Core;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Grains
{
  [ImplicitStreamSubscription(InterfaceConst.PSHello)]
  public class HelloGrain : Grain, IHello
  {
    private readonly ILogger _logger;
    private readonly EventStoreClient _client;

    public HelloGrain(ILogger<IHello> logger, EventStoreClient client)
    {
      _logger = logger;

      _client = client;
      observer = new Observer<string>(logger, (string greeting) => SayHello(greeting));

      _counter = 0;
    }

    private Orleans.Streams.IAsyncStream<string> Stream => GetStreamProvider(InterfaceConst.SMSProvider).GetStream<string>(this.GetPrimaryKey(), InterfaceConst.PSHello);

    public async override Task OnActivateAsync()
    {
      await _client.SubscribeToStreamAsync(InterfaceConst.PSHello, SubscribeReturn);

      await base.OnActivateAsync();
    }
    private async Task SubscribeReturn(EventStore.Client.StreamSubscription ss, ResolvedEvent vnt, CancellationToken ct)
    {
      var result = Encoding.UTF8.GetString(vnt.Event.Data.Span);
      await Stream.OnNextAsync(result);
    }

    public Task Consume()
    {
      _logger.LogInformation("Starting to consume...");
      return Task.CompletedTask;
    }

    private readonly Observer<string> observer;
    public async Task OnSubscribed(IStreamSubscriptionHandleFactory handleFactory)
    {
      var handle = handleFactory.Create<string>();
      await handle.ResumeAsync(observer);
    }

    private int _counter;
    public Task<string> SayHello(string greeting)
    {
      var counter = $"{++_counter} times!";
      var time = $"Time: {DateTime.UtcNow.ToFileTimeUtc()}";

      _logger.LogInformation($"\n [SayHello] {counter} | greeting = '{greeting}' | {time}");
      return Task.FromResult($"\n Client: '{greeting}' | Grain: Hello {counter} | {time}");
    }
  }
}
