using Orleans;
using Orleans.Streams.Core;
using System.Threading.Tasks;

namespace GrainInterfaces
{
  public interface IPrime : IGrainWithIntegerKey, IStreamSubscriptionObserver, IReactiveCacheFrom<int>, IConsumer
  {
    Task<bool> IsPrime(int number);
  }
}
