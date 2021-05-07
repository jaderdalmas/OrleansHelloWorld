using EventStore;
using EventStore.Client;
using Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Grains
{
  public class ESService<T> : IESService<T>
  {
    /// <summary>
    /// Logging Interface
    /// </summary>
    private readonly ILogger _logger;

    /// <summary>
    /// Event Store Client
    /// </summary>
    private EventStoreClient _client;

    /// <summary>
    /// Action for the ES Subs Return
    /// </summary>
    private Func<T, Task> _action;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="factory">logger factory</param>
    /// <param name="client">event store client</param>
    public ESService(ILoggerFactory factory, EventStoreClient client)
    {
      _logger = factory.CreateLogger<ESService<T>>();

      _client = client;
    }

    /// <summary>
    /// Initialize ES reading and subscription
    /// </summary>
    /// <param name="action">subscription action</param>
    /// <param name="stream">stream name</param>
    public async Task Consume(Func<T, Task> action, string stream)
    {
      _action = action;

      bool result;
      do
      {
        result = await Initialize(stream);
      } while (result);
    }

    /// <summary>
    /// Event Store Initialize (read events and subscribe at the end)
    /// </summary>
    private async Task<bool> Initialize(string stream)
    {
      var _init = _position;
      try
      {
        var ec_stream = _client.ReadStreamAsync(
          Direction.Forwards, stream,
          StreamPosition.FromInt64(_position),
          maxCount: 100
        );

        if (await ec_stream.ReadState == ReadState.StreamNotFound)
        {
          await _client.SubscribeToStreamAsync(stream, SubscribeReturn);
          return false;
        }

        foreach (var vnt in ec_stream.ToEnumerable().AsParallel())
          await SubscribeReturn(null, vnt, CancellationToken.None);
        await ec_stream.DisposeAsync();
      }
      catch { }

      if (_init == _position)
      {
        await _client.SubscribeToStreamAsync(stream, StreamPosition.FromInt64(_position), SubscribeReturn);
        return false;
      }

      return true;
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
    private Task SubscribeReturn(StreamSubscription ss, ResolvedEvent vnt, CancellationToken ct)
    {
      _position = vnt.OriginalEventNumber.ToInt64();

      try
      {
        var item = vnt.To<T>();
        return _action?.Invoke(item);
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "SubscribeReturn Error");
        return Task.CompletedTask;
      }
    }

    /// <summary>
    /// Dispose Event Store
    /// </summary>
    public ValueTask DisposeAsync()
    {
      _action = null;
      return new ValueTask();
    }
  }
}
