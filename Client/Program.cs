using GrainInterfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Orleans;
using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace Client
{
  public class Program
  {
    public static Task Main(string[] args)
    {
      return StartHostBuilder(args).RunConsoleAsync();
    }

    private static IHostBuilder StartHostBuilder(string[] args)
    {
      return Host.CreateDefaultBuilder(args)
        .ConfigureServices(services =>
        {
          services.AddSingleton<ClusterClientHostedService>();
          services.AddSingleton<IHostedService>(_ => _.GetService<ClusterClientHostedService>());
          services.AddSingleton(_ => _.GetService<ClusterClientHostedService>().Client);

          services.AddHostedService<HelloWorldClientHostedService>();
          services.AddHostedService<PrimeClientHostedService>();

          services.Configure<ConsoleLifetimeOptions>(options =>
          {
            options.SuppressStatusMessages = true;
          });
        })
        .ConfigureLogging(builder => { builder.AddConsole(); });
    }

    [Obsolete("Orleans 2.2")]
    [SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "Maintain Knowledge")]
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

    [Obsolete("Orleans 2.2")]
    private static async Task<IClusterClient> ConnectClient()
    {
      IClusterClient client = new ClientBuilder()
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
