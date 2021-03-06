using Interfaces;
using Interfaces.Model;
using Microsoft.Extensions.Logging;
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
    private Observer<int> observer;

    /// <summary>
    /// Constructor
    /// </summary>
    public void PrimeGrain_Stream(ILoggerFactory factory)
    {
      observer = new Observer<int>(factory, (int number) => IsPrime(number));
    }

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
