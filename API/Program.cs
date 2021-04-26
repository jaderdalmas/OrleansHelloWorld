
using GrainInterfaces;
using Grains.Storage;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Orleans;
using Orleans.Configuration;
using Orleans.Hosting;
using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;

namespace API
{
  public class Program
  {
    public static Task Main(string[] args)
    {
      return CreateHostBuilder(args).Build().RunAsync();
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
      Host.CreateDefaultBuilder(args)
      //.UseOrleans(siloBuilder =>
      //{
      //  siloBuilder
      //  .UseLocalhostClustering()
      //  //.Configure<HostOptions>(options => options.ShutdownTimeout = TimeSpan.FromMinutes(1))
      //  .Configure_ClusterOptions()
      //  .Configure<EndpointOptions>(opts => { opts.AdvertisedIPAddress = IPAddress.Loopback; })
      //  .Configure_Grains(new List<Assembly>() { Grains.AppConst.Assembly })
      //  .AddFileGrainStorage(Grains.AppConst.Storage, opts =>
      //  {
      //    opts.RootDirectory = "./TestFiles";
      //  })
      //  .AddMemoryGrainStorageAsDefault()
      //  .AddSimpleMessageStreamProvider(AppConst.SMSProvider)
      //  .AddMemoryGrainStorage(Grains.AppConst.PSStore)
      //  .UseDashboard(options =>
      //  {
      //    options.HideTrace = true;
      //  });
      //})
      .ConfigureWebHostDefaults(webBuilder =>
      {
        webBuilder.UseStartup<Startup>()
        .UseKestrel(options => options.AddServerHeader = false);
      });
  }
}
