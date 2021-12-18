namespace Vogen.Serialization.TestTypes;

public class Person
{
    public Name Name { get; set; } = TestTypes.Name.Uninitialised;

    public Age Age { get; set; } = TestTypes.Age.Uninitialised;
}
