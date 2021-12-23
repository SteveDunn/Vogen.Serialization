namespace Vogen.Serialization.TestTypes;

[ValueObject(typeof(int))]
[Instance(name: "Uninitialised", value: "-1")]
public partial class Age
{
    private static Validation Validate(int value)
    {
        if (value >= 0)
            return Validation.Ok;

        return Validation.Invalid("age cannot be negative");
    }
}
