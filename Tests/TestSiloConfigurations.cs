using GrainInterfaces;
using Grains;
using Microsoft.Extensions.DependencyInjection;
using Orleans.Hosting;
using Orleans.TestingHost;

namespace Tests
{
  public class TestSiloConfigurations : ISiloConfigurator
  {
    public void Configure(ISiloBuilder hostBuilder)
    {
      hostBuilder.ConfigureServices(services => {
        services.AddSingleton<IHello, HelloGrain>();
      });
    }
  }
}
