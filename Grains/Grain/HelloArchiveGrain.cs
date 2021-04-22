using GrainInterfaces;
using Orleans;
using Orleans.Runtime;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Grains
{
  public class HelloArchiveGrain : Grain, IHelloArchive
  {
    private readonly IPersistentState<GreetingArchive> _archive;

    public HelloArchiveGrain([PersistentState("archive", AppConst.Storage)] IPersistentState<GreetingArchive> archive)
    {
      _archive = archive;

      try { _archive.ClearStateAsync().Wait(); } catch { }
    }

    public async Task<string> SayHello(string greeting)
    {
      await _archive.State.AddGreeting(greeting, _archive.WriteStateAsync);

      return $"You said: '{greeting}', I say: Hello!";
    }

    public Task<IEnumerable<string>> GetGreetings() => Task.FromResult<IEnumerable<string>>(_archive.State.Greetings);
  }

  public class GreetingArchive
  {
    public List<string> Greetings { get; } = new List<string>();

    public async Task AddGreeting(string value, Func<Task> act)
    {
      Greetings.Add(value);

      await act.Invoke();
    }
  }
}
