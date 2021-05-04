using EventStore;
using EventStore.Client;
using Interfaces;
using System;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Grains
{
  public partial class PrimeGrain
  {
    /// <summary>
    /// Event Store Client
    /// </summary>
    private readonly EventStoreClient _client;

    /// <summary>
    /// On Activate, Initialize ES reading and subscription
    /// </summary>
    /// <returns></returns>
    private async Task OnActivateESAsync()
    {
      _es_pool = RegisterTimer(_ => ES_Initialize(), null, TimeSpan.Zero, TimeSpan.FromMilliseconds(1));
      while (_es_pool != null) await Task.Delay(TimeSpan.FromMilliseconds(500));
    }

    /// <summary>
    /// Event Store Pool
    /// </summary>
    private IDisposable _es_pool;
    /// <summary>
    /// Event Store Initialize (read events and subscribe at the end)
    /// </summary>
    public async Task ES_Initialize()
    {
      var ec_stream = _client.ReadStreamAsync(
        Direction.Forwards,
        InterfaceConst.PSPrime,
        StreamPosition.FromInt64(_position),
        maxCount: 100
      );

      if (await ec_stream.ReadState == ReadState.StreamNotFound)
      {
        _es_pool = _es_pool.Clean();
        await _client.SubscribeToStreamAsync(InterfaceConst.PSPrime, SubscribeReturn);
        return;
      }

      var _init = _position;
      foreach (var vnt in ec_stream.ToEnumerable().AsParallel())
        await SubscribeReturn(null, vnt, CancellationToken.None);
      await ec_stream.DisposeAsync();

      if (_init == _position)
      {
        _es_pool = _es_pool.Clean();
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
      var result = int.Parse(Encoding.UTF8.GetString(vnt.Event.Data.Span));
      return IsPrime(result);
    }

    /// <summary>
    /// Add ES prime only number
    /// </summary>
    /// <param name="number">number</param>
    private Task ES_UpdateAsync(int number)
    {
      var vnt = new EventData(
        number.ToUuid(),
        number.GetType().ToString(),
        JsonSerializer.SerializeToUtf8Bytes(number)
      );

      return _client.AppendToStreamAsync(
        InterfaceConst.PSPrimeOnly,
        StreamState.Any,
        new[] { vnt }
      );
    }

    /// <summary>
    /// Dispose Event Store
    /// </summary>
    public ValueTask DisposeESAsync()
    {
      _es_pool = _es_pool.Clean();
      return new ValueTask();
    }
  }
}
