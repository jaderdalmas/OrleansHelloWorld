using GrainInterfaces;
using Microsoft.Extensions.Logging;
using Orleans;
using System;
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
            clock.Restart();

            if (key.KeyChar == '1')
              HelloWorld.ManyHello(client);
            else if (key.KeyChar == '2')
              Prime.ManyPrime(client);
            else break;

            clock.Stop();
            Console.WriteLine($"Elapsed: {clock.ElapsedMilliseconds}ms");
            Console.WriteLine($"Elapsed Ticks: {clock.ElapsedTicks}");

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
  }
}
