using Grains;
using Orleans.Hosting;
using Orleans.TestingHost;

namespace Tests
{
  public class TestSiloConfigurations : ISiloConfigurator
  {
    public void Configure(ISiloBuilder hostBuilder)
    {
      hostBuilder.Configure_Grains();
    }
  }
}
