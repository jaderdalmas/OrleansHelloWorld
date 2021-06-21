using EventStore.Client;
using Interfaces;
using System.Collections.Generic;
using System.Text.Json;

namespace EventSourcing.Event
{
  public class IsPrimeEvent : IEvent
  {
    public int Number { get; set; }

    public bool? Prime { get; set; }

    public bool CanCalculate(IEnumerable<int> primes) => primes.CanCalculate(Number);
    public bool IsPrime(IList<int> primes)
    {
      if (!Prime.HasValue)
        Prime = Number.IsPrime(primes);

      if (Prime.Value)
        primes.Add(Number);

      return Prime.Value;
    }
  }

  public static class IsPrimeEventExtension
  {
    public static EventData GetEvent(this IsPrimeEvent @event)
    {
      return new EventData(
        Uuid.NewUuid(),
        @event.GetType().ToString(),
        JsonSerializer.SerializeToUtf8Bytes(@event)
      );
    }
  }
}
