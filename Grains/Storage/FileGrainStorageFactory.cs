using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Orleans.Configuration.Overrides;
using Orleans.Storage;
using System;

namespace Grains.Storage
{
  public static class FileGrainStorageFactory
  {
    internal static IGrainStorage Create(IServiceProvider services, string name)
    {
      IOptionsMonitor<FileGrainStorageOptions> optionsSnapshot = services.GetRequiredService<IOptionsMonitor<FileGrainStorageOptions>>();
      return ActivatorUtilities.CreateInstance<FileGrainStorage>(services, name, optionsSnapshot.Get(name), services.GetProviderClusterOptions(name));
    }
  }
}
