using GrainInterfaces;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Grains
{
  [StorageProvider(ProviderName = AppConst.Storage)]
  public class PrimeGrain : Grain<PrimeState>, IPrime
  {
    private readonly ILogger logger;
    public PrimeGrain(ILogger<PrimeGrain> logger)
    {
      this.logger = logger;
    }

    public override Task OnActivateAsync()
    {
      State.Initialize();

      return base.OnActivateAsync();
    }

    Task<bool> IPrime.IsPrime(int number)
    {
      if (State.Primes.Contains(number))
      {
        logger.LogInformation($"{number} is prime and is on the list");
        return Task.FromResult(true);
      }

      var _primesqrt = State.Primes.Where(x => x <= Math.Sqrt(number));
      foreach (var prime in _primesqrt.AsParallel())
        if (number.IsDivisible(prime))
        {
          logger.LogInformation($"{number} is not prime, divisible by {prime}");
          return Task.FromResult(false);
        }

      logger.LogInformation($"{number} is prime and will be added on the list");
      State.Primes.Add(number);
      return Task.FromResult(true);
    }
  }

  public class PrimeState
  {
    public void Initialize()
    {
      if(Primes == null)
        Primes = new List<int>() { 2, 3, 5, 7, 11, 13, 17, 19, 23, 29, 31, 37, 41, 43, 47, 53, 59, 61, 67, 71, 73, 79, 83, 89, 97 };
    }

    public List<int> Primes { get; set; }
  }
}
