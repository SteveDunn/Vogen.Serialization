namespace Vogen.Serialization.Benchmarks;

public record struct NumberAsRecordStruct(int Value);

[ValueObject(typeof(int))]
public partial struct NumberAsStruct
{
}

[ValueObject(typeof(int))]
public partial class NumberAsClass
{
}

[ValueObject(typeof(string))]
public partial class NameAsClass
{
}

[ValueObject(typeof(string))]
public partial struct NameAsStruct
{
}
