using System;
using FluentAssertions;
using Newtonsoft.Json;
using Vogen.SerializationTests.Types;
using Xunit;

namespace Vogen.SerializationTests.JsonNetTests;

public class StrictSerializationTests
{
    readonly JsonSerializerSettings _settings = new()
    {
        Converters = { new JsonNetConverterFactory() }
    };

    [Fact]
    public void Serialize_int_struct()
    {
        var vo1 = MyIntStruct.From(123);
        
        string s = JsonConvert.SerializeObject(vo1, _options);

        var vo2 = JsonConvert.DeserializeObject<MyIntStruct>(s, _options);
        
        vo1.Should().Be(vo2);

        (vo1 == vo2).Should().BeTrue();
    }

    [Fact]
    public void Reading_multiple_times_gives_same_result()
    {
        var vo1 = MyIntStruct.From(123);
        
        string s = JsonConvert.SerializeObject(vo1, _options);

        var vo2 = JsonConvert.DeserializeObject<MyIntStruct>(s, _options);
        var vo3 = JsonConvert.DeserializeObject<MyIntStruct>(s, _options);
        var vo4 = JsonConvert.DeserializeObject<MyIntStruct>(s, _options);
        
        vo1.Should().Be(vo2);
        vo1.Should().Be(vo3);
        vo1.Should().Be(vo4);

        (vo1 == vo2).Should().BeTrue();
    }

    [Fact]
    public void Serialize_int_class()
    {
        var vo1 = Types.MyIntClass.From(123);
        
        string s = JsonConvert.SerializeObject(vo1, _options);

        var vo2 = JsonConvert.DeserializeObject<Types.MyIntClass>(s, _options);
        
        vo1.Should().Be(vo2);

        (vo1 == vo2).Should().BeTrue();
    }
    
    [Fact]
    public void Serialize_different_types()
    {
        var ic1 = Types.MyIntClass.From(123);
        string s = JsonConvert.SerializeObject(ic1, _options);
        var ic2 = JsonConvert.DeserializeObject<Types.MyIntClass>(s, _options);
        ic1.Should().Be(ic2);
        (ic1 == ic2).Should().BeTrue();

        var age1 = Age.From(123);
        string s2 = JsonConvert.SerializeObject(age1, _options);
        var age2 = JsonConvert.DeserializeObject<Age>(s2, _options);
        age1.Should().Be(age2);
        (age1 == age2).Should().BeTrue();

        var dave1 = Dave.From("David Beckham");
        string s3 = JsonConvert.SerializeObject(dave1, _options);
        var dave2 = JsonConvert.DeserializeObject<Dave>(s3, _options);
        dave1.Should().Be(dave2);
        (dave1 == dave2).Should().BeTrue();

        var eightiesDate1 = EightiesDate.From(new DateTime(1985, 12, 13));
        string s4 = JsonConvert.SerializeObject(eightiesDate1, _options);
        var eightiesDate2 = JsonConvert.DeserializeObject<EightiesDate>(s4, _options);
        eightiesDate1.Should().Be(eightiesDate2);
        (eightiesDate1 == eightiesDate2).Should().BeTrue();

        var name1 = Name.From("aaa");
        string s5 = JsonConvert.SerializeObject(name1, _options);
        var name2 = JsonConvert.DeserializeObject<Name>(s5, _options);
        name1.Should().Be(name2);
        (name1 == name2).Should().BeTrue();

        var number1 = Number.From(42);
        string s6 = JsonConvert.SerializeObject(number1, _options);
        var number2 = JsonConvert.DeserializeObject<Number>(s6, _options);
        number1.Should().Be(number2);
        (number1 == number2).Should().BeTrue();

        var score1 = Score.From(10_980);
        string s7 = JsonConvert.SerializeObject(score1, _options);
        var score2 = JsonConvert.DeserializeObject<Score>(s7, _options);
        score1.Should().Be(score2);
        (score1 == score2).Should().BeTrue();

        var default1 = MyIntStructWithADefaultOf22.Default;
        string s8 = JsonConvert.SerializeObject(default1, _options);
        var default2 = JsonConvert.DeserializeObject<MyIntStructWithADefaultOf22>(s8, _options);
        default1.Should().Be(default2);
        (default1 == default2).Should().BeTrue();

        var unspecified1 = MyIntWithTwoInstanceOfInvalidAndUnspecified.Unspecified;
        string s9 = JsonConvert.SerializeObject(unspecified1, _options);
        
        Action act = () => JsonConvert.DeserializeObject<MyIntWithTwoInstanceOfInvalidAndUnspecified>(s9, _options);
        act.Should().Throw<ValueObjectValidationException>().WithMessage("must be greater than zero");
    }

    [Fact]
    public void Serialize_invalid_mixes()
    {
        var vo1 = Types.MyIntClass.From(123);
        
        string s = JsonConvert.SerializeObject(vo1, _options);

        var vo2 = JsonConvert.DeserializeObject<MyIntStruct>(s, _options);

        vo2.Value.Should().Be(123);
        
        vo1.Should().NotBe(vo2);

        // compilation error - so that's covered.
        //(vo1 == vo2).Should().BeTrue();
    }

    class MyThing
    {
        public Types.MyIntClass TheClass { get; set; } = Types.MyIntClass.From(666);
        public MyIntStruct TheStruct { get; set; } = MyIntStruct.From(666);
    }

    [Fact]
    public void Serialize_composite()
    {
        var vo1 = new MyThing {TheClass = Types.MyIntClass.From(123), TheStruct = MyIntStruct.From(333)};
        
        string s = JsonConvert.SerializeObject(vo1, _options);

        var vo2 = JsonConvert.DeserializeObject<MyThing>(s, _options)!;

        vo2.TheClass.Value.Should().Be(123);
        vo2.TheStruct.Value.Should().Be(333);
    }

    [Fact]
    public void Deserialize_invalid_int_class()
    {
        string s = "-1";

        Func<MyIntClassWithValidation> act = () => JsonConvert.DeserializeObject<MyIntClassWithValidation>(s, _options)!;

        act.Should().ThrowExactly<ValueObjectValidationException>()
            .WithMessage("MyIntClassWithValidation must be created with a value greater than zero");
    }

    [Fact]
    public void Deserialize_invalid_int_struct()
    {
        string s = "-1";

        Func<MyIntStructWithValidation> act = () => JsonConvert.DeserializeObject<MyIntStructWithValidation>(s, _options);

        act.Should().ThrowExactly<ValueObjectValidationException>()
            .WithMessage("MyIntStructWithValidation must be created with a value greater than zero");
    }
}