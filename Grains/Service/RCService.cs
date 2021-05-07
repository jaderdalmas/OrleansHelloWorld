using Interfaces;
using Interfaces.Model;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Grains
{
  public class RCService<T> : IRCService<T>
  {
    /// <summary>
    /// Logging Interface
    /// </summary>
    private readonly ILogger _logger;

    /// <summary>
    /// Value
    /// </summary>
    private VersionedValue<T> _value;

    /// <summary>
    /// Poll
    /// </summary>
    private TaskCompletionSource<VersionedValue<T>> _rc_poll;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="factory">logger factory</param>
    public RCService(ILoggerFactory factory)
    {
      _logger = factory.CreateLogger<RCService<T>>();

      _value = VersionedValue<T>.None.NextVersion(default);
      _rc_poll = new TaskCompletionSource<VersionedValue<T>>();
    }

    /// <summary>
    /// Get Value
    /// </summary>
    /// <returns>Versioned Value</returns>
    public Task<VersionedValue<T>> GetAsync() => Task.FromResult(_value);

    /// <summary>
    /// Long Poll
    /// </summary>
    /// <param name="knownVersion"></param>
    /// <returns>Versioned Value</returns>
    public Task<VersionedValue<T>> LongPollAsync(VersionToken knownVersion) =>
            knownVersion == _value.Version
            ? _rc_poll.Task.WithDefaultOnTimeout(TimeSpan.FromSeconds(1), VersionedValue<T>.None)
            : Task.FromResult(_value);

    /// <summary>
    /// Update Value Async
    /// </summary>
    /// <param name="value">value</param>
    public Task UpdateAsync(T value)
    {
      _value = _value.NextVersion(value);

      _logger.LogInformation($"{nameof(PrimeGrain)} updated value to {_value.Value} with version {_value.Version}");

      // fulfill waiting promises
      _rc_poll.SetResult(_value);
      _rc_poll = new TaskCompletionSource<VersionedValue<T>>();

      return Task.CompletedTask;
    }

    /// <summary>
    /// Dispose Reactive Cache
    /// </summary>
    public ValueTask DisposeAsync()
    {
      _rc_poll.SetCanceled();
      return new ValueTask();
    }
  }
}
