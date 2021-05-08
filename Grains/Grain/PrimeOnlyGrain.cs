using EventStore;
using EventStore.Client;
using Interfaces;
using Interfaces.Model;
using Microsoft.Extensions.Logging;
using Orleans;
using System;
using System.Text.Json;
using System.Threading.Tasks;

namespace Grains
{
  public class PrimeOnlyGrain : Grain, IPrimeOnly
  {
    private readonly ILogger _logger;
    private readonly EventStoreClient _client;

    public PrimeOnlyGrain(ILogger<PrimeOnlyGrain> logger, EventStoreClient eventStore)
    {
      _logger = logger;
      _client = eventStore;
    }

    public async Task Consume()
    {
      Func<int, Task> func = (int number) => UpdateAsync(number);
      var key = this.GetPrimaryKeyLong();
      await GrainFactory.GetGrain<IPrime>(key).SubscribeAsync(func);
      return;
    }

    public override async Task OnActivateAsync()
    {
      //_poll = RegisterTimer(_ => RC_Initialize(), null, TimeSpan.Zero, TimeSpan.FromMilliseconds(100));

      await base.OnActivateAsync();
    }

    private IDisposable _poll;
    private async Task RC_Initialize()
    {
      var key = this.GetPrimaryKeyLong();

      var update = await GrainFactory.GetGrain<IPrime>(key).LongPollAsync(_cache.Version);
      if (!update.IsValid)
      {
        _logger.LogWarning("The reactive poll timed out by returning a 'none' response before Orleans could break the promise.");
        return;
      }

      _cache = update;
      _logger.LogInformation($"{DateTime.Now.TimeOfDay}: {nameof(PrimeOnlyGrain)} {key} updated value to {_cache.Value} with version {_cache.Version}");
      await UpdateAsync(update.Value);
    }

    /// <summary>
    /// Add ES prime only number
    /// </summary>
    /// <param name="number">number</param>
    public Task UpdateAsync(int number)
    {
      var vnt = new EventData(
        number.ToUuid(),
        number.GetType().ToString(),
        JsonSerializer.SerializeToUtf8Bytes(number)
      );

      return _client.AppendToStreamAsync(
        InterfaceConst.PSPrimeOnly,
        StreamState.Any,
        new[] { vnt }
      );
    }

    private VersionedValue<int> _cache;
    public Task<int> GetAsync() => Task.FromResult(_cache.Value);
  }
}
