using System.ComponentModel;
using System.Text.Json;
using BenchmarkDotNet.Attributes;
using Vogen.Serialization.SystemTextJson;

namespace Vogen.Serialization.Benchmarks
{
    [MemoryDiagnoser, Description("The underlying type is int and the VOs validation that they're > 0")]
    public class Underlying_Int_With_Validation
    {
        private JsonSerializerOptions _options;

        [Params(true, false)]
        public bool Strict { get; set; }

        [GlobalSetup]
        public void GlobalSetup()
        {
            _options = new JsonSerializerOptions()
            {
                Converters =
                {
                    new VogenConverterFactory(new VogenSerializationOptions
                    {
                        IsStrict = Strict
                    })
                }
            };
        }

        [Benchmark(Baseline = true)]
        public (int, int) UsingIntNatively()
        {
            int x = TestData.RandomNumberBetween(1, 10_000);
            int y = TestData.RandomNumberBetween(1, 10_000);

            var xSerialized = JsonSerializer.Serialize(x, _options);
            var ySerialized = JsonSerializer.Serialize(y, _options);

            int x2 = JsonSerializer.Deserialize<int>(xSerialized, _options);
            int y2 = JsonSerializer.Deserialize<int>(ySerialized, _options);
            
            return (x2, y2);
        }

        [Benchmark]
        public (NumberAsRecordStruct,NumberAsRecordStruct) UsingRecordStruct()
        {
            var x = new NumberAsRecordStruct(TestData.RandomNumberBetween(1, 10_000));
            var y = new NumberAsRecordStruct(TestData.RandomNumberBetween(1, 10_000));

            var xSerialized = JsonSerializer.Serialize(x, _options);
            var ySerialized = JsonSerializer.Serialize(y, _options);

            var x2 = JsonSerializer.Deserialize<NumberAsRecordStruct>(xSerialized, _options);
            var y2 = JsonSerializer.Deserialize<NumberAsRecordStruct>(ySerialized, _options);

            return (x2, y2);
        }

        [Benchmark]
        public (NumberAsStruct x2, NumberAsStruct y2) UsingValueObjectStruct()
        {
            var x = NumberAsStruct.From(TestData.RandomNumberBetween(1, 10_000));
            var y = NumberAsStruct.From(TestData.RandomNumberBetween(1, 10_000));

            var xSerialized = JsonSerializer.Serialize(x, _options);
            var ySerialized = JsonSerializer.Serialize(y, _options);

            var x2 = JsonSerializer.Deserialize<NumberAsStruct>(xSerialized, _options);
            var y2 = JsonSerializer.Deserialize<NumberAsStruct>(ySerialized, _options);

            return (x2, y2);
        }

        [Benchmark]
        public (NumberAsClass x2, NumberAsClass y2) UsingValueObjectAsClass()
        {
            var x = NumberAsClass.From(TestData.RandomNumberBetween(1, 10_000));
            var y = NumberAsClass.From(TestData.RandomNumberBetween(1, 10_000));

            var xSerialized = JsonSerializer.Serialize(x, _options);
            var ySerialized = JsonSerializer.Serialize(y, _options);

            var x2 = JsonSerializer.Deserialize<NumberAsClass>(xSerialized, _options);
            var y2 = JsonSerializer.Deserialize<NumberAsClass>(ySerialized, _options);

            return (x2, y2);
        }
    }

    [MemoryDiagnoser, Description("The underlying type is string and the VOs validation that they're not null or empty")]
    public class Underlying_string_With_Validation
    {
        private JsonSerializerOptions _options;
        
        private string Combine(string s1, string s2) => $"{s1}-{s2}";
        
        public NameAsClass Combine(NameAsClass s1, NameAsClass s2) => NameAsClass.From($"{s1}-{s2}");
        
        public NameAsStruct Combine(NameAsStruct s1, NameAsStruct s2) => NameAsStruct.From($"{s1}-{s2}");

        [GlobalSetup]
        public void GlobalSetup()
        {
            _options = new JsonSerializerOptions()
            {
                Converters = { new VogenConverterFactory() }
            };
        }

        [Benchmark(Baseline = true)]
        public (string r3, string r4) UsingStringNatively()
        {
            var r1 = TestData.GetRandomString();
            var r1Serialised = JsonSerializer.Serialize(r1, _options);

            var r2 = TestData.GetRandomString();
            var r2Serialised = JsonSerializer.Serialize(r2, _options);

            var r3 = JsonSerializer.Deserialize<string>(r1Serialised, _options);
            var r4 = JsonSerializer.Deserialize<string>(r2Serialised, _options);

            return (r3, r4);
        }

        [Benchmark]
        public (NameAsClass r3, NameAsClass r4) UsingValueObjectAsClass()
        {
            var r1 = NameAsClass.From(TestData.GetRandomString());
            var r1Serialised = JsonSerializer.Serialize(r1, _options);

            var r2 = NameAsClass.From(TestData.GetRandomString());
            var r2Serialised = JsonSerializer.Serialize(r2, _options);

            var r3 = JsonSerializer.Deserialize<NameAsClass>(r1Serialised, _options);
            var r4 = JsonSerializer.Deserialize<NameAsClass>(r2Serialised, _options);

            return (r3, r4);
        }

        [Benchmark]
        public (NameAsStruct r3, NameAsStruct r4) UsingValueObjectAsStruct()
        {
            var r1 = NameAsStruct.From(TestData.GetRandomString());
            var r1Serialised = JsonSerializer.Serialize(r1, _options);

            var r2 = NameAsStruct.From(TestData.GetRandomString());
            var r2Serialised = JsonSerializer.Serialize(r2, _options);

            var r3 = JsonSerializer.Deserialize<NameAsStruct>(r1Serialised, _options);
            var r4 = JsonSerializer.Deserialize<NameAsStruct>(r2Serialised, _options);

            return (r3, r4);
        }
    }
}
