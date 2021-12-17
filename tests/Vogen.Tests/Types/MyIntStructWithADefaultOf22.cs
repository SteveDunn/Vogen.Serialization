namespace Vogen.SerializationTests.Types;

[ValueObject(typeof(int))]
[Instance(name: "Default", value: 22)]
[Instance(name: "Default2", value: 33)]
public partial struct MyIntStructWithADefaultOf22
{
}