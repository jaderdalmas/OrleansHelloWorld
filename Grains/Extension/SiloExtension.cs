using Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Orleans.Hosting;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Grains
{
  public static class SiloExtension
  {
    public static ISiloBuilder Configure_Grains(this ISiloBuilder builder)
    {
      return builder.ConfigureServices(services =>
      {
        services.AddSingleton<IHello, HelloGrain>();
        services.AddSingleton<IHelloArchive, HelloArchiveGrain>();
        services.AddSingleton<IPrime, PrimeGrain>();
        services.AddSingleton<IPrimeOnly, PrimeOnlyGrain>();
      });
    }

    [Obsolete("Orleans 2.2")]
    public static ISiloHostBuilder Configure_Grains(this ISiloHostBuilder builder)
    {
      var assemblies = new List<Assembly>() {
        typeof(HelloGrain).Assembly
      };

      return builder.Configure_Grains(assemblies);
    }
  }
}
