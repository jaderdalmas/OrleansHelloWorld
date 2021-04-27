using Orleans.Streams.Core;
using System.Threading.Tasks;

namespace GrainInterfaces
{
  public interface IPrime : Orleans.IGrainWithIntegerKey, IStreamSubscriptionObserver
  {
    Task<bool> IsPrime(int number);
  }
}
