namespace Grains
{
  public static class IntExtension
  {
    public static bool IsDivisible(this int number, int divisor)
    {
      return (decimal)number % divisor == 0;
    }
  }

  public static class LongExtension
  {
    public static bool IsDivisible(this long number, long divisor)
    {
      return (decimal)number % divisor == 0;
    }
  }

  public static class UIntExtension
  {
    public static bool IsDivisible(this uint number, uint divisor)
    {
      return (decimal)number % divisor == 0;
    }
  }

  public static class ULongExtension
  {
    public static bool IsDivisible(this ulong number, ulong divisor)
    {
      return (decimal)number % divisor == 0;
    }
  }
}
