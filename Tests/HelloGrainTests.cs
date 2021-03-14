using GrainInterfaces;
using Grains;
using Orleans.TestingHost;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Tests
{
  [Collection(ClusterCollection.Name)]
  public class HelloGrainTests
  {
    private readonly TestCluster _cluster;

    public HelloGrainTests(ClusterFixture fixture)
    {
      _cluster = fixture.Cluster;
    }

    [Fact]
    public async Task SaysHelloCorrectly()
    {
      var text = "teste";

      var hello = _cluster.GrainFactory.GetGrain<IHello>(0);
      var greeting = await hello.SayHello(text);

      Assert.Equal($"\n Client said: '{text}', so HelloGrain says: Hello!", greeting);
    }
  }
}
