using EventStore;
using EventStore.Client;
using Interfaces;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Grains
{
  public partial class PrimeGrain
  {
    /// <summary>
    /// Event Store Client
    /// </summary>
    private EventStoreClient _client;

    /// <summary>
    /// Constructor
    /// </summary>
    public void PrimeGrain_ES(EventStoreClient client)
    {
      _client = client;
    }

    /// <summary>
    /// On Activate, Initialize ES reading and subscription
    /// </summary>
    /// <returns></returns>
    private async Task OnActivateESAsync()
    {
      _es_poll = RegisterTimer(_ => ES_Initialize(), null, TimeSpan.Zero, TimeSpan.FromMilliseconds(1));
      while (_es_poll != null) await Task.Delay(TimeSpan.FromMilliseconds(500));
    }

    /// <summary>
    /// Event Store Poll
    /// </summary>
    private IDisposable _es_poll;
    /// <summary>
    /// Event Store Initialize (read events and subscribe at the end)
    /// </summary>
    public async Task ES_Initialize()
    {
      var _init = _position;
      try
      {
        var ec_stream = _client.ReadStreamAsync(
          Direction.Forwards,
          InterfaceConst.PSPrime,
          StreamPosition.FromInt64(_position),
          maxCount: 100
        );

        if (await ec_stream.ReadState == ReadState.StreamNotFound)
        {
          _es_poll = _es_poll.Clean();
          await _client.SubscribeToStreamAsync(InterfaceConst.PSPrime, SubscribeReturn);
          return;
        }

        foreach (var vnt in ec_stream.ToEnumerable().AsParallel())
          await SubscribeReturn(null, vnt, CancellationToken.None);
        await ec_stream.DisposeAsync();
      }
      catch { }

      if (_init == _position)
      {
        _es_poll = _es_poll.Clean();
        await _client.SubscribeToStreamAsync(InterfaceConst.PSPrime, StreamPosition.FromInt64(_position), SubscribeReturn);
      }
    }

    /// <summary>
    /// Event reading position
    /// </summary>
    private long _position = 0;
    /// <summary>
    /// Event subscription return to execute
    /// </summary>
    /// <param name="ss">Stream Subscription</param>
    /// <param name="vnt">Resolved Event</param>
    /// <param name="ct">Cancellation Token</param>
    /// <returns>true if prime</returns>
    private Task<bool> SubscribeReturn(StreamSubscription ss, ResolvedEvent vnt, CancellationToken ct)
    {
      _position++;
      var number = int.Parse(vnt.ToJson());
      return IsPrime(number);
    }

    /// <summary>
    /// Dispose Event Store
    /// </summary>
    public ValueTask DisposeESAsync()
    {
      _es_poll = _es_poll.Clean();
      return new ValueTask();
    }
  }
}
