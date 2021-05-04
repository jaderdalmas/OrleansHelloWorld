using EventStore;
using EventStore.Client;
using Interfaces;
using Orleans;
using Orleans.TestingHost;
using System;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;

namespace Tests.Grain
{
  [Collection(nameof(ClusterCollection))]
  public class PrimeGrainTests : IAsyncDisposable
  {
    private readonly TestCluster _cluster;
    private readonly EventStoreClient _eventStore;

    public PrimeGrainTests(ClusterFixture fixture)
    {
      _cluster = fixture.Cluster;
      _eventStore = fixture.EventStore;
    }

    [Theory]
    [InlineData(101)]
    [InlineData(103)]
    [InlineData(107)]
    [InlineData(109)]
    [InlineData(113)]
    public async Task IsPrime_Call(int number)
    {
      // Arrange
      var grain = _cluster.GrainFactory.GetGrain<IPrime>(0);
      await grain.Consume();
      // Act
      var isprime = await grain.IsPrime(number);
      // Assert
      Assert.True(isprime);
    }

    [Theory]
    [InlineData(101)]
    [InlineData(103)]
    [InlineData(107)]
    [InlineData(109)]
    [InlineData(113)]
    public async Task IsPrime_Stream(int number)
    {
      // Arrange
      var grain = _cluster.Client.GetGrain<IPrime>(0);
      var key = grain.GetGrainIdentity().PrimaryKey;

      await _eventStore.SoftDeleteAsync(InterfaceConst.PSPrime, StreamState.Any);
      var stream = _cluster.Client.GetStreamProvider(InterfaceConst.SMSProvider)
        .GetStream<int>(key, InterfaceConst.PSPrime);
      // Act
      await stream.OnNextAsync(number);
      // Assert
      var item = await grain.GetAsync();
      Assert.True(item.IsValid);
      Assert.Equal(number, item.Value);
    }

    [Theory]
    [InlineData(101)]
    [InlineData(103)]
    [InlineData(107)]
    [InlineData(109)]
    [InlineData(113)]
    public async Task IsPrime_ES(int number)
    {
      // Arrange
      var grain = _cluster.Client.GetGrain<IPrime>(0);
      await _eventStore.SoftDeleteAsync(InterfaceConst.PSPrime, StreamState.Any);
      await grain.Consume();

      var vnt = new EventData(
        number.GetHashCode().ToUuid(),
        number.GetType().ToString(),
        JsonSerializer.SerializeToUtf8Bytes(number)
      );
      // Act
      await _eventStore.AppendToStreamAsync(
        InterfaceConst.PSPrime,
        StreamState.Any,
        new[] { vnt }
      );
      await Task.Delay(TimeSpan.FromMilliseconds(100));
      // Assert
      var item = await grain.GetAsync();
      Assert.True(item.IsValid);
      Assert.Equal(number, item.Value);
    }

    [Theory]
    [InlineData(105)]
    [InlineData(111)]
    [InlineData(115)]
    [InlineData(117)]
    public async Task IsNotPrime_Call(int number)
    {
      // Arrange
      var grain = _cluster.GrainFactory.GetGrain<IPrime>(0);
      // Act
      var isprime = await grain.IsPrime(number);
      // Assert
      Assert.False(isprime);
    }

    [Theory]
    [InlineData(105)]
    [InlineData(111)]
    [InlineData(115)]
    [InlineData(117)]
    public async Task IsNotPrime_Stream(int number)
    {
      // Arrange
      var grain = _cluster.Client.GetGrain<IPrime>(0);
      var key = grain.GetGrainIdentity().PrimaryKey;

      var stream = _cluster.Client.GetStreamProvider(InterfaceConst.SMSProvider)
        .GetStream<int>(key, InterfaceConst.PSPrime);
      // Act
      await stream.OnNextAsync(number);
      // Assert
      var item = await grain.GetAsync();
      Assert.True(item.IsValid);
      Assert.NotEqual(number, item.Value);
    }

    [Theory]
    [InlineData(105)]
    [InlineData(111)]
    [InlineData(115)]
    [InlineData(117)]
    public async Task IsNotPrime_ES(int number)
    {
      // Arrange
      var grain = _cluster.Client.GetGrain<IPrime>(0);
      await _eventStore.SoftDeleteAsync(InterfaceConst.PSPrime, StreamState.Any);
      await grain.Consume();

      var vnt = new EventData(
        number.GetHashCode().ToUuid(),
        number.GetType().ToString(),
        JsonSerializer.SerializeToUtf8Bytes(number)
      );
      // Act
      await _eventStore.AppendToStreamAsync(
        InterfaceConst.PSPrime,
        StreamState.Any,
        new[] { vnt }
      );
      await Task.Delay(TimeSpan.FromMilliseconds(100));
      // Assert
      var item = await grain.GetAsync();
      Assert.True(item.IsValid);
      Assert.NotEqual(number, item.Value);
    }

    public async ValueTask DisposeAsync()
    {
      await _cluster.DisposeAsync();
      await _eventStore.DisposeAsync();
    }
  }
}
