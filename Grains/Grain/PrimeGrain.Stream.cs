using Interfaces;
using Interfaces.Model;
using Orleans;
using Orleans.Streams.Core;
using System.Threading.Tasks;

namespace Grains
{
  [ImplicitStreamSubscription(InterfaceConst.PSPrime)]
  public partial class PrimeGrain
  {
    /// <summary>
    /// Stream Observer
    /// </summary>
    private readonly Observer<int> observer;

    /// <summary>
    /// Initialize Stream Subscription
    /// </summary>
    /// <param name="handleFactory">stream factory</param>
    public Task OnSubscribed(IStreamSubscriptionHandleFactory handleFactory)
    {
      var handle = handleFactory.Create<int>();
      return handle.ResumeAsync(observer);
    }
  }
}
