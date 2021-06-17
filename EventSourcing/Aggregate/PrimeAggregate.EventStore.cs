using EventSourcing.Event;
using EventStore;
using EventStore.Client;
using Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EventSourcing.Aggregate
{
  public partial class PrimeAggregate : IAggregate
  {
    public IList<IsPrimeEvent> Events { get; private set; } = new List<IsPrimeEvent>();
    public IEnumerable<int> PrimeEvents => Events.Where(x => x.Prime.Value).Select(x => x.Number);

    private EventStoreClient _eventStore;

    public void Initialize_ES(EventStoreClient eventStore)
    {
      _eventStore = eventStore;

      var result = _eventStore.ReadStreamAsync(Direction.Forwards, InterfaceConst.ESPrime, StreamPosition.Start);
      if (result.ReadState.Result == ReadState.StreamNotFound)
        return;

      foreach (var vnt in result.ToEnumerable())
        Events.Add(vnt.To<IsPrimeEvent>());

      foreach (var @event in Events)
      {
        if (!@event.Prime.HasValue)
          IsPrime(@event.Number);
        else if (@event.Prime.Value)
          Primes.Add(@event.Number);
      }
    }

    public async Task Emit(IEvent @event)
    {
      var vnt = @event as IsPrimeEvent;
      Events.Add(vnt);

      await _eventStore.AppendToStreamAsync(
        InterfaceConst.ESPrime,
        StreamState.Any,
        new[] { vnt.GetEvent() }
      );
    }
  }
}
