namespace Vogen.Examples.SystemTextJson;

[ValueObject(typeof(int))]
[Instance("Unspecified", -1)]
public partial struct CustomerId
{
    private static Validation Validate(int value) =>
        value > 0 ? Validation.Ok : Validation.Invalid("Must be greater than zero.");
}