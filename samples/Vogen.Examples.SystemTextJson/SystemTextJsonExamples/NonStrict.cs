using System.Text.Json;
using Vogen.Serialization.SystemTextJson;

namespace Vogen.Examples.SystemTextJson.SystemTextJsonExamples;

internal class NonStrict
{
    static readonly JsonSerializerOptions _nonStrictSerializerSettings = new()
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
        var originalVo = CustomerId.Unspecified;
        var json = JsonSerializer.Serialize(originalVo, _nonStrictSerializerSettings);

        var newVo = JsonSerializer.Deserialize<CustomerId>(json, _nonStrictSerializerSettings);
        if (newVo.Value != CustomerId.Unspecified)
        {
            throw new InvalidOperationException("should be the same before and after serialization.");
        }

    }

    private static void SerializatonOfInvalidValueDoesNotThrow()
    {
        string invalidValue = "0";

        var newVo = JsonSerializer.Deserialize<CustomerId>(invalidValue, _nonStrictSerializerSettings);
        if (newVo.Value != 0)
        {
            throw new InvalidOperationException("should be the same before and after serialization!");
        }
    }

    private static void SerializatonOfGoodValues()
    {
        var originalVo = CustomerId.From(123);

        var json = JsonSerializer.Serialize(originalVo, _nonStrictSerializerSettings);

        CustomerId newVo = JsonSerializer.Deserialize<CustomerId>(json, _nonStrictSerializerSettings);

        if (newVo != originalVo)
        {
            throw new InvalidOperationException("should be the same before and after serialization!");
        }
    }
}