namespace Vogen.Serialization.TestTypes;

public class SmallComposite
{
    public MyIntClass TheClass { get; set; } = MyIntClass.From(666);
    public MyIntStruct TheStruct { get; set; } = MyIntStruct.From(666);
}

