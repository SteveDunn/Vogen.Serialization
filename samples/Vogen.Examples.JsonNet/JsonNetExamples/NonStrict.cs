using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Vogen.Serialization.JsonNet;

namespace Vogen.Examples.JsonNetExamples;

internal class NonStrict
{
    private static readonly JsonSerializerSettings _nonStrictSerializerSettings = new()
    {
        Converters = new List<JsonConverter>
        {
            new ValueObjectConverter(isStrict: false)
        },
    };

    public void Run()
    {
        SerializatonOfGoodValues();
        SerializatonOfInvalidValueDoesNotThrow();
        SerializatonOfUnspecifiedValueDoesNotThrow();
    }

    /*
    Non strict means we skip validation when deserializing back into a value object.
    You'll need this mode if any of your value objects have instance values, e.g. 'Unspecified'.
     */

    private static void SerializatonOfUnspecifiedValueDoesNotThrow()
    {
        var originalVo = JsonNet.CustomerId.Unspecified;
        var json = JsonConvert.SerializeObject(originalVo, _nonStrictSerializerSettings);

        var newVo = JsonConvert.DeserializeObject<JsonNet.CustomerId>(json, _nonStrictSerializerSettings);
        if (newVo.Value != JsonNet.CustomerId.Unspecified)
        {
            throw new InvalidOperationException("should be the same before and after serialization.");
        }

    }

    private static void SerializatonOfInvalidValueDoesNotThrow()
    {
        string invalidValue = "0";

        var newVo = JsonConvert.DeserializeObject<JsonNet.CustomerId>(invalidValue, _nonStrictSerializerSettings);
        if (newVo.Value != 0)
        {
            throw new InvalidOperationException("should be the same before and after serialization!");
        }
    }

    private static void SerializatonOfGoodValues()
    {
        var originalVo = JsonNet.CustomerId.From(123);

        var json = JsonConvert.SerializeObject(originalVo, _nonStrictSerializerSettings);

        JsonNet.CustomerId newVo = JsonConvert.DeserializeObject<JsonNet.CustomerId>(json, _nonStrictSerializerSettings);

        if (newVo != originalVo)
        {
            throw new InvalidOperationException("should be the same before and after serialization!");
        }
    }
}