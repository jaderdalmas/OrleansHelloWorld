using System;

namespace Interfaces
{
  public interface IRCService<T> : IReactiveCacheFrom<T>, IAsyncDisposable
  {
  }
}
