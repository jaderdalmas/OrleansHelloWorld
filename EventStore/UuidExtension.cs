using EventStore.Client;

namespace EventStore
{
  public static class UuidExtension
  {
    public static string Empty => Uuid.Empty.ToString();

    public static Uuid ToUuid(this uint number) => number.ToString(Empty).ToUuid();
    public static Uuid ToUuid(this ulong number) => number.ToString(Empty).ToUuid();

    public static Uuid ToUuid(this int number) => number.ToString(Empty).ToUuid();
    public static Uuid ToUuid(this long number) => number.ToString(Empty).ToUuid();

    public static Uuid ToUuid(this string text)
    {
      if (text.StartsWith('-'))
        text = text.Remove(0, 1);

      return Uuid.Parse(text);
    }
  }
}
