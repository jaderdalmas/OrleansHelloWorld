using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

public class RXObserver<T> : IObserver<T>
{
  private readonly ILogger logger;
  private readonly Func<T, Task> action;
  private IDisposable unsubscriber;

  public RXObserver(ILoggerFactory factory, Func<T, Task> action)
  {
    logger = factory.CreateLogger<RXObserver<T>>();
    this.action = action;
  }

  public virtual void Subscribe(IObservable<T> provider)
  {
    if (provider != null)
      unsubscriber = provider.Subscribe(this);
  }

  public virtual void OnCompleted() => unsubscriber.Dispose();

  public virtual void OnError(Exception e)
  {
    logger.LogError(e, e.Message);
  }

  public virtual void OnNext(T value) => action(value);
}