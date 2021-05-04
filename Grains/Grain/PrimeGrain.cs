using EventStore.Client;
using Interfaces;
using Interfaces.Model;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Providers;
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

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="logger">grain logger</param>
    /// <param name="client">event store client</param>
    public PrimeGrain(ILogger<PrimeGrain> logger, EventStoreClient client)
    {
      _logger = logger;
      _client = client; // Event Store
      // Stream
      observer = new Observer<int>(_logger, (int number) => IsPrime(number));
    }

    /// <summary>
    /// Activate grain
    /// </summary>
    public Task Consume()
    {
      _logger.LogInformation("Starting to consume...");
      return Task.CompletedTask;
    }

    /// <summary>
    /// Activate grain (state and pooling as ES and RC)
    /// </summary>
    public async override Task OnActivateAsync()
    {
      State.Initialize(WriteStateAsync);

      await OnActivateRCAsync();
      await OnActivateESAsync();

      await base.OnActivateAsync();
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
        await RC_UpdateAsync(number);
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
      await RC_UpdateAsync(number);
      await ES_UpdateAsync(number);

      return true;
    }

    /// <summary>
    /// State Pool
    /// </summary>
    private IDisposable _state_pool;
    public async Task State_UpdateAsync(int number)
    {
      await State.AddPrime(number);

      if (_state_pool == null)
        _state_pool = RegisterTimer(_ => State_WriteAsync(), null, TimeSpan.FromSeconds(1), TimeSpan.FromMinutes(1));
    }
    public async Task State_WriteAsync()
    {
      _state_pool = _state_pool.Clean();

      State.Primes = State.Primes.OrderBy(x => x).ToList();
      await WriteStateAsync();
    }

    /// <summary>
    /// Dispose pooling
    /// </summary>
    public async ValueTask DisposeAsync()
    {
      await DisposeRCAsync();
      await DisposeESAsync();

      _state_pool = _state_pool.Clean();
      await WriteStateAsync();
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
