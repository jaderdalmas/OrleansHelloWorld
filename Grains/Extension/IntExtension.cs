namespace Grains
{
  public static class IntExtension
  {
    public static bool IsDivisible(this int number, int divisor)
    {
      return (decimal)number % divisor == 0;
    }
  }
}
