using Orleans;
using Orleans.Streams.Core;
using System.Threading.Tasks;

namespace GrainInterfaces
{
  public interface IHello : IGrainWithIntegerKey, IStreamSubscriptionObserver
  {
    Task<string> SayHello(string greeting);
  }
}
