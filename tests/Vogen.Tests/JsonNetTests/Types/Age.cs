using System.ComponentModel;
using Vogen.Serialization.JsonNet;

namespace Vogen.SerializationTests.JsonNetTests.Types;

[TypeConverter(typeof(NewtonsoftConverter))]
[ValueObject(typeof(int))]
public partial class Age
{
}