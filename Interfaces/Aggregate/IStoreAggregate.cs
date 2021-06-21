using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Interfaces
{
  public interface IStoreAggregate<T> where T : IEvent
  {
    IList<T> Events { get; }

    Task Emit<TEvent>(TEvent @event) where TEvent : T;

    Task Initialize(Func<T, Task> act);
  }
}
