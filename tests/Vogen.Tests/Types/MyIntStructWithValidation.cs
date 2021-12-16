namespace Vogen.SerializationTests.Types;

[ValueObject(typeof(int))]
public partial struct MyIntStructWithValidation
{
    private static Validation Validate(int value)
    {
        if (value > 0)
            return Validation.Ok;

        return Validation.Invalid("MyIntStructWithValidation must be created with a value greater than zero");
    }
}