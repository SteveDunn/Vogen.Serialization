using Newtonsoft.Json;
using Vogen.Serialization.JsonNet;

namespace Vogen.Examples.JsonNetExamples;

internal class Strict
{
    static readonly JsonSerializerSettings _strictSerializerSettings = new()
    {
        Converters = new List<JsonConverter>
        {
            new ValueObjectConverter(isStrict: true)
        },
    };

    public void Run()
    {
        SerializationOfGoodValues();
        DeserializeZeroShouldThrow();
        DeserializeUnspecifiedShouldThrow();
    }

    private static void SerializationOfGoodValues()
    {
        var originalVo = JsonNet.CustomerId.From(123);

        var json = JsonConvert.SerializeObject(originalVo, _strictSerializerSettings);

        JsonNet.CustomerId newVo = JsonConvert.DeserializeObject<JsonNet.CustomerId>(json, _strictSerializerSettings);

        if (newVo != originalVo)
        {
            throw new InvalidOperationException("should be the same before and after serialization!");
        }
    }

    private static void DeserializeZeroShouldThrow()
    {
        string invalidValue = "0";

        try
        {
            JsonConvert.DeserializeObject<JsonNet.CustomerId>(invalidValue, _strictSerializerSettings);
        }
        catch (ValueObjectValidationException)
        {
            return;
        }

        throw new InvalidOperationException("was expecting an exception!");
    }

    private static void DeserializeUnspecifiedShouldThrow()
    {
        try
        {
            var originalVo = JsonNet.CustomerId.Unspecified;

            var json = JsonConvert.SerializeObject(originalVo, _strictSerializerSettings);
            JsonNet.CustomerId vo = JsonConvert.DeserializeObject<JsonNet.CustomerId>(json, _strictSerializerSettings);
        }
        catch (ValueObjectValidationException)
        {
            return;
        }

        throw new InvalidOperationException("was expecting an exception!");
    }
}