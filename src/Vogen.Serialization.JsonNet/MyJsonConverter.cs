using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Vogen.Serialization.JsonNet;

/// <summary>
/// todo:
/// </summary>
public class MyJsonConverter : JsonConverter
{
    // private MethodInfo? _builderMethod;
    private Func<CombinedSerializerOptions, object> _lambda = null!;
    private ValueObjectConverterInnerBase _instance = null!;

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
         var jo = serializer.Deserialize<JValue>(reader)!;
    //
    //     return jo.Value<int>();//["Value"]!;
    // }

    var ret2 = _instance.Build(jo);
    return ret2;
    
        // var converterInner = (ValueObjectConverterInnerBase)_lambda.Invoke(null!);
        //
        // var ret = converterInner.Build(jo);
        //
        // return ret;
        //return _builderMethod?.Invoke(null, new[] {reader.Value}) ?? throw new InvalidOperationException("Null!");
    }

    class CombinedSerializerOptions
    {
        
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

        if (voAttribute == null)
        {
            return false;
        }

        Type typeOfPrimitive = voAttribute.UnderlyingType;

        Type genericType = typeof(ValueObjectConverterInner<,>).MakeGenericType(objectType, typeOfPrimitive);

        var ctor = genericType.GetConstructor(new[] { typeof(CombinedSerializerOptions) })!;

        _instance = (ValueObjectConverterInnerBase) ctor.Invoke(
            new[]
            {
                new CombinedSerializerOptions()
            });

        var parameter = Expression.Parameter(typeof(CombinedSerializerOptions), "options");
        NewExpression newExp = Expression.New(ctor, parameter);

        var lambda = Expression.Lambda<Func<CombinedSerializerOptions, object>>(newExp, parameter);

        _lambda = lambda.Compile();

        return true;
    }

    abstract class ValueObjectConverterInnerBase
    {
        public abstract object Build(JValue value);
    }

    [SuppressMessage("Microsoft.Usage", "CA1812:*", Justification = "It is instantiated by Reflection")]
    private class ValueObjectConverterInner<TValueType, TPrimitive> : ValueObjectConverterInnerBase where TPrimitive : notnull
    {
        // private readonly JsonConverter<TValueType>? _valueConverter;
        private readonly Type _destinationType = typeof(TValueType);
        private readonly MethodInfo? _fromMethod;
        private readonly PropertyInfo _readerMethod;
        private readonly ConstructorInfo _constructor;

        public ValueObjectConverterInner(CombinedSerializerOptions options)
        {
            // For performance, use the existing converter if available.
            // var jsonConverter = options.JsonSerializerOptions.GetConverter(typeof(JsonConverter<TValueType>));

            // _valueConverter = jsonConverter as JsonConverter<TValueType>;

            _fromMethod = _destinationType.GetMethod(
                    "From",
                    BindingFlags.Public | BindingFlags.FlattenHierarchy | BindingFlags.Static)
                ?? throw new InvalidOperationException($"Cannot find the From method on the ValueObject of type {_destinationType.FullName}");

            var types = new[] { typeof(TPrimitive) };
            
            _constructor = _destinationType.GetConstructor(
                BindingFlags.NonPublic | BindingFlags.Instance,
                null,
                types,
                null)
                ?? throw new InvalidOperationException($"Cannot find the constructor on ValueObject on type {_destinationType.FullName} that takes a type of {typeof(TPrimitive)}");

            var valueProperty = _destinationType.GetProperty(
                "Value",
                BindingFlags.Public | BindingFlags.Instance);

            _readerMethod = valueProperty
                ?? throw new InvalidOperationException(
                    $"Cannot find the Value property on ValueObject of type {_destinationType.FullName}");
        }

        public TValueType Build(Type typeToConvert, JValue v)
        {
            if (v == null)
            {
                throw new InvalidOperationException($"No value to read for value object '{typeof(TValueType)}'");
            }

            try
            {
                if (true)
                {
                    var parms = new object[] {v.Value<TPrimitive>()!};
                    
                    var result = (TValueType)_fromMethod?.Invoke(null, parms)!
                        ?? throw new InvalidOperationException(
                            $"Value object cannot be converted from a {typeof(TValueType)} as there is no public static 'From' method defined.");

                    return (TValueType) result;
                }

                // var x = _constructor?.Invoke(new[] { v })
                //     ?? throw new InvalidOperationException(
                //         $"Value object cannot be converted from a {typeof(TValueType)} as there is no public constructor taking '{typeof(TPrimitive)}'");
                //
                // return (TValueType) x;
            }
            catch (Exception e) when (e is TargetInvocationException &&
                                      e.InnerException is ValueObjectValidationException)
            {
                throw e.InnerException;
            }
        }

        public override object Build(JValue value)
        {
            return Build(null!, value)!;
        }
    }
}