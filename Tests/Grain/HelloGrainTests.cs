using EventStore;
using EventStore.Client;
using Interfaces;
using Orleans;
using Orleans.TestingHost;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;

namespace Tests.Grain
{
  [Collection(nameof(ClusterCollection))]
  public class HelloGrainTests
  {
    private readonly TestCluster _cluster;
    private readonly EventStoreClient _eventStore;

    public HelloGrainTests(ClusterFixture fixture)
    {
      _cluster = fixture.Cluster;
      _eventStore = fixture.EventStore;
    }

    [Fact]
    public async Task SaysHello_Call()
    {
      var text = "teste";

      var hello = _cluster.GrainFactory.GetGrain<IHello>(0);
      var greeting = await hello.SayHello(text);

      Assert.Equal($"\n Client: '{text}' | Grain: Hello 1 times! | Time: ", greeting.Substring(0, 50));
    }

    [Fact]
    public async Task SaysHello_Stream()
    {
      var grain = _cluster.Client.GetGrain<IHello>(0);
      var key = grain.GetGrainIdentity().PrimaryKey;

      var stream = _cluster.Client.GetStreamProvider(InterfaceConst.SMSProvider)
        .GetStream<string>(key, InterfaceConst.PSHello);

      var text = "teste";
      await stream.OnNextAsync(text);
    }

    [Fact]
    public async Task SaysHello_ES()
    {
      var grain = _cluster.Client.GetGrain<IHello>(0);
      await grain.Consume();

      await _eventStore.SoftDeleteAsync(InterfaceConst.PSHello, StreamState.Any);

      var evt = $"teste";
      var vnt = new EventData(
        evt.GetHashCode().ToUuid(),
        evt.GetType().ToString(),
        JsonSerializer.SerializeToUtf8Bytes(evt)
      );

      await _eventStore.AppendToStreamAsync(
        InterfaceConst.PSHello,
        StreamState.Any,
        new[] { vnt }
      );
    }
  }
}
