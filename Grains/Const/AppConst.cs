using System.Reflection;

namespace Grains
{
  public static class AppConst
  {
    public static Assembly Assembly => typeof(HelloGrain).Assembly;

    public const string Storage = "File";
  }
}
