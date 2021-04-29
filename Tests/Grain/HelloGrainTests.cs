using Interfaces;
using Orleans.TestingHost;
using System.Threading.Tasks;
using Xunit;

namespace Tests.Grain
{
  [Collection(nameof(ClusterCollection))]
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

      Assert.Equal($"\n Client: '{text}' | Grain: Hello 1 times! | Time: ", greeting.Substring(0, 50));
    }
  }
}
