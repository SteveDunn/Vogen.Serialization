using System.ComponentModel;
using Vogen.Serialization.JsonNet;

namespace Vogen.SerializationTests.JsonNetTests.Types;

[TypeConverter(typeof(NewtonsoftConverter))]
[ValueObject(typeof(int))]
public readonly partial struct Number
{
}
