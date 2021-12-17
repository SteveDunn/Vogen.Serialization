using System;
using System.Text.Json;
using FluentAssertions;
using Vogen.Serialization.SystemTextJson;
using Vogen.SerializationTests.Types;
using Xunit;

namespace Vogen.SerializationTests.SystemTextJsonTests;

public class StrictSerializationTests
{
    private readonly JsonSerializerOptions _options = new()
    {
        Converters =
        {
            new VogenConverterFactory(
                new VogenSerializationOptions
                {
                    IsStrict = true
                })
        }
    };

    [Fact]
    public void Serialize_int_struct()
    {
        var vo1 = MyIntStruct.From(123);
        
        string s = JsonSerializer.Serialize(vo1, _options);

        var vo2 = JsonSerializer.Deserialize<MyIntStruct>(s, _options);
        
        vo1.Should().Be(vo2);

        (vo1 == vo2).Should().BeTrue();
    }

    [Fact]
    public void Reading_multiple_times_gives_same_result()
    {
        var vo1 = MyIntStruct.From(123);
        
        string s = JsonSerializer.Serialize(vo1, _options);

        var vo2 = JsonSerializer.Deserialize<MyIntStruct>(s, _options);
        var vo3 = JsonSerializer.Deserialize<MyIntStruct>(s, _options);
        var vo4 = JsonSerializer.Deserialize<MyIntStruct>(s, _options);
        
        vo1.Should().Be(vo2);
        vo1.Should().Be(vo3);
        vo1.Should().Be(vo4);

        (vo1 == vo2).Should().BeTrue();
    }

    [Fact]
    public void Serialize_int_class()
    {
        var vo1 = MyIntClass.From(123);
        
        string s = JsonSerializer.Serialize(vo1, _options);

        var vo2 = JsonSerializer.Deserialize<MyIntClass>(s, _options);
        
        vo1.Should().Be(vo2);

        (vo1 == vo2).Should().BeTrue();
    }
    
    [Fact]
    public void Serialize_different_types()
    {
        var ic1 = MyIntClass.From(123);
        string s = JsonSerializer.Serialize(ic1, _options);
        var ic2 = JsonSerializer.Deserialize<MyIntClass>(s, _options);
        ic1.Should().Be(ic2);
        (ic1 == ic2).Should().BeTrue();

        var age1 = Age.From(123);
        string s2 = JsonSerializer.Serialize(age1, _options);
        var age2 = JsonSerializer.Deserialize<Age>(s2, _options);
        age1.Should().Be(age2);
        (age1 == age2).Should().BeTrue();

        var dave1 = Dave.From("David Beckham");
        string s3 = JsonSerializer.Serialize(dave1, _options);
        var dave2 = JsonSerializer.Deserialize<Dave>(s3, _options);
        dave1.Should().Be(dave2);
        (dave1 == dave2).Should().BeTrue();

        var eightiesDate1 = EightiesDate.From(new DateTime(1985, 12, 13));
        string s4 = JsonSerializer.Serialize(eightiesDate1, _options);
        var eightiesDate2 = JsonSerializer.Deserialize<EightiesDate>(s4, _options);
        eightiesDate1.Should().Be(eightiesDate2);
        (eightiesDate1 == eightiesDate2).Should().BeTrue();

        var name1 = Name.From("aaa");
        string s5 = JsonSerializer.Serialize(name1, _options);
        var name2 = JsonSerializer.Deserialize<Name>(s5, _options);
        name1.Should().Be(name2);
        (name1 == name2).Should().BeTrue();

        var number1 = Number.From(42);
        string s6 = JsonSerializer.Serialize(number1, _options);
        var number2 = JsonSerializer.Deserialize<Number>(s6, _options);
        number1.Should().Be(number2);
        (number1 == number2).Should().BeTrue();

        var score1 = Score.From(10_980);
        string s7 = JsonSerializer.Serialize(score1, _options);
        var score2 = JsonSerializer.Deserialize<Score>(s7, _options);
        score1.Should().Be(score2);
        (score1 == score2).Should().BeTrue();

        var default1 = MyIntStructWithADefaultOf22.Default;
        string s8 = JsonSerializer.Serialize(default1, _options);
        var default2 = JsonSerializer.Deserialize<MyIntStructWithADefaultOf22>(s8, _options);
        default1.Should().Be(default2);
        (default1 == default2).Should().BeTrue();

        var unspecified1 = MyIntWithTwoInstanceOfInvalidAndUnspecified.Unspecified;
        string s9 = JsonSerializer.Serialize(unspecified1, _options);
        
        Action act = () => JsonSerializer.Deserialize<MyIntWithTwoInstanceOfInvalidAndUnspecified>(s9, _options);
        act.Should().Throw<ValueObjectValidationException>().WithMessage("must be greater than zero");
    }

    [Fact]
    public void Serialize_invalid_mixes()
    {
        var vo1 = MyIntClass.From(123);
        
        string s = JsonSerializer.Serialize(vo1, _options);

        var vo2 = JsonSerializer.Deserialize<MyIntStruct>(s, _options);

        vo2.Value.Should().Be(123);
        
        vo1.Should().NotBe(vo2);

        // compilation error - so that's covered.
        //(vo1 == vo2).Should().BeTrue();
    }

    class MyThing
    {
        public MyIntClass TheClass { get; set; } = MyIntClass.From(666);
        public MyIntStruct TheStruct { get; set; } = MyIntStruct.From(666);
    }

    [Fact]
    public void Serialize_composite()
    {
        var vo1 = new MyThing {TheClass = MyIntClass.From(123), TheStruct = MyIntStruct.From(333)};
        
        string s = JsonSerializer.Serialize(vo1, _options);

        var vo2 = JsonSerializer.Deserialize<MyThing>(s, _options)!;

        vo2.TheClass.Value.Should().Be(123);
        vo2.TheStruct.Value.Should().Be(333);
    }

    [Fact]
    public void Deserialize_invalid_int_class()
    {
        string s = "-1";

        Func<MyIntClassWithValidation> act = () => JsonSerializer.Deserialize<MyIntClassWithValidation>(s, _options)!;

        act.Should().ThrowExactly<ValueObjectValidationException>()
            .WithMessage("MyIntClassWithValidation must be created with a value greater than zero");
    }

    [Fact]
    public void Deserialize_invalid_int_struct()
    {
        string s = "-1";

        Func<MyIntStructWithValidation> act = () => JsonSerializer.Deserialize<MyIntStructWithValidation>(s, _options);

        act.Should().ThrowExactly<ValueObjectValidationException>()
            .WithMessage("MyIntStructWithValidation must be created with a value greater than zero");
    }
}