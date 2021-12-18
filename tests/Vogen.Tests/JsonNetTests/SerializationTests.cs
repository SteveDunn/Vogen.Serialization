using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Newtonsoft.Json;
using Vogen.Serialization.JsonNet;
using Vogen.SerializationTests.Types;
using Xunit;
using Xunit.Abstractions;

namespace Vogen.SerializationTests.JsonNetTests;

public class SerializationTests
{
    public SerializationTests(ITestOutputHelper helper) => Console.SetOut(new ConsoleOutputHelper(helper));

    [Theory]
    [ClassData(typeof(TestData))]
    public void SerializingComposite(JsonSerializerSettings settings)
    {
        Person p = new()
        {
            Age = Age.From(42),
            Name = Name.From("Whatever")
        };

        string text = JsonConvert.SerializeObject(p, settings);

        var p2 = JsonConvert.DeserializeObject<Person>(text, settings)!;

        p2.Age.Should().Be(Age.From(42));
        p2.Name.Should().Be(Name.From("Whatever"));
    }

    [Theory]
    [ClassData(typeof(TestData))]
    public void SerializingCollection(JsonSerializerSettings settings)
    {
        var people = Enumerable.Range(1, 5).Select(
            n => new Person
            {
                Age = Age.From(n),
                Name = Name.From($"Fred{n}")
            }).ToList();

        string text = JsonConvert.SerializeObject(people, settings);

        var p2 = JsonConvert.DeserializeObject<List<Person>>(text, settings)!;

        p2.Count.Should().Be(5);

        p2[0].Age.Should().Be(Age.From(1));
        p2[0].Name.Should().Be(Name.From("Fred1"));

        p2[4].Age.Should().Be(Age.From(5));
        p2[4].Name.Should().Be(Name.From("Fred5"));
    }

    private class Person
    {
        public Name Name { get; set; } = Name.Uninitialised;

        public Age Age { get; set; } = Age.Uninitialised;
    }

    [Theory]
    [ClassData(typeof(TestData))]
    public void Serialize_int_class(JsonSerializerSettings settings)
    {
        var vo1 = MyIntClass.From(123);

        string s = JsonConvert.SerializeObject(vo1, settings);

        var vo2 = JsonConvert.DeserializeObject<MyIntClass>(s, settings);

        vo1.Should().Be(vo2);

        (vo1 == vo2).Should().BeTrue();
    }

    [Theory]
    [ClassData(typeof(TestData))]
    public void Serialize_int_struct(JsonSerializerSettings settings)
    {
        var vo1 = MyIntStruct.From(123);

        string s = JsonConvert.SerializeObject(vo1, settings);

        var vo2 = JsonConvert.DeserializeObject<MyIntStruct>(s, settings);

        vo1.Should().Be(vo2);

        (vo1 == vo2).Should().BeTrue();
    }

    [Theory]
    [ClassData(typeof(TestData))]
    public void Reading_multiple_times_gives_same_result(JsonSerializerSettings settings)
    {
        var vo1 = MyIntStruct.From(123);

        string s = JsonConvert.SerializeObject(vo1, settings);

        for (int i = 0; i < 10; i++)
        {
            var vo2 = JsonConvert.DeserializeObject<MyIntStruct>(s, settings);
            (vo1 == vo2).Should().BeTrue();
        }
    }

