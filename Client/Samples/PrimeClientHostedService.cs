using EventStore;
using EventStore.Client;
using Interfaces;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Streams;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Client
{
  public class PrimeClientHostedService : IHostedService
  {
    private readonly ILogger _logger;
    private readonly IClusterClient _orleans;
    private readonly EventStoreClient _eventStore;

    public PrimeClientHostedService(ILogger<HelloWorldClientHostedService> logger, IClusterClient orleans, EventStoreClient eventStore)
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
    private Task Simple()
    {
      Task.Delay(10000);

      Prime.ManyPrime(_orleans, 1);

      return Task.CompletedTask;
    }

    [SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "Old Version")]
    private Task Stream()
    {
      var grain = _orleans.GetGrain<IPrime>(0);
      var key = grain.GetGrainIdentity().PrimaryKey;

      var stream = _orleans.GetStreamProvider(InterfaceConst.SMSProvider)
        .GetStream<int>(key, InterfaceConst.PSPrime);
      //await stream.SubscribeAsync(OnNextAsync);

      var tasks = new List<Task>();
      for (int i = 101; i < 110; i++)
      {
        tasks.Add(stream.OnNextAsync(i));

        if (i % 100 == 0)
          Task.WaitAll(tasks.ToArray());
      }
      Task.WaitAll(tasks.ToArray());

      return Task.CompletedTask;
    }
    [SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "Testing")]
    private Task OnNextAsync(int item, StreamSequenceToken token = null)
    {
      _logger.LogInformation($"OnNextAsync: item: {item}, token = {token}");
      return Task.CompletedTask;
    }

    private async Task EventStore()
    {
      for (uint mil = 0; mil < 1; mil++)
      {
        for (uint dez = (uint)(mil == 0 ? 100 : 0); dez < 1000; dez += 10)
        {
          var item = mil * 1000 + dez;

          var events = new List<EventData>
          {
            GetEvent(item + 1),
            GetEvent(item + 3),
            GetEvent(item + 7),
            GetEvent(item + 9)
          };

          await _eventStore.AppendToStreamAsync(
            InterfaceConst.PSPrime,
            StreamState.Any,
            events
          );
        }
      }
    }
    private EventData GetEvent(uint number)
    {
      return new EventData(
        number.ToUuid(),
        number.GetType().ToString(),
        JsonSerializer.SerializeToUtf8Bytes(number)
      );
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
  }
}
