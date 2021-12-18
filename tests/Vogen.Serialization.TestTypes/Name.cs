namespace Vogen.Serialization.TestTypes;

[ValueObject(typeof(string))]
[Instance(name: "Uninitialised", value: "[uninitialised]")]
public partial class Name
{
    private static Validation Validate(string value)
    {
        if (value.Length > 0)
        {
            return Validation.Ok;
        }

        return Validation.Invalid("name cannot be empty");
    }
}
