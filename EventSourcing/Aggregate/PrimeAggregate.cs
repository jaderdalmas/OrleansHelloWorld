using EventSourcing.Event;
using EventStore.Client;
using Interfaces;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EventSourcing.Aggregate
{
  public partial class PrimeAggregate
  {
    public IList<int> Primes { get; private set; } = new List<int>();

    public void Order()
    {
      Primes = Primes.OrderBy(x => x).ToList();
    }

    private ILogger _logger;

    public PrimeAggregate(ILoggerFactory factory, EventStoreClient eventStore)
    {
      _logger = factory.CreateLogger<PrimeAggregate>();

      Initialize_ES(eventStore);
    }

    public async Task<bool> Apply(IEvent @event)
    {
      var vnt = @event as IsPrimeEvent;

      var has = Events.FirstOrDefault(x => x.Number == vnt.Number);
      if (has != null)
      {
        _logger.LogInformation($"{vnt.Number} is{(has.Prime.Value ? string.Empty : " not")} prime and is on the event list");
        return has.Prime.Value;
      }

      if (!vnt.CanCalculate(PrimeEvents))
        await AddInnerPrimes(vnt.Number);

      vnt.IsPrime(PrimeEvents.ToList());
      _logger.LogInformation($"{vnt.Number} is{(has.Prime.Value ? string.Empty : " not")} prime and will be added on the event list");

      await Emit(vnt);
      return vnt.Prime.Value;
    }

    private bool IsPrime(int number)
    {
      if (number.IsPrime(PrimeEvents))
      {
        Primes.Add(number);
        return true;
      }

      return false;
    }

    private async Task AddInnerPrimes(int number)
    {
      var value = PrimeEvents.Any() ? PrimeEvents.Max() : PrimeConst.FirstPrime;

      while (value < number)
      {
        await Apply(new IsPrimeEvent() { Number = value });

        if (value == PrimeConst.FirstPrime) value++;
        else value += 2; //jump pair
      }
    }
  }
}
