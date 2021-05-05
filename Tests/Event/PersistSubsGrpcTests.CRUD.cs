using EventStore.Client;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Tests.Event
{
  public partial class PersistSubsGrpcTests
  {
    [Fact]
    public async Task Create()
    {
      // Act
      await _persist.CreateAsync(TestNoStream, TestGroup, Settings);
      // Assert
      try
      { // can be created only once
        await _persist.CreateAsync(TestNoStream, TestGroup, Settings);
      }
      catch(Exception ex)
      {
        Assert.IsType<InvalidOperationException>(ex);
      }
      // Clean
      await _persist.DeleteAsync(TestNoStream, TestGroup);
    }

    [Fact]
    public async Task Delete()
    {
      // Arrange
      await _persist.CreateAsync(TestNoStream, TestGroup, Settings);
      // Act
      await _persist.DeleteAsync(TestNoStream, TestGroup);
      // Assert
      try
      { // can be deleted only once
        await _persist.DeleteAsync(TestNoStream, TestGroup);
      }
      catch (Exception ex)
      {
        Assert.IsType<PersistentSubscriptionNotFoundException>(ex);
      }
    }

    [Fact]
    public async Task Subscribe()
    {
      // Arrange
      await _persist.CreateAsync(TestNoStream, TestGroup, Settings);
      // Act
      await _persist.SubscribeAsync(TestNoStream, TestGroup, null);
      // Clean
      await _persist.DeleteAsync(TestNoStream, TestGroup);
    }

    [Fact]
    public async Task Update()
    {
      // Arrange
      await _persist.CreateAsync(TestNoStream, TestGroup, Settings);
      // Act
      await _persist.UpdateAsync(TestNoStream, TestGroup, Settings);
      // Clean
      await _persist.DeleteAsync(TestNoStream, TestGroup);
      // Assert
      try
      { // must exist to update
        await _persist.UpdateAsync(TestNoStream, TestGroup, Settings);
      }
      catch (Exception ex)
      {
        Assert.IsType<InvalidOperationException>(ex);
      }
    }
  }
}
