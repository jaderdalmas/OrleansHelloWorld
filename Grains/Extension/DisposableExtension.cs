using System;

namespace Grains
{
  public static class DisposableExtension
  {
    public static IDisposable Clean(this IDisposable item)
    {
      item.Dispose();
      return null;
    }
  }
}
