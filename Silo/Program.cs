using GrainInterfaces;
using Grains;
using Grains.Storage;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Configuration;
using Orleans.Hosting;
using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;

namespace Silo
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
        var host = await StartSilo();
        Console.WriteLine("\n\n Press Enter to terminate...\n\n");
        Console.ReadLine();

        await host.StopAsync();

        return 0;
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex);
        return 1;
      }
    }

    private static async Task<ISiloHost> StartSilo()
    {
      // define the cluster configuration
      var builder = new SiloHostBuilder()
          .UseLocalhostClustering()
          .Configure_ClusterOptions()
          .Configure<EndpointOptions>(options => options.AdvertisedIPAddress = IPAddress.Loopback)
          .Configure_Grains()
          .ConfigureLogging(logging => logging.AddConsole())
          .AddFileGrainStorage("File", opts =>
          {
            opts.RootDirectory = "./TestFiles";
          });

      var host = builder.Build();
      await host.StartAsync();
      return host;
    }
  }
}
