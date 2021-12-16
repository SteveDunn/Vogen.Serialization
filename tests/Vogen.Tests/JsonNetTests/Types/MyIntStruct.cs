using System.ComponentModel;
using Vogen.Serialization.JsonNet;

namespace Vogen.SerializationTests.JsonNetTests.Types;

[ValueObject(typeof(int))]
[TypeConverter(typeof(NewtonsoftConverter))]
public partial struct MyIntStruct
{
    private static Validation Validate(int value)
    {
        if (value > 0)
            return Validation.Ok;

        return Validation.Invalid("MyIntStruct must be created with a value greater than zero");
    }
}