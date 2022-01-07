using System.Text.Json;
using Vogen.Serialization.SystemTextJson;

namespace Vogen.Examples.SystemTextJson.SystemTextJsonExamples;

internal class Strict
{
    static readonly JsonSerializerOptions _strictSerializerSettings = new()
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

    public void Run()
    {
        SerializationOfGoodValues();
        DeserializeZeroShouldThrow();
        DeserializeUnspecifiedShouldThrow();
    }

    private static void SerializationOfGoodValues()
    {
        var originalVo = CustomerId.From(123);

        var json = JsonSerializer.Serialize(originalVo, _strictSerializerSettings);

        CustomerId newVo = JsonSerializer.Deserialize<CustomerId>(json, _strictSerializerSettings);

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
            JsonSerializer.Deserialize<CustomerId>(invalidValue, _strictSerializerSettings);
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
            var originalVo = CustomerId.Unspecified;

            var json = JsonSerializer.Serialize(originalVo, _strictSerializerSettings);
            CustomerId vo = JsonSerializer.Deserialize<CustomerId>(json, _strictSerializerSettings);
        }
        catch (ValueObjectValidationException)
        {
            return;
        }

        throw new InvalidOperationException("was expecting an exception!");
    }
}