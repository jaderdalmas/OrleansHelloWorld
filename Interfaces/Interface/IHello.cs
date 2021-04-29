using Orleans;
using Orleans.Streams.Core;
using System.Threading.Tasks;

namespace Interfaces
{
  public interface IHello : IGrainWithIntegerKey, IStreamSubscriptionObserver, IConsumer
  {
    Task<string> SayHello(string greeting);
  }
}
