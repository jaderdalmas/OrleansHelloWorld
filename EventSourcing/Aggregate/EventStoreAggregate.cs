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
    private string _stream;

    public EventStoreAggregate(EventStoreClient eventStore, string stream = InterfaceConst.ESPrime)
    {
      _eventStore = eventStore;
      _stream = stream;

      var result = _eventStore.ReadStreamAsync(Direction.Forwards, _stream, StreamPosition.Start);
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

      await _eventStore.AppendToStreamAsync(_stream,
        StreamState.Any, new[] { @event.GetEvent() }
      );
    }
  }
}
