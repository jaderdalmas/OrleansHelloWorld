using Orleans.Streams.Core;
using System.Threading.Tasks;

namespace GrainInterfaces
{
  public interface IPrime : Orleans.IGrainWithIntegerKey, IStreamSubscriptionObserver, IConsumer
  {
    Task<bool> IsPrime(int number);
  }
}
