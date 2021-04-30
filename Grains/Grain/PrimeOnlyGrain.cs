using Interfaces;
using Interfaces.Model;
using Microsoft.Extensions.Logging;
using Orleans;
using System;
using System.Threading.Tasks;

namespace Grains
{
  public class PrimeOnlyGrain : Grain, IPrimeOnly
  {
    private readonly ILogger _logger;

    public PrimeOnlyGrain(ILogger<PrimeOnlyGrain> logger)
    {
      _logger = logger;
    }

    public override async Task OnActivateAsync()
    {
      var key = this.GetPrimaryKeyLong();
      // start long polling
      var func = this.RegisterRCPoolAsync(
          () => GrainFactory.GetGrain<IPrime>(key).GetAsync(),
          () => GrainFactory.GetGrain<IPrime>(key).LongPollAsync(_cache.Version),
          result => result.IsValid,
          apply =>
          {
            _cache = apply;
            _logger.LogInformation($"{DateTime.Now.TimeOfDay}: {nameof(PrimeOnlyGrain)} {key} updated value to {_cache.Value} with version {_cache.Version}");
            return Task.CompletedTask;
          },
          failed =>
          {
            _logger.LogWarning("The reactive poll timed out by returning a 'none' response before Orleans could break the promise.");
            return Task.CompletedTask;
          });
      _pool = RegisterTimer(_ => func, null, TimeSpan.Zero, TimeSpan.FromMilliseconds(1));

      await base.OnActivateAsync();
    }

    public Task Consume()
    {
      _logger.LogInformation("Starting to consume...");
      return Task.CompletedTask;
    }

    private VersionedValue<int> _cache;
    private IDisposable _pool;

    public Task<int> GetAsync() => Task.FromResult(_cache.Value);
  }
}
