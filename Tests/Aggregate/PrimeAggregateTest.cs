using EventSourcing.Aggregate;
using EventSourcing.Event;
using EventStore.Client;
using Interfaces;
using Orleans.TestingHost;
using System.Threading.Tasks;
using Tests.Extension;
using Xunit;

namespace Tests.Aggregate
{
  [Collection(nameof(ClusterCollection))]
  public class PrimeAggregateTest
  {
    private readonly TestCluster _cluster;
    private readonly EventStoreClient _eventStore;

    public PrimeAggregateTest(ClusterFixture fixture)
    {
      _cluster = fixture.Cluster;
      _eventStore = fixture.EventStore;
    }

    [Theory]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(5)]
    [InlineData(7)]
    [InlineData(33331)]
    public async Task Apply_IsPrimeEvent(int number)
    {
      // Arrange
      IAggregate aggregate = _cluster.GetService<PrimeAggregate>();
      // Act
      var result = await aggregate.Apply(new IsPrimeEvent() { Number = number });
      // Assert
      Assert.True(result);
    }

    [Theory]
    [InlineData(9)]
    [InlineData(15)]
    [InlineData(21)]
    [InlineData(25)]
    [InlineData(27)]
    public async Task Apply_IsPrimeEvent_False(int number)
    {
      // Arrange
      IAggregate aggregate = _cluster.GetService<PrimeAggregate>();
      // Act
      var result = await aggregate.Apply(new IsPrimeEvent() { Number = number });
      // Assert
      Assert.False(result);
    }
  }
}
