using EventStore;
using EventStore.Client;
using Interfaces;
using Interfaces.Model;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Providers;
using Orleans.Streams.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Grains
{
  [ImplicitStreamSubscription(InterfaceConst.PSPrime)]
  [StorageProvider(ProviderName = GrainConst.Storage)]
  public class PrimeGrain : Grain<PrimeState>, IPrime
  {
    private readonly ILogger _logger;
    private readonly EventStoreClient _client;

    public PrimeGrain(ILogger<PrimeGrain> logger, EventStoreClient client)
    {
      _logger = logger;

      _client = client;
      observer = new Observer<int>(logger, (int number) => IsPrime(number));
    }

    private Orleans.Streams.IAsyncStream<int> Stream => GetStreamProvider(InterfaceConst.SMSProvider)
        .GetStream<int>(this.GetPrimaryKey(), InterfaceConst.PSPrime);

    public async override Task OnActivateAsync()
    {
      State.Initialize(WriteStateAsync);

      // initialize the value
      _value = VersionedValue<int>.None.NextVersion(0);
      // initialize the polling wait handle
      _wait = new TaskCompletionSource<VersionedValue<int>>();

      _es_pool = RegisterTimer(_ => ES_Initialize(), null, TimeSpan.FromSeconds(5), TimeSpan.FromMilliseconds(1));
      await base.OnActivateAsync();
    }

    public Task Consume()
    {
      _logger.LogInformation("Starting to consume...");
      return Task.CompletedTask;
    }

    private readonly Observer<int> observer;
    public async Task OnSubscribed(IStreamSubscriptionHandleFactory handleFactory)
    {
      var handle = handleFactory.Create<int>();
      await handle.ResumeAsync(observer);
    }

    private IDisposable _es_pool;
    private long _position = 0;
    private async Task ES_Initialize()
    {
      var ec_stream = _client.ReadStreamAsync(
        Direction.Forwards,
        InterfaceConst.PSPrime,
        StreamPosition.FromInt64(_position),
        maxCount: 100
      );

      if (await ec_stream.ReadState == ReadState.StreamNotFound)
      {
        await _client.SubscribeToStreamAsync(InterfaceConst.PSPrime, SubscribeReturn);
        _es_pool.Dispose();
        return;
      }
      
      var tasks = new List<Task>();
      foreach (var vnt in ec_stream.ToEnumerable().AsParallel())
        tasks.Add(SubscribeReturn(null, vnt, CancellationToken.None));
      await ec_stream.DisposeAsync();
      _position += tasks.Count;
      Task.WaitAll(tasks.ToArray());

      if(tasks.Any() == false)
      {
        await _client.SubscribeToStreamAsync(InterfaceConst.PSPrime, StreamPosition.FromInt64(_position), SubscribeReturn);
        _es_pool.Dispose();
      }
    }
    private async Task SubscribeReturn(EventStore.Client.StreamSubscription ss, ResolvedEvent vnt, CancellationToken ct)
    {
      var result = int.Parse(Encoding.UTF8.GetString(vnt.Event.Data.Span));
      await Stream.OnNextAsync(result);
    }

    public async Task<bool> IsPrime(int number)
    {
      if (State.HasPrime(number))
      {
        _logger.LogInformation($"{number} is prime and is on the list");
        await RC_UpdateAsync(number);
        return true;
      }

      var _primesqrt = State.GetPrimes(number);
      foreach (var prime in _primesqrt.AsParallel())
        if (number.IsDivisible(prime))
        {
          _logger.LogInformation($"{number} is not prime, divisible by {prime}");
          return false;
        }

      _logger.LogInformation($"{number} is prime and will be added on the list");

      State.AddPrime(number, WriteStateAsync);
      await RC_UpdateAsync(number);
      await ES_UpdateAsnc(number);

      return true;
    }
    private async Task ES_UpdateAsnc(int number)
    {
      var vnt = new EventData(
        number.ToUuid(),
        number.GetType().ToString(),
        JsonSerializer.SerializeToUtf8Bytes(number)
      );

      await _client.AppendToStreamAsync(
        InterfaceConst.PSPrimeOnly,
        StreamState.Any,
        new[] { vnt }
      );
    }

    #region RC
    private VersionedValue<int> _value;
    private TaskCompletionSource<VersionedValue<int>> _wait;

    public Task<VersionedValue<int>> GetAsync() => Task.FromResult(_value);

    public Task RC_UpdateAsync(int value)
    {
      var key = this.GetPrimaryKeyLong();
      // update the state
      _value = _value.NextVersion(value);
      _logger.LogInformation($"{nameof(PrimeGrain)} {key} updated value to {_value.Value} with version {_value.Version}");

      // fulfill waiting promises
      _wait.SetResult(_value);
      _wait = new TaskCompletionSource<VersionedValue<int>>();

      return Task.CompletedTask;
    }

    public Task<VersionedValue<int>> LongPollAsync(VersionToken knownVersion) =>
            knownVersion == _value.Version
            ? _wait.Task.WithDefaultOnTimeout(TimeSpan.Zero, VersionedValue<int>.None)
            : Task.FromResult(_value);
    #endregion
  }

  public class PrimeState : IDisposable
  {
    public void Initialize(Func<Task> act)
    {
      if (Primes == null)
      {
        Primes = new List<int>() { 2, 3, 5, 7, 11, 13, 17, 19, 23, 29, 31, 37, 41, 43, 47, 53, 59, 61, 67, 71, 73, 79, 83, 89, 97 };
        act.Invoke();
      }
    }

    public IList<int> Primes { get; set; }

    public bool HasPrime(int value) => Primes.Contains(value);
    public IEnumerable<int> GetPrimes(int value) => Primes.Where(x => x <= Math.Sqrt(value));

    private Func<Task> Act = null;
    public async Task AddPrime(int value, Func<Task> act)
    {
      Primes.Add(value);

      if (Act == null)
      {
        Act = act;
        await Task.Delay(1000);

        Primes = Primes.OrderBy(x => x).ToList();

        await Act.Invoke();
        Act = null;
      }
    }

    public void Dispose()
    {
      if (Act != null)
        Act.Invoke();
    }
  }
}
