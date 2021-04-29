using Orleans.TestingHost;
using System.Threading.Tasks;
using Xunit;

namespace Tests
{
  public class ClusterFixture : IAsyncLifetime
  {
    public ClusterFixture()
    {
      var builder = new TestClusterBuilder();

      builder.AddSiloBuilderConfigurator<ClusterSiloConfigurations>();

      Cluster = builder.Build();
    }

    public async Task InitializeAsync()
    {
      await Cluster.DeployAsync();
    }

    public async Task DisposeAsync()
    {
      await Cluster.StopAllSilosAsync();
      Cluster.Dispose();
    }

    public TestCluster Cluster { get; private set; }
  }
}
