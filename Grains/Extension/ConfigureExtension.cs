using GrainInterfaces;
using Microsoft.Extensions.DependencyInjection;
using Orleans.Hosting;
using System.Collections.Generic;
using System.Reflection;

namespace Grains
{
  public static class BuilderExtension
  {
    public static ISiloBuilder Configure_Grains(this ISiloBuilder builder)
    {
      return builder.ConfigureServices(services =>
      {
        services.AddSingleton<IHello, HelloGrain>();
        services.AddSingleton<IPrime, PrimeGrain>();
      });
    }

    public static ISiloHostBuilder Configure_Grains(this ISiloHostBuilder builder)
    {
      var assemblies = new List<Assembly>() {
        typeof(HelloGrain).Assembly
      };

      return builder.Configure_Grains(assemblies);
    }
  }
}
