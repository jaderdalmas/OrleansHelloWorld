using EventStore.Client;
using Interfaces;
using Orleans.TestingHost;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Tests.Grain
{
  [Collection(nameof(ClusterCollection))]
  public class PrimeOnlyGrainTests
  {
    private readonly TestCluster _cluster;
    private readonly EventStoreClient _eventStore;

    public PrimeOnlyGrainTests(ClusterFixture fixture)
    {
      _cluster = fixture.Cluster;
      _eventStore = fixture.EventStore;
    }

    [Fact]
    public async Task Get_Empty()
    {
      // Arrange
      var value = 0;
      // Act
      var only = _cluster.GrainFactory.GetGrain<IPrimeOnly>(1);
      var result = await only.GetAsync();
      // Assert
      Assert.Equal(value, result);
    }

    [Theory]
    [InlineData(101)]
    [InlineData(103)]
    [InlineData(107)]
    [InlineData(109)]
    [InlineData(113)]
    public async Task Get_Call(int number)
    {
      // Arrange
      var prime = _cluster.GrainFactory.GetGrain<IPrime>(2);
      var only = _cluster.GrainFactory.GetGrain<IPrimeOnly>(2);
      await prime.Consume();
      await only.Consume();
      // Act
      await prime.IsPrime(number);
      await Task.Delay(TimeSpan.FromSeconds(1));

      //var rprime = await prime.RXService().GetAsync(); // LongPollAsync(VersionToken.None);
      var rpnly = await only.GetAsync();
      // Assert
      //Assert.Equal(number, rprime.Value);
      Assert.Equal(number, rpnly);
    }
  }
}
