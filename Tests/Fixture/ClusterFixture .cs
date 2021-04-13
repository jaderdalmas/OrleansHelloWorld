using Orleans.TestingHost;
using System;

namespace Tests
{
  public class ClusterFixture : IDisposable
  {
    public ClusterFixture()
    {
      var builder = new TestClusterBuilder();
      builder.AddSiloBuilderConfigurator<TestSiloConfigurations>();
      this.Cluster = builder.Build();
      this.Cluster.Deploy();
    }

    public void Dispose()
    {
      this.Cluster.StopAllSilos();
    }

    public TestCluster Cluster { get; private set; }
  }
}
