namespace Vogen.Serialization.TestTypes;

[ValueObject(typeof(int))]
public partial class MyIntClass
{
    private static Validation Validate(int value)
    {
        if (value > 0)
            return Validation.Ok;

        return Validation.Invalid("MyIntClass must be created with a value greater than zero");
    }
}