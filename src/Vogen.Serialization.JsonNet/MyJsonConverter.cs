using System;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Vogen.Serialization.JsonNet;

/// <summary>
/// todo:
/// </summary>
public class MyJsonConverter : JsonConverter
{
    private MethodInfo? _builderMethod;
    
    /// <summary>
    /// todo:
    /// </summary>
    public override bool CanWrite => false;

    // public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    // {
    //   writer.wr  
    // }

    /// <summary>
    /// todo:
    /// </summary>
    /// <param name="writer"></param>
    /// <param name="value"></param>
    /// <param name="serializer"></param>
    /// <exception cref="NotImplementedException"></exception>
    public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer) => throw new NotImplementedException();

    /// <summary>
    /// todo:
    /// </summary>
    /// <param name="reader"></param>
    /// <param name="objectType"></param>
    /// <param name="existingValue"></param>
    /// <param name="serializer"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public override object ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
    {
        return _builderMethod?.Invoke(null, new[] {reader.Value}) ?? throw new InvalidOperationException("Null!");
    }

    /// <summary>
    /// todo:
    /// </summary>
    /// <param name="objectType"></param>
    /// <returns></returns>
    /// <exception cref="JsonException"></exception>
    public override bool CanConvert(Type objectType)
    {
        ValueObjectAttribute? voAttribute =
            Attribute.GetCustomAttribute(objectType, typeof(ValueObjectAttribute)) as ValueObjectAttribute;

        if (voAttribute != null)
        {
            MethodInfo? bm = objectType.GetMethod(
                "From",
                BindingFlags.Public | BindingFlags.DeclaredOnly | BindingFlags.Static);

            if (bm == null)
            {
                throw new JsonException("Found a Value Object without a From method!");
            }

            _builderMethod = bm;
            
            return true;
        }

        return false;
    }
}