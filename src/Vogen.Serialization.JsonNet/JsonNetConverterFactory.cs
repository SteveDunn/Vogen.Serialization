using System;
using Newtonsoft.Json;

namespace Vogen.Serialization.JsonNet;

internal class JsonNetConverterFactory : JsonConverterFactory
{
    public override bool CanConvert(Type typeToConvert) => throw new NotImplementedException();

    public override JsonConverter? CreateConverter(Type typeToConvert, JsonSerializerOptions options) => throw new NotImplementedException();
}