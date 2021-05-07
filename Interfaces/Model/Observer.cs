using Microsoft.Extensions.Logging;
using Orleans.Streams;
using System;
using System.Threading.Tasks;

namespace Interfaces.Model
{
  public class Observer<T> : IAsyncObserver<T>
  {
    private readonly ILogger logger;
    private readonly Func<T, Task> action;

    public Observer(ILoggerFactory factory, Func<T, Task> action)
    {
      logger = factory.CreateLogger<Observer<T>>();
      this.action = action;
    }

    public Task OnCompletedAsync() => Task.CompletedTask;

    public Task OnErrorAsync(Exception ex)
    {
      logger.LogError(ex, ex.Message);
      return Task.CompletedTask;
    }

    public Task OnNextAsync(T item, StreamSequenceToken token = null) => action(item);
  }
}
