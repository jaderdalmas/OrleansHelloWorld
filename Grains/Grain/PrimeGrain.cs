using GrainInterfaces;
using Microsoft.Extensions.Logging;
using Orleans;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Grains
{
  public class PrimeGrain : Grain, IPrime
  {
    private readonly ILogger logger;
    private List<int> _primes;

    public PrimeGrain(ILogger<PrimeGrain> logger)
    {
      this.logger = logger;

      _primes = new List<int>() { 2, 3, 5, 7, 11, 13, 17, 19, 23, 29, 31, 37, 41, 43, 47, 53, 59, 61, 67, 71, 73, 79, 83, 89, 97 };
    }

    Task<bool> IPrime.IsPrime(int number)
    {
      if (_primes.Contains(number))
      {
        logger.LogInformation($"{number} is prime and is on the list");
        return Task.FromResult(true);
      }

      var _primesqrt = _primes.Where(x => x <= Math.Sqrt(number));
      foreach (var prime in _primesqrt.AsParallel())
        if (number.IsDivisible(prime))
        {
          logger.LogInformation($"{number} is not prime, divisible by {prime}");
          return Task.FromResult(false);
        }

      logger.LogInformation($"{number} is prime and will be added on the list");
      _primes.Add(number);
      return Task.FromResult(true);
    }
  }
}
