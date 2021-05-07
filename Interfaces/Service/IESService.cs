using System;
using System.Threading.Tasks;

namespace Interfaces
{
  public interface IESService<T> : IAsyncDisposable
  {
    Task Consume(Func<T, Task> action, string stream);
  }
}
