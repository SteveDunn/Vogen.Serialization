using System.ComponentModel;
using Vogen.Serialization.JsonNet;

namespace Vogen.SerializationTests.JsonNetTests.Types;

[TypeConverter(typeof(NewtonsoftConverter))]
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