using GrainInterfaces;
using Orleans;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Client
{
  public static class Prime
  {
    public static async Task DoSinglePrime(IClusterClient client, int number)
    {
      var friend = client.GetGrain<IPrime>(0);
      var response = await friend.IsPrime(number);

      var isprime = response ? "is prime" : "is not prime";
      Console.WriteLine($"\n\n{number} {isprime}\n\n");
    }

    public static void ManyPrime(IClusterClient client)
    {
      for (int mil = 0; mil < 100; mil++)
      {
        var tasks = new List<Task>();

        for (int dez = mil == 0 ? 100 : 0; dez < 1000; dez += 10)
        {
          var item = mil * 1000 + dez;

          tasks.Add(DoSinglePrime(client, item + 1));
          tasks.Add(DoSinglePrime(client, item + 3));
          tasks.Add(DoSinglePrime(client, item + 7));
          tasks.Add(DoSinglePrime(client, item + 9));
        }

        Task.WaitAll(tasks.ToArray());
      }
    }
  }
}
