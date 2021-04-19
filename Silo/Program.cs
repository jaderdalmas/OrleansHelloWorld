﻿using GrainInterfaces;
using Grains;
using Grains.Storage;
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
    [SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "Program Main Method")]
    public static Task Main(string[] args)
    {
      return StartHostBuilder().RunConsoleAsync();
    }

    private static IHostBuilder StartHostBuilder()
    {
      return new HostBuilder()
        .UseOrleans(builder =>
        {
          builder
              .UseLocalhostClustering()
              .Configure_ClusterOptions()
              .Configure<EndpointOptions>(options => options.AdvertisedIPAddress = IPAddress.Loopback)
              .Configure_Grains(new List<Assembly>() { Grains.AppConst.Assembly })
              .AddFileGrainStorage(Grains.AppConst.Storage, opts =>
              {
                opts.RootDirectory = "./TestFiles";
              });
        })
        .ConfigureServices(services =>
        {
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
          .Configure_Grains(new List<Assembly>() { Grains.AppConst.Assembly })
          .ConfigureLogging(logging => logging.AddConsole())
          .AddFileGrainStorage(Grains.AppConst.Storage, opts =>
          {
            opts.RootDirectory = "./TestFiles";
          });

      var host = builder.Build();
      await host.StartAsync();
      return host;
    }
  }
}
