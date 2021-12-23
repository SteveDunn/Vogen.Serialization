using System.Collections.Generic;

namespace Vogen.Serialization.TestTypes;

public class Composite
{
    public NameAsStruct NameAsStruct { get; set; }
    public NameAsClass NameAsClass { get; set; }
    public NumberAsClass NumberAsClass { get; set; }
    public NumberAsStruct NumberAsStruct { get; set; }
    

    public List<NameAsStruct> NamesAsStruct { get; set; }
    public List<NameAsClass> NamesAsClass { get; set; }
    public List<NumberAsClass> NumbersAsClass { get; set; }
    public List<NumberAsStruct> NumbersAsStruct { get; set; }
    
}