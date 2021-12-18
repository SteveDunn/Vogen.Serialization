using System;
using FluentAssertions;
using Vogen.Serialization.SystemTextJson;
using Vogen.Serialization.UnitTests.Types;
using Xunit;

namespace Vogen.Serialization.UnitTests.SystemTextJsonTests;

public class NotAValueObject { }


public class ValueObjectConverterFactoryTests
{
    public class ValueObjectAttribute : Attribute { }

    [ValueObject]
    public class FakeVo
    {
    }

    [Fact]
    public void Cannot_not_convert_a_type_without_the_vo_attribute()
    {
        var sut = new VogenConverterFactory();
        sut.CanConvert(typeof(NotAValueObject)).Should().BeFalse();
    }

    [Fact]
    public void Cannot_not_convert_a_type_with_a_different_vo_attribute()
    {
        var sut = new VogenConverterFactory();
        sut.CanConvert(typeof(FakeVo)).Should().BeFalse();
    }

    [Fact]
    public void Can_convert_a_type_with_the_vo_attribute()
    {
        var sut = new VogenConverterFactory();
        sut.CanConvert(typeof(MyIntStruct)).Should().BeTrue();
        sut.CanConvert(typeof(MyIntClass)).Should().BeTrue();
    }
}