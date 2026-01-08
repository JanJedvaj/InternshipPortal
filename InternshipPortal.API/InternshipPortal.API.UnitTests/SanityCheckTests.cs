using FluentAssertions;
using Moq;

namespace InternshipPortal.API.UnitTests.Sanity;

public class SanityCheckTests
{
    [Fact]
    public void Moq_And_FluentAssertions_Work()
    {
        var mock = new Mock<IDisposable>();
        mock.Object.Should().NotBeNull();
    }
}
