using Orleans;
using Orleans.Streams.Core;
using System.Threading.Tasks;

namespace Interfaces
{
  public interface IHello : IGrainWithIntegerKey, IStreamSubscriptionObserver
  {
    Task<string> SayHello(string greeting);
  }
}
