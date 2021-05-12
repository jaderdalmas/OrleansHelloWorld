using Interfaces;
using Interfaces.Model;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
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
    /// Observers
    /// </summary>
    private List<IObserver<T>> _observers;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="factory">logger factory</param>
    public RXService(ILoggerFactory factory)
    {
      _logger = factory.CreateLogger<RXService<T>>();

      _value = VersionedValue<T>.None.NextVersion(default);

      _observers = new List<IObserver<T>>();
    }

    /// <summary>
    /// Get Value
    /// </summary>
    /// <returns>Versioned Value</returns>
    public Task<VersionedValue<T>> GetAsync() => Task.FromResult(_value);

    public Task Subscribe(IObserver<T> observer)
    {
      if (!_observers.Contains(observer))
        _observers.Add(observer);

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

      foreach (var observer in _observers.AsParallel())
        observer.OnNext(value);

      return Task.CompletedTask;
    }

    public void OnCompleted()
    {
      foreach (var observer in _observers.AsParallel())
        if (_observers.Contains(observer))
          observer.OnCompleted();

      _observers.Clear();
    }
  }
}
