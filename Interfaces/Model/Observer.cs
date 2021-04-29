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

    public Observer(ILogger logger, Func<T, Task> action)
    {
      this.logger = logger;
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
