using GrainInterfaces;
using GrainInterfaces.Model;
using Microsoft.Extensions.Logging;
using Orleans;
using System;
using System.Threading.Tasks;

namespace Grains
{
  public class PrimeOnlyGrain : Grain, IGrainWithIntegerKey, IReactiveCacheTo<int>
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
          () => GrainFactory.GetGrain<IPrime>(key).LongPollAsync(Cache.Version),
          result => result.IsValid,
          apply =>
          {
            Cache = apply;
            _logger.LogInformation($"{DateTime.Now.TimeOfDay}: {nameof(PrimeOnlyGrain)} {key} updated value to {Cache.Value} with version {Cache.Version}");
            return Task.CompletedTask;
          },
          failed =>
          {
            _logger.LogWarning("The reactive poll timed out by returning a 'none' response before Orleans could break the promise.");
            return Task.CompletedTask;
          });

      Pool = RegisterTimer(_ => func, null, TimeSpan.Zero, TimeSpan.FromTicks(1));

      await base.OnActivateAsync();
    }

    VersionedValue<int> IReactiveCacheTo<int>.Cache { get; set; }
    private VersionedValue<int> Cache
    {
      get => (this as IReactiveCacheTo<int>).Cache;
      set => (this as IReactiveCacheTo<int>).Cache = value;
    }
    IDisposable IReactiveCacheTo<int>.Pool { get; set; }
    private IDisposable Pool
    {
      get => (this as IReactiveCacheTo<int>).Pool;
      set => (this as IReactiveCacheTo<int>).Pool = value;
    }
  }
}
