using GrainInterfaces;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Providers;
using Orleans.Streams;
using Orleans.Streams.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Grains
{
  [ImplicitStreamSubscription(GrainInterfaces.AppConst.PSPrime)]
  [StorageProvider(ProviderName = AppConst.Storage)]
  public class PrimeGrain : Grain<PrimeState>, IPrime, IStreamSubscriptionObserver
  {
    private readonly ILogger logger;
    private readonly PrimeObserver observer;

    public PrimeGrain(ILogger<PrimeGrain> logger)
    {
      this.logger = logger;

      observer = new PrimeObserver(logger, (int number) => IsPrime(number));
    }

    public override Task OnActivateAsync()
    {
      State.Initialize(WriteStateAsync);

      return base.OnActivateAsync();
    }

    public async Task OnSubscribed(IStreamSubscriptionHandleFactory handleFactory)
    {
      var handle = handleFactory.Create<int>();
      await handle.ResumeAsync(observer);
    }

    public Task<bool> IsPrime(int number)
    {
      if (State.HasPrime(number))
      {
        logger.LogInformation($"{number} is prime and is on the list");
        return Task.FromResult(true);
      }

      var _primesqrt = State.GetPrimes(number);
      foreach (var prime in _primesqrt.AsParallel())
        if (number.IsDivisible(prime))
        {
          logger.LogInformation($"{number} is not prime, divisible by {prime}");
          return Task.FromResult(false);
        }

      logger.LogInformation($"{number} is prime and will be added on the list");

      State.AddPrime(number, WriteStateAsync);

      return Task.FromResult(true);
    }
  }

  public class PrimeObserver : IAsyncObserver<int>
  {
    private readonly ILogger logger;
    private readonly Func<int, Task> action;

    public PrimeObserver(ILogger<IPrime> logger, Func<int, Task> action)
    {
      this.logger = logger;
      this.action = action;
    }

    public Task OnCompletedAsync() => Task.CompletedTask;

    public Task OnErrorAsync(Exception ex)
    {
      logger.LogError(ex, ex.Message);
      return Task.CompletedTask;
    }

    public Task OnNextAsync(int item, StreamSequenceToken token = null) => action(item);
  }

  public class PrimeState : IDisposable
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

    private Func<Task> Act = null;
    public async Task AddPrime(int value, Func<Task> act)
    {
      Primes.Add(value);

      if (Act == null)
      {
        Act = act;
        await Task.Delay(1000);

        Primes = Primes.OrderBy(x => x).ToList();

        await Act.Invoke();
        Act = null;
      }
    }

    public void Dispose()
    {
      if (Act != null)
        Act.Invoke();
    }
  }
}
