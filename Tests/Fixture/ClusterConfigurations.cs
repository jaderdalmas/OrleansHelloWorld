using EventStore;
using Grains;
using Interfaces;
using Microsoft.Extensions.Configuration;
using Orleans;
using Orleans.Hosting;
using Orleans.TestingHost;

namespace Tests
{
  public class ClientConfigurations : IClientBuilderConfigurator
  {
    public void Configure(IConfiguration configuration, IClientBuilder hostBuilder)
    {
      hostBuilder
        .AddSimpleMessageStreamProvider(InterfaceConst.SMSProvider);

      hostBuilder.ConfigureServices(services =>
      {
        services.AddEventStoreService();
      });
    }
  }

  public class SiloConfigurations : ISiloConfigurator
  {
    public void Configure(ISiloBuilder hostBuilder)
    {
      hostBuilder.Configure_Grains()
        .AddMemoryGrainStorage(GrainConst.Storage)
        .AddSimpleMessageStreamProvider(InterfaceConst.SMSProvider)
        .AddMemoryGrainStorage(GrainConst.PSStore);

      hostBuilder.ConfigureServices(services =>
      {
        services.AddEventStoreService();
        services.AddGrainServices();
      });
    }
  }
}
