using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using BenchmarkDotNet.Attributes;
using Newtonsoft.Json;
using Vogen.Serialization.JsonNet;
using Vogen.Serialization.TestTypes;

namespace Benchmarks.JsonNet;

[MemoryDiagnoser, Description("[De]Serializing with Newtonsoft.Json")]
public class JsonNetBenchmarks
{
    private JsonSerializerSettings _options;

    [Params(true, false)]
    public bool Strict { get; set; }

    public string Library => "Newtonsoft.Json";

    [GlobalSetup]
    public void GlobalSetup() =>
        _options = new JsonSerializerSettings
        {
            Converters = new List<JsonConverter>
            {
                new ValueObjectConverter(isStrict: Strict)
            },
        };

    [Benchmark]
    public (int, int) NativeInt_JsonNet()
    {
        int x = TestData.RandomNumberBetween(1, 10_000);
        int y = TestData.RandomNumberBetween(1, 10_000);

        var xSerialized = JsonConvert.SerializeObject(x, _options);
        var ySerialized = JsonConvert.SerializeObject(y, _options);

        int x2 = JsonConvert.DeserializeObject<int>(xSerialized, _options);
        int y2 = JsonConvert.DeserializeObject<int>(ySerialized, _options);
            
        return (x2, y2);
    }

    [Benchmark]
    public (NumberAsRecordStruct,NumberAsRecordStruct) record_struct_containing_int()
    {
        var x = new NumberAsRecordStruct(TestData.RandomNumberBetween(1, 10_000));
        var y = new NumberAsRecordStruct(TestData.RandomNumberBetween(1, 10_000));

        var xSerialized = JsonConvert.SerializeObject(x, _options);
        var ySerialized = JsonConvert.SerializeObject(y, _options);

        var x2 = JsonConvert.DeserializeObject<NumberAsRecordStruct>(xSerialized, _options);
        var y2 = JsonConvert.DeserializeObject<NumberAsRecordStruct>(ySerialized, _options);

        return (x2, y2);
    }

    [Benchmark]
    public (NumberAsStruct x2, NumberAsStruct y2) value_object_struct_containing_int()
    {
        var x = NumberAsStruct.From(TestData.RandomNumberBetween(1, 10_000));
        var y = NumberAsStruct.From(TestData.RandomNumberBetween(1, 10_000));

        var xSerialized = JsonConvert.SerializeObject(x, _options);
        var ySerialized = JsonConvert.SerializeObject(y, _options);

        var x2 = JsonConvert.DeserializeObject<NumberAsStruct>(xSerialized, _options);
        var y2 = JsonConvert.DeserializeObject<NumberAsStruct>(ySerialized, _options);

        return (x2, y2);
    }

    [Benchmark]
    public (NumberAsClass x2, NumberAsClass y2) value_object_class_containing_int()
    {
        var x = NumberAsClass.From(TestData.RandomNumberBetween(1, 10_000));
        var y = NumberAsClass.From(TestData.RandomNumberBetween(1, 10_000));

        var xSerialized = JsonConvert.SerializeObject(x, _options);
        var ySerialized = JsonConvert.SerializeObject(y, _options);

        var x2 = JsonConvert.DeserializeObject<NumberAsClass>(xSerialized, _options);
        var y2 = JsonConvert.DeserializeObject<NumberAsClass>(ySerialized, _options);

        return (x2, y2);
    }

    private string Combine(string s1, string s2) => $"{s1}-{s2}";

    public NameAsClass Combine(NameAsClass s1, NameAsClass s2) => NameAsClass.From($"{s1}-{s2}");

    public NameAsStruct Combine(NameAsStruct s1, NameAsStruct s2) => NameAsStruct.From($"{s1}-{s2}");

    [Benchmark]
    public (string r3, string r4) native_string()
    {
        var r1 = TestData.GetRandomString();
        var r1Serialised = JsonConvert.SerializeObject(r1, _options);

        var r2 = TestData.GetRandomString();
        var r2Serialised = JsonConvert.SerializeObject(r2, _options);

        var r3 = JsonConvert.DeserializeObject<string>(r1Serialised, _options);
        var r4 = JsonConvert.DeserializeObject<string>(r2Serialised, _options);

        return (r3, r4);
    }

    [Benchmark]
    public (NameAsClass r3, NameAsClass r4) value_object_class_containing_string()
    {
        var r1 = NameAsClass.From(TestData.GetRandomString());
        var r1Serialised = JsonConvert.SerializeObject(r1, _options);

        var r2 = NameAsClass.From(TestData.GetRandomString());
        var r2Serialised = JsonConvert.SerializeObject(r2, _options);

        var r3 = JsonConvert.DeserializeObject<NameAsClass>(r1Serialised, _options);
        var r4 = JsonConvert.DeserializeObject<NameAsClass>(r2Serialised, _options);

        return (r3, r4);
    }

    [Benchmark]
    public (NameAsStruct r3, NameAsStruct r4) value_object_struct_containing_string()
    {
        var r1 = NameAsStruct.From(TestData.GetRandomString());
        var r1Serialised = JsonConvert.SerializeObject(r1, _options);

        var r2 = NameAsStruct.From(TestData.GetRandomString());
        var r2Serialised = JsonConvert.SerializeObject(r2, _options);

        var r3 = JsonConvert.DeserializeObject<NameAsStruct>(r1Serialised, _options);
        var r4 = JsonConvert.DeserializeObject<NameAsStruct>(r2Serialised, _options);

        return (r3, r4);
    }

    [Benchmark]
    public string various_value_objects()
    {
        Serialize(10, n => NameAsClass.From($"Name {n}"));
        Serialize(10, n => NameAsStruct.From($"Name {n}"));
        
        Serialize(10, NumberAsStruct.From);
        Serialize(10, NumberAsClass.From);

        Serialize(
            10,
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
                }
        );
        

        return "Done";
    }

    private IEnumerable<T> Serialize<T>(int amount, Func<int, T> func)
    {
        var n1 = Enumerable.Range(1, amount).Select(func).ToList();

        var text = JsonConvert.SerializeObject(n1, _options);

        n1 = JsonConvert.DeserializeObject<List<T>>(text, _options);

        return n1;
    }
}