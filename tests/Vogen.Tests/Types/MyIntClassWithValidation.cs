namespace Vogen.SerializationTests.Types;

[ValueObject(typeof(int))]
public partial class MyIntClassWithValidation
{
    private static Validation Validate(int value)
    {
        if (value > 0)
            return Validation.Ok;

        return Validation.Invalid("MyIntClassWithValidation must be created with a value greater than zero");
    }
}