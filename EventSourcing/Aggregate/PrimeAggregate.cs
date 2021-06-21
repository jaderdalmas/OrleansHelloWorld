using EventSourcing.Event;
using Interfaces;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EventSourcing.Aggregate
{
  public class PrimeAggregate : IEventAggregate<IsPrimeEvent>
  {
    public IList<int> Primes { get; private set; } = new List<int>();
    //public IEnumerable<int> PrimeEvents => _aggregate.GetPrimes();

    public void Order()
    {
      Primes = Primes.OrderBy(x => x).ToList();
    }

    private ILogger _logger;
    private IStoreAggregate<IsPrimeEvent> _aggregate;

    public PrimeAggregate(ILoggerFactory factory, IStoreAggregate<IsPrimeEvent> aggregate)
    {
      _logger = factory.CreateLogger<PrimeAggregate>();
      _aggregate = aggregate;

      _aggregate.Initialize((IsPrimeEvent @event) => Initialize(@event)).Wait();
      //Initialize_ES(eventStore);
    }

    private Task Initialize(IsPrimeEvent @event)
    {
      if (!@event.Prime.HasValue)
        IsPrime(@event.Number);
      else if (@event.Prime.Value)
        Primes.Add(@event.Number);

      return Task.CompletedTask;
    }

    public async Task<bool> Apply(IsPrimeEvent @event)
    {
      var has = _aggregate.Events.FirstOrDefault(x => x.Number == @event.Number);
      if (has != null)
      {
        _logger.LogInformation($"{@event.Number} is{(has.Prime.Value ? string.Empty : " not")} prime and is on the event list");
        return has.Prime.Value;
      }

      if (!@event.CanCalculate(Primes))
        await AddInnerPrimes(@event.Number);

      @event.IsPrime(Primes);
      _logger.LogInformation($"{@event.Number} is{(has.Prime.Value ? string.Empty : " not")} prime and will be added on the event list");

      await _aggregate.Emit(@event);
      return @event.Prime.Value;
    }

    private bool IsPrime(int number)
    {
      if (number.IsPrime(Primes))
      {
        Primes.Add(number);
        return true;
      }

      return false;
    }

    private async Task AddInnerPrimes(int number)
    {
      var value = Primes.Any() ? Primes.Max() : PrimeConst.FirstPrime;

      while (value < number)
      {
        await Apply(new IsPrimeEvent() { Number = value });

        if (value == PrimeConst.FirstPrime) value++;
        else value += 2; //jump pair
      }
    }
  }
}
