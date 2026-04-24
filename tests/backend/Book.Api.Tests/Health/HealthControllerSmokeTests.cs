namespace Book.Api.Tests.Health;

public sealed class HealthControllerSmokeTests
{
    [Fact]
    public void ProgramType_ShouldBeAvailable_ForIntegrationTests()
    {
        Assert.NotNull(typeof(Program));
    }
}
