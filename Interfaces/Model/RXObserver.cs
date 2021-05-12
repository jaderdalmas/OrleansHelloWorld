using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

public class RXObserver<T> : IObserver<T>
{
  private readonly ILogger _logger;
  private Func<T, Task> _action;

  public RXObserver(ILoggerFactory factory, Func<T, Task> action)
  {
    _logger = factory.CreateLogger<RXObserver<T>>();
    _action = action;
  }

  public virtual void Subscribe(IObservable<T> provider)
  {
    if (provider != null)
      _ = provider.Subscribe(this);
  }

  public virtual void OnCompleted() { }

  public virtual void OnError(Exception e)
  {
    _logger.LogError(e, e.Message);
  }

  public virtual void OnNext(T value)
  {
    if (_action != null)
      _action(value).Wait();
  }
}