    [Theory]
    [ClassData(typeof(TestData))]
    public void Serialize_different_types(JsonSerializerSettings settings)
    {
        var ic1 = MyIntClass.From(123);
        string s = JsonConvert.SerializeObject(ic1, settings);
        var ic2 = JsonConvert.DeserializeObject<MyIntClass>(s, settings);
        ic1.Should().Be(ic2);
        (ic1 == ic2).Should().BeTrue();

        var age1 = Age.From(123);
        string s2 = JsonConvert.SerializeObject(age1, settings);
        var age2 = JsonConvert.DeserializeObject<Age>(s2, settings);
        age1.Should().Be(age2);
        (age1 == age2).Should().BeTrue();

        var dave1 = Dave.From("David Beckham");
        string s3 = JsonConvert.SerializeObject(dave1, settings);
        var dave2 = JsonConvert.DeserializeObject<Dave>(s3, settings);
        dave1.Should().Be(dave2);
        (dave1 == dave2).Should().BeTrue();

        var eightiesDate1 = EightiesDate.From(new DateTime(1985, 12, 13));
        string s4 = JsonConvert.SerializeObject(eightiesDate1, settings);
        var eightiesDate2 = JsonConvert.DeserializeObject<EightiesDate>(s4, settings);
        eightiesDate1.Should().Be(eightiesDate2);
        (eightiesDate1 == eightiesDate2).Should().BeTrue();

        var name1 = Name.From("aaa");
        string s5 = JsonConvert.SerializeObject(name1, settings);
        var name2 = JsonConvert.DeserializeObject<Name>(s5, settings);
        name1.Should().Be(name2);
        (name1 == name2).Should().BeTrue();

        var number1 = Number.From(42);
        string s6 = JsonConvert.SerializeObject(number1, settings);
        var number2 = JsonConvert.DeserializeObject<Number>(s6, settings);
        number1.Should().Be(number2);
        (number1 == number2).Should().BeTrue();

        var score1 = Score.From(10_980);
        string s7 = JsonConvert.SerializeObject(score1, settings);
        var score2 = JsonConvert.DeserializeObject<Score>(s7, settings);
        score1.Should().Be(score2);
        (score1 == score2).Should().BeTrue();

        var default1 = MyIntStructWithADefaultOf22.Default;
        string s8 = JsonConvert.SerializeObject(default1, settings);
        var default2 = JsonConvert.DeserializeObject<MyIntStructWithADefaultOf22>(s8, settings);
        default1.Should().Be(default2);
        (default1 == default2).Should().BeTrue();
    }

    [Theory]
    [ClassData(typeof(TestData))]
    public void Serialize_invalid_mixes(JsonSerializerSettings settings)
    {
        var vo1 = MyIntClass.From(123);

        string s = JsonConvert.SerializeObject(vo1, settings);

        var vo2 = JsonConvert.DeserializeObject<MyIntStruct>(s, settings);

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

    [Theory]
    [ClassData(typeof(TestData))]
    public void Serialize_composite(JsonSerializerSettings settings)
    {
        var vo1 = new MyThing { TheClass = MyIntClass.From(123), TheStruct = MyIntStruct.From(333) };

        string s = JsonConvert.SerializeObject(vo1, settings);

        var vo2 = JsonConvert.DeserializeObject<MyThing>(s, settings)!;

        vo2.TheClass.Value.Should().Be(123);
        vo2.TheStruct.Value.Should().Be(333);
    }

    [Fact]
    public void Deserialize_invalid_int_struct_in_non_strict_mode()
    {
        string s = "-1";

        MyIntStruct ret = MyIntStruct.From(1);
        Action act = () => ret = JsonConvert.DeserializeObject<MyIntStruct>(s, TestData.NonStrictSerializerSettings);

        act.Should().NotThrow<ValueObjectValidationException>();

        ret.Value.Should().Be(-1);
    }

    [Fact]
    public void Deserialize_invalid_int_struct_in_strict_mode()
    {
        string s = "-1";

        MyIntStruct ret = MyIntStruct.From(1);
        Action act = () => ret = JsonConvert.DeserializeObject<MyIntStruct>(s, TestData.StrictSerializerSettings);

        act.Should().Throw<ValueObjectValidationException>().WithMessage("MyIntStruct must be created with a value greater than zero");
    }

    [Fact]
    public void Deserialize_invalid_int_class_in_non_strict_mode()
    {
        string s = "-1";

        MyIntClass ret = null!;
        Action act = () => ret = JsonConvert.DeserializeObject<MyIntClass>(s, TestData.NonStrictSerializerSettings)!;

        act.Should().NotThrow<ValueObjectValidationException>();

        ret.Value.Should().Be(-1);
    }

    [Fact]
    public void Deserialize_invalid_int_class_in_strict_mode()
    {
        string s = "-1";

        MyIntClass ret = null!;
        Action act = () => ret = JsonConvert.DeserializeObject<MyIntClass>(s, TestData.StrictSerializerSettings)!;

        act.Should().Throw<ValueObjectValidationException>().WithMessage("MyIntClass must be created with a value greater than zero");
    }

    public class TestData : IEnumerable<object[]>
    {
        public static JsonSerializerSettings StrictSerializerSettings = new()
        {
            Converters = new List<JsonConverter>
            {
                new ValueObjectConverter(isStrict: true)
            },
        };

        public static JsonSerializerSettings NonStrictSerializerSettings = new()
        {
            Converters = new List<JsonConverter>
            {
                new ValueObjectConverter(isStrict: false)
            },
        };

        public IEnumerator<object[]> GetEnumerator()
        {
            yield return new object[]
            {
                NonStrictSerializerSettings
            };
            yield return new object[]
            {
                StrictSerializerSettings
            };
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}

