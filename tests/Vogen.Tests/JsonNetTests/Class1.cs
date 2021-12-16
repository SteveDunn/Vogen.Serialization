using System;
using System.ComponentModel;
using FluentAssertions;
using Newtonsoft.Json;
using Xunit;
using Vogen.Serialization.JsonNet;

namespace Vogen.SerializationTests.JsonNetTests
{
    public class DeserialiseJson
    {
        public class NewtonsoftStringBasedValueObjectConverterTests
    {
        [Fact]
        public void Deserialising()
        {
            var name = JsonConvert.DeserializeObject<Name>(@"");
        }






        private class Person
        {
            public Name Name { get; set; } = Name.Uninitialised;

            public Age Age { get; set; } = string.Empty;
        }
    }

        [TypeConverter(typeof(NewtonsoftStringBasedValueObjectConverter))]
        [ValueObject(typeof(string))]
        [Instance(name: "Invalid", value: "xxx")]
        public partial class Name
        {
            private static Validation Validate(string value)
            {
                if (value.Length > 0)
                    return Validation.Ok;

                return Validation.Invalid("name cannot be empty");
            }
        }

        [TypeConverter(typeof(NewtonsoftStringBasedValueObjectConverter))]
        [ValueObject(typeof(int))]
        [Instance(name: "Invalid", value: "xxx")]
        public partial class Age
        {
            private static Validation Validate(int value)
            {
                if (value >= 0)
                    return Validation.Ok;

                return Validation.Invalid("age cannot be negative");
            }
        }
    }