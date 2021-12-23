using FluentAssertions;
using Vogen.Serialization.SystemTextJson;
using Xunit;

namespace Vogen.Serialization.UnitTests;

public class VogenSerializationOptionsTests
{
    [Fact]
    public void Defaults()
    {
        VogenSerializationOptions sut = new();

        sut.IsStrict.Should().BeFalse();
    }
}