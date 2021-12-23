using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.Json;
using FluentAssertions;
using Vogen.Serialization.SystemTextJson;
using Vogen.Serialization.TestTypes;
using Vogen.Serialization.UnitTests.JsonNetTests;
using Xunit;
using Xunit.Abstractions;

namespace Vogen.Serialization.UnitTests.SystemTextJsonTests;

public class SerializationTests : IDisposable
{
    private readonly TraceOutputHelper _traceOutputHelper;

    public SerializationTests(ITestOutputHelper helper)
    {
        _traceOutputHelper = new TraceOutputHelper(helper);
        Trace.Listeners.Add(_traceOutputHelper);
    }

    public void Dispose() => Trace.Listeners.Remove(_traceOutputHelper);


    [Theory]
    [ClassData(typeof(TestData))]
    public void SerializingCompositeList(JsonSerializerOptions options)
    {
        var composites = Enumerable.Range(1, 10).Select(
            n =>
                new Composite
                {
                    NameAsClass = NameAsClass.From($"Name {n}"),
                    NumberAsClass = NumberAsClass.From(n),
                    NameAsStruct = NameAsStruct.From($"Name {n}"),
                    NumberAsStruct = NumberAsStruct.From(n),

                    NamesAsClass = Enumerable.Range(1, 10).Select(n => NameAsClass.From($"Name {n}")).ToList(),
                    NamesAsStruct = Enumerable.Range(1, 10).Select(n => NameAsStruct.From($"Name {n}")).ToList(),
                    NumbersAsClass = Enumerable.Range(1, 10).Select(NumberAsClass.From).ToList(),
                    NumbersAsStruct = Enumerable.Range(1, 10).Select(NumberAsStruct.From).ToList(),
                }).ToList();

        string text = JsonSerializer.Serialize(composites, options);

        composites = JsonSerializer.Deserialize<List<Composite>>(text, options)!;

        composites[0].NumbersAsClass[0].Value.Should().Be(1);
        composites[9].NumbersAsClass[9].Value.Should().Be(10);
    }

    [Theory]
    [ClassData(typeof(TestData))]
    public void SerializingComposite(JsonSerializerOptions options)
    {
        Person p = new()
        {
            Age = Age.From(42),
            Name = Name.From("Whatever")
        };

        string text = JsonSerializer.Serialize(p, options);

        var p2 = JsonSerializer.Deserialize<Person>(text, options)!;

        p2.Age.Should().Be(Age.From(42));
        p2.Name.Should().Be(Name.From("Whatever"));
    }

    [Theory]
    [ClassData(typeof(TestData))]
    public void SerializingCollection(JsonSerializerOptions options)
    {
        var people = Enumerable.Range(1, 5).Select(
            n => new Person
            {
                Age = Age.From(n),
                Name = Name.From($"Fred{n}")
            }).ToList();

        string text = JsonSerializer.Serialize(people, options);

        var p2 = JsonSerializer.Deserialize<List<Person>>(text, options)!;

        p2.Count.Should().Be(5);

        p2[0].Age.Should().Be(Age.From(1));
        p2[0].Name.Should().Be(Name.From("Fred1"));

        p2[4].Age.Should().Be(Age.From(5));
        p2[4].Name.Should().Be(Name.From("Fred5"));
    }

    [Theory]
    [ClassData(typeof(TestData))]
    public void Serialize_int_class(JsonSerializerOptions options)
    {
        var vo1 = MyIntClass.From(123);

        string s = JsonSerializer.Serialize(vo1, options);

        var vo2 = JsonSerializer.Deserialize<MyIntClass>(s, options);

        vo1.Should().Be(vo2);

        (vo1 == vo2).Should().BeTrue();
    }

    [Theory]
    [ClassData(typeof(TestData))]
    public void Serialize_int_struct(JsonSerializerOptions options)
    {
        var vo1 = MyIntStruct.From(123);

        string s = JsonSerializer.Serialize(vo1, options);

        var vo2 = JsonSerializer.Deserialize<MyIntStruct>(s, options);

        vo1.Should().Be(vo2);

        (vo1 == vo2).Should().BeTrue();
    }

    [Theory]
    [ClassData(typeof(TestData))]
    public void Reading_multiple_times_gives_same_result(JsonSerializerOptions options)
    {
        var vo1 = MyIntStruct.From(123);

        string s = JsonSerializer.Serialize(vo1, options);

        for (int i = 0; i < 10; i++)
        {
            var vo2 = JsonSerializer.Deserialize<MyIntStruct>(s, options);
            (vo1 == vo2).Should().BeTrue();
        }
    }

