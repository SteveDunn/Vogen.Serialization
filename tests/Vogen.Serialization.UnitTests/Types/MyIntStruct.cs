namespace Vogen.Serialization.UnitTests.Types;

[ValueObject(typeof(int))]
public partial struct MyIntStruct
{
    private static Validation Validate(int value)
    {
        if (value > 0)
            return Validation.Ok;

        return Validation.Invalid("MyIntStruct must be created with a value greater than zero");
    }
}