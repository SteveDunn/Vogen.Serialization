using System;
using System.Collections.Generic;
using FluentAssertions;
using Newtonsoft.Json;
using Vogen.Serialization.JsonNet;
using Vogen.SerializationTests.Types;
using Xunit;

namespace Vogen.SerializationTests.JsonNetTests;

public class SerializationTests
{
    readonly JsonSerializerSettings _settings = new()
    {
        Converters = new List<JsonConverter>
        {
            new MyJsonConverter()
        },
    };

    // [Fact]
    // public void Deserialising()
    // {
    //     string text = JsonConvert.SerializeObject<Name>(Person);
    // }

    private class Person
    {
        public Name Name { get; set; } = Name.Uninitialised;

        public Age Age { get; set; } = Age.Uninitialised;
    }

    [Fact]
    public void Serialize_int_class()
    {
        var vo1 = MyIntClass.From(123);

        string s = JsonConvert.SerializeObject(vo1, _settings);

        var vo2 = JsonConvert.DeserializeObject<MyIntClass>(s, _settings);

        vo1.Should().Be(vo2);

        (vo1 == vo2).Should().BeTrue();
    }

    // [Fact]
    // public void Serialize_int_struct()
    // {
    //     var vo1 = MyIntStruct.From(123);
    //     
    //     string s = JsonConvert.SerializeObject(vo1, _settings);
    //
    //     var vo2 = JsonConvert.DeserializeObject<MyIntStruct>(s, _settings);
    //     
    //     vo1.Should().Be(vo2);
    //
    //     (vo1 == vo2).Should().BeTrue();
    // }
    //
    // [Fact]
    // public void Reading_multiple_times_gives_same_result()
    // {
    //     var vo1 = MyIntStruct.From(123);
    //     
    //     string s = JsonConvert.SerializeObject(vo1, _settings);
    //
    //     var vo2 = JsonConvert.DeserializeObject<MyIntStruct>(s, _settings);
    //     var vo3 = JsonConvert.DeserializeObject<MyIntStruct>(s, _settings);
    //     var vo4 = JsonConvert.DeserializeObject<MyIntStruct>(s, _settings);
    //     
    //     vo1.Should().Be(vo2);
    //     vo1.Should().Be(vo3);
    //     vo1.Should().Be(vo4);
    //
    //     (vo1 == vo2).Should().BeTrue();
    // }
    //
    // [Fact]
    // public void Serialize_different_types()
    // {
    //     var ic1 = MyIntClass.From(123);
    //     string s = JsonConvert.SerializeObject(ic1, _settings);
    //     var ic2 = JsonConvert.DeserializeObject<MyIntClass>(s, _settings);
    //     ic1.Should().Be(ic2);
    //     (ic1 == ic2).Should().BeTrue();
    //
    //     var age1 = Age.From(123);
    //     string s2 = JsonConvert.SerializeObject(age1, _settings);
    //     var age2 = JsonConvert.DeserializeObject<Age>(s2, _settings);
    //     age1.Should().Be(age2);
    //     (age1 == age2).Should().BeTrue();
    //
    //     var dave1 = Dave.From("David Beckham");
    //     string s3 = JsonConvert.SerializeObject(dave1, _settings);
    //     var dave2 = JsonConvert.DeserializeObject<Dave>(s3, _settings);
    //     dave1.Should().Be(dave2);
    //     (dave1 == dave2).Should().BeTrue();
    //
    //     var eightiesDate1 = EightiesDate.From(new DateTime(1985, 12, 13));
    //     string s4 = JsonConvert.SerializeObject(eightiesDate1, _settings);
    //     var eightiesDate2 = JsonConvert.DeserializeObject<EightiesDate>(s4, _settings);
    //     eightiesDate1.Should().Be(eightiesDate2);
    //     (eightiesDate1 == eightiesDate2).Should().BeTrue();
    //
    //     var name1 = Name.From("aaa");
    //     string s5 = JsonConvert.SerializeObject(name1, _settings);
    //     var name2 = JsonConvert.DeserializeObject<Name>(s5, _settings);
    //     name1.Should().Be(name2);
    //     (name1 == name2).Should().BeTrue();
    //
    //     var number1 = Number.From(42);
    //     string s6 = JsonConvert.SerializeObject(number1, _settings);
    //     var number2 = JsonConvert.DeserializeObject<Number>(s6, _settings);
    //     number1.Should().Be(number2);
    //     (number1 == number2).Should().BeTrue();
    //
    //     var score1 = Score.From(10_980);
    //     string s7 = JsonConvert.SerializeObject(score1, _settings);
    //     var score2 = JsonConvert.DeserializeObject<Score>(s7, _settings);
    //     score1.Should().Be(score2);
    //     (score1 == score2).Should().BeTrue();
    //
    //     var default1 = MyIntStructWithADefaultOf22.Default;
    //     string s8 = JsonConvert.SerializeObject(default1, _settings);
    //     var default2 = JsonConvert.DeserializeObject<MyIntStructWithADefaultOf22>(s8, _settings);
    //     default1.Should().Be(default2);
    //     (default1 == default2).Should().BeTrue();
    //
    //     var unspecified1 = MyIntWithTwoInstanceOfInvalidAndUnspecified.Unspecified;
    //     string s9 = JsonConvert.SerializeObject(unspecified1, _settings);
    //     var unspecified2 = JsonConvert.DeserializeObject<MyIntWithTwoInstanceOfInvalidAndUnspecified>(s9, _settings);
    //     unspecified1.Should().Be(unspecified2);
    //     (unspecified1 == unspecified2).Should().BeTrue();
    // }
    //
    // [Fact]
    // public void Serialize_invalid_mixes()
    // {
    //     var vo1 = MyIntClass.From(123);
    //     
    //     string s = JsonConvert.SerializeObject(vo1, _settings);
    //
    //     var vo2 = JsonConvert.DeserializeObject<MyIntStruct>(s, _settings);
    //
    //     vo2.Value.Should().Be(123);
    //     
    //     vo1.Should().NotBe(vo2);
    //
    //     // compilation error - so that's covered.
    //     //(vo1 == vo2).Should().BeTrue();
    // }
    //
    // class MyThing
    // {
    //     public MyIntClass TheClass { get; set; } = MyIntClass.From(666);
    //     public MyIntStruct TheStruct { get; set; } = MyIntStruct.From(666);
    // }
    //
    // [Fact]
    // public void Serialize_composite()
    // {
    //     var vo1 = new MyThing {TheClass = MyIntClass.From(123), TheStruct = MyIntStruct.From(333)};
    //     
    //     string s = JsonConvert.SerializeObject(vo1, _settings);
    //
    //     var vo2 = JsonConvert.DeserializeObject<MyThing>(s, _settings)!;
    //
    //     vo2.TheClass.Value.Should().Be(123);
    //     vo2.TheStruct.Value.Should().Be(333);
    // }
    //
    // [Fact]
    // public void Deserialize_invalid_int_struct()
    // {
    //     string s = "-1";
    //
    //     MyIntStruct? ret = null!;
    //     Action act = () => ret = JsonConvert.DeserializeObject<MyIntStruct>(s, _settings)!;
    //
    //     act.Should().NotThrow<ValueObjectValidationException>();
    //
    //     ret.Value.Should().Be(-1);
    // }
    //
    // // [Fact]
    // // public void Deserialize_invalid_int_struct2()
    // // {
    // //     string s = "-1";
    // //
    // //     MyIntStruct? ret = null!;
    // //     Action act = () => ret = JsonConvert.DeserializeObject<MyIntStruct>(s, _settings)!;
    // //
    // //     act.Should().NotThrow<ValueObjectValidationException>();
    // //
    // //     ret.Value.Should().Be(-1);
    // // }
    //
    // [Fact]
    // public void Deserialize_invalid_int_class()
    // {
    //     string s = "-1";
    //
    //     MyIntClass ret = null!;
    //     Action act = () => ret = JsonConvert.DeserializeObject<MyIntClass>(s, _settings)!;
    //
    //     act.Should().NotThrow<ValueObjectValidationException>();
    //
    //     ret.Value.Should().Be(-1);
    // }
}