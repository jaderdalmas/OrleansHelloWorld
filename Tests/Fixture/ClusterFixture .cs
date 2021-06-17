using EventStore.Client;
using Orleans;
using Orleans.TestingHost;
using System.Threading.Tasks;
using Tests.Extension;
using Xunit;

namespace Tests
{
  public class ClusterFixture : IAsyncLifetime
  {
    public ClusterFixture()
    {
      var builder = new TestClusterBuilder(1);

      builder.AddSiloBuilderConfigurator<SiloConfigurations>();
      builder.AddClientBuilderConfigurator<ClientConfigurations>();

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
    public IClusterClient Orleans => Cluster.Client;
    public EventStoreClient EventStore => Cluster.GetService<EventStoreClient>();
  }
}
