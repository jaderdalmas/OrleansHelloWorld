using EventStore;
using Grains;
using Grains.Storage;
using Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Configuration;
using Orleans.Hosting;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;

namespace Silo
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
        .UseOrleans(builder =>
        {
          builder
          .UseLocalhostClustering()
          .Configure_ClusterOptions()
          .Configure<EndpointOptions>(options => options.AdvertisedIPAddress = IPAddress.Loopback)
          .Configure_Grains(new List<Assembly>() { GrainConst.Assembly })
          .AddFileGrainStorage(GrainConst.Storage, opts =>
          {
            opts.RootDirectory = "./TestFiles";
          })
          .AddSimpleMessageStreamProvider(InterfaceConst.SMSProvider)
          .AddMemoryGrainStorage(GrainConst.PSStore)
          .UseDashboard(options =>
          {
            options.HideTrace = true;
          })
          .AddStartupTask(async (provider, token) =>
          {
            var factory = provider.GetService<IGrainFactory>();
            var client = provider.GetService<IClusterClient>();

            await factory.GetGrain<IHello>(0).Consume();
            await factory.GetGrain<IPrimeOnly>(0).Consume();
            //await factory.GetGrain<IPrime>(0).Consume();
          });
        })
        .ConfigureServices(services =>
        {
          services.Configure<EventStoreSettings>(settings => settings.Connection = "esdb://localhost:2113?tls=false");
          services.AddEventStoreService();
          services.AddGrainServices();

          services.Configure<ConsoleLifetimeOptions>(options =>
          {
            options.SuppressStatusMessages = true;
          });
        })
        .ConfigureLogging(builder =>
        {
          builder.AddConsole();
          builder.AddFilter("Grpc", LogLevel.Debug);
        });
    }

    [Obsolete("Orleans 2.2")]
    [SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "Maintain Knowledge")]
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

    [Obsolete("Orleans 2.2")]
    private static async Task<ISiloHost> StartSilo()
    {
      // define the cluster configuration
      var builder = new SiloHostBuilder()
        .UseLocalhostClustering()
        .Configure_ClusterOptions()
        .Configure<EndpointOptions>(options => options.AdvertisedIPAddress = IPAddress.Loopback)
        .Configure_Grains(new List<Assembly>() { GrainConst.Assembly })
        .ConfigureLogging(logging => logging.AddConsole())
        .AddFileGrainStorage(GrainConst.Storage, opts =>
        {
          opts.RootDirectory = "./TestFiles";
        });

      var host = builder.Build();
      await host.StartAsync();
      return host;
    }
  }
}
