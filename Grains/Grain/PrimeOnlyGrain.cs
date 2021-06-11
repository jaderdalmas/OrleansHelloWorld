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
    private readonly IObserver<int> _observer;

    public PrimeOnlyGrain(ILoggerFactory factory, EventStoreClient eventStore)
    {
      _logger = factory.CreateLogger<PrimeOnlyGrain>();
      _client = eventStore;

      _observer = new RXObserver<int>(factory, (int number) => this.UpdateAsync(number));
    }

    public Task Consume() => Task.CompletedTask;

    public override async Task OnActivateAsync()
    {
      var key = this.GetPrimaryKeyLong();
      _ = GrainFactory.GetGrain<IPrime>(key).Subscribe(_observer);

      await base.OnActivateAsync();
    }

    public override Task OnDeactivateAsync()
    {
      _observer.OnCompleted();
      return Task.CompletedTask;
    }

    /// <summary>
    /// Add ES prime only number
    /// </summary>
    /// <param name="number">number</param>
    public Task UpdateAsync(int number)
    {
      var key = this.GetPrimaryKeyLong();
      _cache = _cache.NextVersion(number);
      _logger.LogInformation($"{DateTime.Now.TimeOfDay}: {nameof(PrimeOnlyGrain)} {key} updated value to {_cache.Value} with version {_cache.Version}");

      return _client.AppendToStreamAsync(
        InterfaceConst.PSPrimeOnly,
        StreamState.Any,
        new[] { number.GetEvent() }
      );
    }

    private VersionedValue<int> _cache;
    public Task<int> GetAsync() => Task.FromResult(_cache.Value);
  }
}
