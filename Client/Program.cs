using GrainInterfaces;
using Microsoft.Extensions.Logging;
using Orleans;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Client
{
  public class Program
  {
    public static int Main(string[] args)
    {
      return RunMainAsync().Result;
    }

    private static async Task<int> RunMainAsync()
    {
      try
      {
        using (var client = await ConnectClient())
        {
          Console.WriteLine("Will it be a 1-hello or a 2-prime test?");

          var key = Console.ReadKey();
          while (key != null)
          {
            var clock = new Stopwatch();
            if (key.KeyChar == '1')
            {
              var tasks = new List<Task>();
              for (int i = 1; i < 100; i++)
                tasks.Add(DoHello(client, i));

              Task.WaitAll(tasks.ToArray());
            }
            else if (key.KeyChar == '2')
            {
              clock.Restart();

              await DoMultiPrime(client);

              clock.Stop();
              Console.WriteLine($"Elapsed: {clock.ElapsedMilliseconds}ms");
              Console.WriteLine($"Elapsed Ticks: {clock.ElapsedTicks}");
            }
            else break;

            key = Console.ReadKey();
          }
        }

        return 0;
      }
      catch (Exception e)
      {
        Console.WriteLine($"\nException while trying to run client: {e.Message}");
        Console.WriteLine("Make sure the silo the client is trying to connect to is running.");
        Console.WriteLine("\nPress any key to exit.");
        Console.ReadKey();
        return 1;
      }
    }

    private static async Task<IClusterClient> ConnectClient()
    {
      IClusterClient client;
      client = new ClientBuilder()
          .UseLocalhostClustering()
          .Configure_ClusterOptions()
          .ConfigureLogging(logging => logging.AddConsole())
          .Build();

      await client.Connect();
      Console.WriteLine("Client successfully connected to silo host \n");
      return client;
    }

    private static async Task DoHello(IClusterClient client, int counter)
    { // example of calling grains from the initialized client
      var friend = client.GetGrain<IHello>(0);
      var response = await friend.SayHello($"Good morning, {counter}!");
      Console.WriteLine($"\n\n{response}\n\n");
    }

    private static async Task DoMultiPrime(IClusterClient client)
    {
      for (int mil = 0; mil < 100; mil++)
      {
        var tasks = new List<Task>();

        for (int dez = 0; dez < 1000; dez += 10)
        {
          var item = (mil == 0 ? 100 : 0) + mil * 1000 + dez;

          tasks.Add(DoSinglePrime(client, item + 1));
          tasks.Add(DoSinglePrime(client, item + 3));
          tasks.Add(DoSinglePrime(client, item + 7));
          tasks.Add(DoSinglePrime(client, item + 9));
        }

        Task.WaitAll(tasks.ToArray());
      }
    }

    private static async Task DoSinglePrime(IClusterClient client, int number)
    {
      var friend = client.GetGrain<IPrime>(0);
      var response = await friend.IsPrime(number);

      var isprime = response ? "is prime" : "is not prime";
      Console.WriteLine($"\n\n{number} {isprime}\n\n");
    }
  }
}
