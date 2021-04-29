using System.Reflection;

namespace Grains
{
  public static class GrainConst
  {
    public static Assembly Assembly => typeof(HelloGrain).Assembly;

    public const string Storage = "File";

    public const string PSStore = "PubSubStore";
  }
}
