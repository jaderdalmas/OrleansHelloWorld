using EventStore.Client;
using System.Text;
using System.Text.Json;

namespace EventStore
{
  public static class ResolvedEventExtension
  {
    public static string ToJson(this ResolvedEvent vnt)
    {
      return Encoding.UTF8.GetString(vnt.Event.Data.Span);
    }

    public static T To<T>(this ResolvedEvent vnt)
    {
      return JsonSerializer.Deserialize<T>(vnt.ToJson());
    }
  }
}
