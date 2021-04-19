using Orleans;
using Orleans.Configuration;
using Orleans.Hosting;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace GrainInterfaces
{
  public static class BuilderExtension
  {
    public static IClientBuilder Configure_ClusterOptions(this IClientBuilder builder)
    {
      return builder.Configure<ClusterOptions>(options =>
      {
        options.ClusterId = AppConst.ClusterId;
        options.ServiceId = AppConst.ServiceId;
      });
    }

    public static ISiloBuilder Configure_ClusterOptions(this ISiloBuilder builder)
    {
      return builder.Configure<ClusterOptions>(options =>
      {
        options.ClusterId = AppConst.ClusterId;
        options.ServiceId = AppConst.ServiceId;
      });
    }

    [Obsolete("Orleans 2.2")]
    public static ISiloHostBuilder Configure_ClusterOptions(this ISiloHostBuilder builder)
    {
      return builder.Configure<ClusterOptions>(options =>
      {
        options.ClusterId = AppConst.ClusterId;
        options.ServiceId = AppConst.ServiceId;
      });
    }

    public static ISiloBuilder Configure_Grains(this ISiloBuilder builder, IEnumerable<Assembly> assemblies)
    {
      return builder.ConfigureApplicationParts(parts =>
      {
        foreach (var assembly in assemblies)
          parts.AddApplicationPart(assembly).WithReferences();
      });
    }

    [Obsolete("Orleans 2.2")]
    public static ISiloHostBuilder Configure_Grains(this ISiloHostBuilder builder, IEnumerable<Assembly> assemblies)
    {
      return builder.ConfigureApplicationParts(parts =>
      {
        foreach (var assembly in assemblies)
          parts.AddApplicationPart(assembly).WithReferences();
      });
    }
  }
}
