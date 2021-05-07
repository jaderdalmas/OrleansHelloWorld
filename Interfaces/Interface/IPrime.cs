using Interfaces.Model;
using Orleans;
using Orleans.Streams.Core;
using System.Threading.Tasks;

namespace Interfaces
{
  public interface IPrime : IGrainWithIntegerKey, IStreamSubscriptionObserver, IConsumer
  {
    Task<bool> IsPrime(int number);

    Task<VersionedValue<int>> LongPollAsync(VersionToken knownVersion);
  }
}
