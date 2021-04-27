using GrainInterfaces;
using Orleans.TestingHost;
using System.Threading.Tasks;
using Xunit;

namespace Tests
{
  [Collection(nameof(ClusterCollection))]
  public class PrimeGrainTests
  {
    private readonly TestCluster _cluster;

    public PrimeGrainTests(ClusterFixture fixture)
    {
      _cluster = fixture.Cluster;
    }

    [Theory]
    [InlineData(101)]
    [InlineData(103)]
    [InlineData(107)]
    [InlineData(109)]
    [InlineData(113)]
    public async Task IsPrime(int number)
    {
      var grain = _cluster.GrainFactory.GetGrain<IPrime>(0);
      var isprime = await grain.IsPrime(number);

      Assert.True(isprime);
    }

    [Theory]
    [InlineData(105)]
    [InlineData(111)]
    [InlineData(115)]
    [InlineData(117)]
    public async Task IsNotPrime(int number)
    {
      var grain = _cluster.GrainFactory.GetGrain<IPrime>(0);
      var isprime = await grain.IsPrime(number);

      Assert.False(isprime);
    }
  }
}
