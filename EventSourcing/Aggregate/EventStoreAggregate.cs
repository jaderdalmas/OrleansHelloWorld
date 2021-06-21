using EventSourcing.Event;
using EventStore;
using EventStore.Client;
using Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EventSourcing.Aggregate
{
  public partial class EventStoreAggregate<T> : IStoreAggregate<T>
    where T : IEvent
  {
    public IList<T> Events { get; private set; } = new List<T>();

    private EventStoreClient _eventStore;

    public EventStoreAggregate(EventStoreClient eventStore)
    {
      _eventStore = eventStore;

      var result = _eventStore.ReadStreamAsync(Direction.Forwards, InterfaceConst.ESPrime, StreamPosition.Start);
      if (result.ReadState.Result == ReadState.StreamNotFound)
        return;

      foreach (var vnt in result.ToEnumerable())
        Events.Add(vnt.To<T>());
    }

    public async Task Initialize(Func<T, Task> act)
    {
      foreach (var @event in Events)
        await act(@event);
    }

    public async Task Emit<TEvent>(TEvent @event) where TEvent : T
    {
      Events.Add(@event);

      await _eventStore.AppendToStreamAsync(
        InterfaceConst.ESPrime,
        StreamState.Any,
        new[] { @event.GetEvent() }
      );
    }
  }
}