    [Theory]
    [ClassData(typeof(TestData))]
    public void Serialize_different_types(JsonSerializerOptions options)
    {
        var ic1 = MyIntClass.From(123);
        string s = JsonSerializer.Serialize(ic1, options);
        var ic2 = JsonSerializer.Deserialize<MyIntClass>(s, options);
        ic1.Should().Be(ic2);
        (ic1 == ic2).Should().BeTrue();

        var age1 = Age.From(123);
        string s2 = JsonSerializer.Serialize(age1, options);
        var age2 = JsonSerializer.Deserialize<Age>(s2, options);
        age1.Should().Be(age2);
        (age1 == age2).Should().BeTrue();

        var dave1 = Dave.From("David Beckham");
        string s3 = JsonSerializer.Serialize(dave1, options);
        var dave2 = JsonSerializer.Deserialize<Dave>(s3, options);
        dave1.Should().Be(dave2);
        (dave1 == dave2).Should().BeTrue();

        var eightiesDate1 = EightiesDate.From(new DateTime(1985, 12, 13));
        string s4 = JsonSerializer.Serialize(eightiesDate1, options);
        var eightiesDate2 = JsonSerializer.Deserialize<EightiesDate>(s4, options);
        eightiesDate1.Should().Be(eightiesDate2);
        (eightiesDate1 == eightiesDate2).Should().BeTrue();

        var name1 = Name.From("aaa");
        string s5 = JsonSerializer.Serialize(name1, options);
        var name2 = JsonSerializer.Deserialize<Name>(s5, options);
        name1.Should().Be(name2);
        (name1 == name2).Should().BeTrue();

        var number1 = Number.From(42);
        string s6 = JsonSerializer.Serialize(number1, options);
        var number2 = JsonSerializer.Deserialize<Number>(s6, options);
        number1.Should().Be(number2);
        (number1 == number2).Should().BeTrue();

        var score1 = Score.From(10_980);
        string s7 = JsonSerializer.Serialize(score1, options);
        var score2 = JsonSerializer.Deserialize<Score>(s7, options);
        score1.Should().Be(score2);
        (score1 == score2).Should().BeTrue();

        var default1 = MyIntStructWithADefaultOf22.Default;
        string s8 = JsonSerializer.Serialize(default1, options);
        var default2 = JsonSerializer.Deserialize<MyIntStructWithADefaultOf22>(s8, options);
        default1.Should().Be(default2);
        (default1 == default2).Should().BeTrue();
    }

    [Theory]
    [ClassData(typeof(TestData))]
    public void Serialize_invalid_mixes(JsonSerializerOptions options)
    {
        var vo1 = MyIntClass.From(123);

        string s = JsonSerializer.Serialize(vo1, options);

        var vo2 = JsonSerializer.Deserialize<MyIntStruct>(s, options);

        vo2.Value.Should().Be(123);

        vo1.Should().NotBe(vo2);

        // compilation error - so that's covered.
        //(vo1 == vo2).Should().BeTrue();
    }

    [Theory]
    [ClassData(typeof(TestData))]
    public void Serialize_composite(JsonSerializerOptions options)
    {
        var vo1 = new SmallComposite { TheClass = MyIntClass.From(123), TheStruct = MyIntStruct.From(333) };

        string s = JsonSerializer.Serialize(vo1, options);

        var vo2 = JsonSerializer.Deserialize<SmallComposite>(s, options)!;

        vo2.TheClass.Value.Should().Be(123);
        vo2.TheStruct.Value.Should().Be(333);
    }

    [Fact]
    public void Deserialize_invalid_int_struct_in_non_strict_mode()
    {
        string s = "-1";

        MyIntStruct ret = MyIntStruct.From(1);
        Action act = () => ret = JsonSerializer.Deserialize<MyIntStruct>(s, TestData.NonStrictSerializerSettings);

        act.Should().NotThrow<ValueObjectValidationException>();

        ret.Value.Should().Be(-1);
    }

    [Fact]
    public void Deserialize_invalid_int_struct_in_strict_mode()
    {
        string s = "-1";

        MyIntStruct ret = MyIntStruct.From(1);
        Action act = () => ret = JsonSerializer.Deserialize<MyIntStruct>(s, TestData.StrictSerializerSettings);

        act.Should().Throw<ValueObjectValidationException>().WithMessage("MyIntStruct must be created with a value greater than zero");
    }

    [Fact]
    public void Deserialize_invalid_int_class_in_non_strict_mode()
    {
        string s = "-1";

        MyIntClass ret = null!;
        Action act = () => ret = JsonSerializer.Deserialize<MyIntClass>(s, TestData.NonStrictSerializerSettings)!;

        act.Should().NotThrow<ValueObjectValidationException>();

        ret.Value.Should().Be(-1);
    }

    [Fact]
    public void Deserialize_invalid_int_class_in_strict_mode()
    {
        string s = "-1";

        MyIntClass ret = null!;
        Action act = () => ret = JsonSerializer.Deserialize<MyIntClass>(s, TestData.StrictSerializerSettings)!;

        act.Should().Throw<ValueObjectValidationException>().WithMessage("MyIntClass must be created with a value greater than zero");
    }

    public class TestData : IEnumerable<object[]>
    {
        public static JsonSerializerOptions StrictSerializerSettings = new()
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

        public static JsonSerializerOptions NonStrictSerializerSettings = new()
        {
            Converters =
            {
                new VogenConverterFactory(
                    new VogenSerializationOptions
                    {
                        IsStrict = false
                    })
            }
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