using EventStore.Client;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EventStore
{
  public static class EventStoreClientExtension
  {
    /// <summary>
    /// List streams
    /// </summary>
    public static async Task<IEnumerable<string>> GetStreams(this EventStoreClient client)
    {
      IList<string> list = new List<string>();
      await foreach (var @event in client.ReadAllAsync(Direction.Forwards, Position.Start))
        if (!list.Contains(@event.OriginalStreamId))
          list.Add(@event.OriginalStreamId);

      return list;
    }

    /// <summary>
    /// List streams that starts with '{name}*'
    /// </summary>
    /// <param name="startWith">'{streamName}*'</param>
    public static async Task<IEnumerable<string>> GetStreams(this EventStoreClient client, string startWith)
    {
      var list = await GetStreams(client);
      var streamName = startWith.Replace("*", "");
      return list.Where(x => x.StartsWith(streamName));
    }
  }
}
