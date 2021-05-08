using Interfaces;
using Interfaces.Model;
using Microsoft.Extensions.Logging;
using System;
using System.Reactive.Subjects;
using System.Threading.Tasks;

namespace Grains
{
  public class RXService<T> : IRXService<T>
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
    /// Subject
    /// </summary>
    private Subject<T> _jobs;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="factory">logger factory</param>
    public RXService(ILoggerFactory factory)
    {
      _logger = factory.CreateLogger<RCService<T>>();

      _value = VersionedValue<T>.None.NextVersion(default);

      _jobs = new Subject<T>();
    }

    /// <summary>
    /// Get Value
    /// </summary>
    /// <returns>Versioned Value</returns>
    public Task<VersionedValue<T>> GetAsync() => Task.FromResult(_value);

    public Task Subscribe(Func<T, Task> action)
    {
      _jobs.Subscribe((T job) => action.Invoke(job));

      return Task.CompletedTask;
    }

    /// <summary>
    /// Update Value Async
    /// </summary>
    /// <param name="value">value</param>
    public Task UpdateAsync(T value)
    {
      _value = _value.NextVersion(value);

      _logger.LogInformation($"{nameof(PrimeGrain)} updated value to {_value.Value} with version {_value.Version}");

      _jobs.OnNext(value);

      return Task.CompletedTask;
    }
  }
}
