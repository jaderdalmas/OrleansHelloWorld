using EventStore;
using EventStore.Client;
using Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace Grains
{
  public partial class PrimeGrain
  {
    /// <summary>
    /// Event Store Persistent Subscriptions Client
    /// </summary>
    private EventStorePersistentSubscriptionsClient _persist;

    /// <summary>
    /// Constructor
    /// </summary>
    public void PrimeGrain_Persist(EventStorePersistentSubscriptionsClient persist)
    {
      _persist = persist;
    }

    /// <summary>
    /// On Activate, Initialize persist subscription
    /// </summary>
    /// <returns></returns>
    private async Task OnActivatePersistAsync()
    {
      var settings = new PersistentSubscriptionSettings();

      try
      {
        await _persist.CreateAsync(InterfaceConst.PSPrime, InterfaceConst.GroupPrime, settings);
      }
      catch { }

      await _persist.SubscribeAsync(InterfaceConst.PSPrime, InterfaceConst.GroupPrime, SubscribeReturn);
    }

    /// <summary>
    /// Event subscription return to execute
    /// </summary>
    /// <param name="ps">Persistent Subscription</param>
    /// <param name="vnt">Resolved Event</param>
    /// <param name="n">Event Number</param>
    /// <param name="ct">Cancellation Token</param>
    /// <returns>true if prime</returns>
    private Task<bool> SubscribeReturn(PersistentSubscription ps, ResolvedEvent vnt, int? n, CancellationToken ct)
    {
      var number = int.Parse(vnt.ToJson());
      return IsPrime(number);

      //await ps.Ack(new[] { vnt.OriginalEvent.EventId });
    }
  }
}
