using Interfaces.Model;
using Microsoft.Extensions.Logging;
using Orleans;
using System;
using System.Threading.Tasks;

namespace Grains
{
  public partial class PrimeGrain
  {
    /// <summary>
    /// Value
    /// </summary>
    private VersionedValue<int> _value;

    /// <summary>
    /// Poll
    /// </summary>
    private TaskCompletionSource<VersionedValue<int>> _rc_poll;

    /// <summary>
    /// Initialize Value and Poll wait
    /// </summary>
    public Task OnActivateRCAsync()
    {
      _value = VersionedValue<int>.None.NextVersion(0);
      _rc_poll = new TaskCompletionSource<VersionedValue<int>>();

      return Task.CompletedTask;
    }

    /// <summary>
    /// Get Value
    /// </summary>
    /// <returns>Versioned Value</returns>
    public Task<VersionedValue<int>> GetAsync() => Task.FromResult(_value);

    /// <summary>
    /// Long Poll
    /// </summary>
    /// <param name="knownVersion"></param>
    /// <returns>Versioned Value</returns>
    public Task<VersionedValue<int>> LongPollAsync(VersionToken knownVersion) =>
            knownVersion == _value.Version
            ? _rc_poll.Task.WithDefaultOnTimeout(TimeSpan.FromSeconds(1), VersionedValue<int>.None)
            : Task.FromResult(_value);

    /// <summary>
    /// Update Value Async
    /// </summary>
    /// <param name="value">value</param>
    public Task RC_UpdateAsync(int value)
    {
      _value = _value.NextVersion(value);

      var key = this.GetPrimaryKeyLong();
      _logger.LogInformation($"{nameof(PrimeGrain)} {key} updated value to {_value.Value} with version {_value.Version}");

      // fulfill waiting promises
      _rc_poll.SetResult(_value);
      _rc_poll = new TaskCompletionSource<VersionedValue<int>>();

      return Task.CompletedTask;
    }

    /// <summary>
    /// Dispose Reactive Cache
    /// </summary>
    private ValueTask DisposeRCAsync()
    {
      _rc_poll.SetCanceled();
      return new ValueTask();
    }
  }
}
