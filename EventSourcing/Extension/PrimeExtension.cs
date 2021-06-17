using Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EventSourcing
{
  public static class PrimeExtension
  {
    public static bool CanCalculate(this IEnumerable<int> primes, int number)
      => number == PrimeConst.FirstPrime || primes.Any(x => x >= Math.Sqrt(number));

    public static IEnumerable<int> GetSqrt(this IEnumerable<int> primes, int number) => primes.Where(x => x <= Math.Sqrt(number));

    public static bool IsPrime(this int number, IEnumerable<int> primes)
    {
      if (number < 3) return number == PrimeConst.FirstPrime;

      if (number.HasPrime(primes))
        return true;

      var _primesqrt = primes.GetSqrt(number);
      return number.CalcPrime(_primesqrt);
    }

    public static bool HasPrime(this int number, IEnumerable<int> primes) => primes.Contains(number);

    public static bool CalcPrime(this int number, IEnumerable<int> primes)
    {
      foreach (var prime in primes.AsParallel())
        if ((decimal)number % prime == 0)
          return false;

      return true;
    }
  }
}
