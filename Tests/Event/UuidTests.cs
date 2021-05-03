using EventStore;
using System;
using Xunit;

namespace Tests.Event
{
  public partial class UuidTests
  {
    [Theory]
    [InlineData(33331)]
    [InlineData(long.MinValue)]
    [InlineData(int.MinValue)]
    public void Conversion(long number)
    {
      // Arrange
      var text = number.ToString(Guid.Empty.ToString());
      if (text.StartsWith('-'))
        text = text.Remove(0, 1);
      //Act
      var guid = Guid.Parse(text);
      var uuid = number.ToUuid();
      // Assert
      Assert.Equal(text, guid.ToString());
      Assert.Equal(text, uuid.ToString());
    }

    [Theory]
    [InlineData(uint.MinValue)]
    [InlineData(uint.MaxValue)]
    [InlineData(ulong.MaxValue)]
    public void UConversion(ulong number)
    {
      // Arrange
      var text = number.ToString(Guid.Empty.ToString());
      if (text.StartsWith('-'))
        text = text.Remove(0, 1);
      //Act
      var guid = Guid.Parse(text);
      var uuid = number.ToUuid();
      // Assert
      Assert.Equal(text, guid.ToString());
      Assert.Equal(text, uuid.ToString());
    }
  }
}
