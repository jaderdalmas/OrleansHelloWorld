using Microsoft.Extensions.Logging;
using Orleans.Streams;
using System;
using System.Threading.Tasks;

namespace Interfaces.Model
{
  public class Observer<T> : IAsyncObserver<T>
  {
    private readonly ILogger _logger;
    private readonly Func<T, Task> _action;

    public Observer(ILoggerFactory factory, Func<T, Task> action)
    {
      _logger = factory.CreateLogger<Observer<T>>();
      _action = action;
    }

    public Task OnCompletedAsync() => Task.CompletedTask;

    public Task OnErrorAsync(Exception ex)
    {
      _logger.LogError(ex, ex.Message);
      return Task.CompletedTask;
    }

    public Task OnNextAsync(T item, StreamSequenceToken token = null) => _action(item);
  }
}
