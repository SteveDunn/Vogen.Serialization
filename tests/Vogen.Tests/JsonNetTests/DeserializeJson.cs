using System;
using System.ComponentModel;
using FluentAssertions;
using Newtonsoft.Json;
using Xunit;
using Vogen.Serialization.JsonNet;

namespace Vogen.SerializationTests.JsonNetTests
{
    //[TypeConverter(typeof(NewtonsoftConverter))]
    [ValueObject(typeof(string))]
    [Instance(name: "Uninitialised", value: "[uninitialised]")]
    public partial class Name
    {
        private static Validation Validate(string value)
        {
            if (value.Length > 0)
            {
                return Validation.Ok;
            }

            return Validation.Invalid("name cannot be empty");
        }
    }

    //[TypeConverter(typeof(NewtonsoftConverter))]
    [ValueObject(typeof(int))]
    [Instance(name: "Uninitialised", value: "-1")]
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