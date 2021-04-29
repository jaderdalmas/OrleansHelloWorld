using Grains;
using Orleans.Hosting;
using Orleans.TestingHost;

namespace Tests
{
  public class ClusterSiloConfigurations : ISiloConfigurator
  {
    public void Configure(ISiloBuilder hostBuilder)
    {
      hostBuilder.Configure_Grains()
        .AddMemoryGrainStorage(name: GrainConst.Storage)
        .AddSimpleMessageStreamProvider(Interfaces.InterfaceConst.SMSProvider)
        .AddMemoryGrainStorage(GrainConst.PSStore);
    }
  }
}
