using EventSourcing.Aggregate;
using EventSourcing.Event;
using EventStore.Client;
using Interfaces;
using Interfaces.Model;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Providers;
using Orleans.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Grains
{
  [StorageProvider(ProviderName = GrainConst.Storage)]
  public partial class PrimeGrain : Grain<PrimeState>, IPrime, IAsyncDisposable
  {
    /// <summary>
    /// Logging Interface
    /// </summary>
    private readonly ILogger _logger;
    private readonly IESService<int> _es_service;
    private readonly IRXService<int> _rx_service;
    public Task Subscribe(IObserver<int> item) => _rx_service.Subscribe(item);
    public Task<VersionedValue<int>> GetAsync() => _rx_service.GetAsync();

    /// <summary>
    /// Prime Aggregate
    /// </summary>
    private readonly IPersistentState<PrimeAggregate> _aggregate;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="factory">factory logger</param>
    /// <param name="client">event store client</param>
    /// <param name="persist">event store persistent subscription client</param>
    public PrimeGrain(ILoggerFactory factory, IESService<int> es_service, IRXService<int> rx_service, [PersistentState("primes", GrainConst.Storage)] IPersistentState<PrimeAggregate> aggregate, EventStorePersistentSubscriptionsClient persist)
    {
      _logger = factory.CreateLogger<PrimeGrain>();
      _es_service = es_service;
      _rx_service = rx_service;
      _aggregate = aggregate;

      PrimeGrain_Persist(persist);
      PrimeGrain_Stream(factory);
    }

    /// <summary>
    /// Activate grain
    /// </summary>
    public Task Consume() => Task.CompletedTask;

    /// <summary>
    /// Activate grain (state and polling as ES and RC)
    /// </summary>
    public async override Task OnActivateAsync()
    {
      State.Initialize(WriteStateAsync);

      await OnActivatePersistAsync();

      await _es_service.Consume((int number) => IsPrime(number), InterfaceConst.PSPrime);

      await base.OnActivateAsync();
    }

    /// <summary>
    /// Handle IsPrime_Event
    /// </summary>
    /// <param name="event">event to be handled</param>
    public async Task<bool> Handle(IsPrimeEvent @event)
    {
      var result = await _aggregate.State.Apply(@event);

      if (result)
        await State_UpdateAsync(@event.Number);

      return result;
    }

    /// <summary>
    /// Verify if it is prime
    /// </summary>
    /// <param name="number">number</param>
    /// <returns>true if prime</returns>
    public async Task<bool> IsPrime(int number)
    {
      if (State.HasPrime(number))
      {
        _logger.LogInformation($"{number} is prime and is on the list");
        await _rx_service.UpdateAsync(number);
        return true;
      }

      var _primesqrt = State.GetPrimes(number);
      foreach (var prime in _primesqrt.AsParallel())
        if (number.IsDivisible(prime))
        {
          _logger.LogInformation($"{number} is not prime, divisible by {prime}");
          return false;
        }

      _logger.LogInformation($"{number} is prime and will be added on the list");

      await State_UpdateAsync(number);
      await _rx_service.UpdateAsync(number);

      return true;
    }

    /// <summary>
    /// State Poll
    /// </summary>
    private IDisposable _state_poll;
    public async Task State_UpdateAsync(int number)
    {
      await State.AddPrime(number);

      if (_state_poll == null)
        _state_poll = RegisterTimer(_ => State_WriteAsync(), null, TimeSpan.FromSeconds(1), TimeSpan.FromMinutes(1));
    }
    public async Task State_WriteAsync()
    {
      _state_poll = _state_poll.Clean();

      State.Primes = State.Primes.OrderBy(x => x).ToList();
      await WriteStateAsync();

      _aggregate.State.Order();
      await _aggregate.WriteStateAsync();
    }

    /// <summary>
    /// Dispose polling
    /// </summary>
    public async ValueTask DisposeAsync()
    {
      await State_WriteAsync();
      return;
    }
  }

  public class PrimeState
  {
    public void Initialize(Func<Task> act)
    {
      if (Primes == null)
      {
        Primes = new List<int>() { 2, 3, 5, 7, 11, 13, 17, 19, 23, 29, 31, 37, 41, 43, 47, 53, 59, 61, 67, 71, 73, 79, 83, 89, 97 };
        act.Invoke();
      }
    }

    public IList<int> Primes { get; set; }

    public bool HasPrime(int value) => Primes.Contains(value);
    public IEnumerable<int> GetPrimes(int value) => Primes.Where(x => x <= Math.Sqrt(value));

    public Task AddPrime(int value)
    {
      Primes.Add(value);

      return Task.CompletedTask;
    }
  }
}
