using System;
using System.Collections.Generic;
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
    class ConversionOptions
    {
        public bool IsStrict { get; set; }
    }

    // private ValueObjectConverterInnerBase _instance = null!;
    private readonly ConversionOptions _conversionOptions;

    /// <summary>
    /// todo:
    /// </summary>
    /// <param name="isStrict"></param>
    public MyJsonConverter(bool isStrict = false) => _conversionOptions = new ConversionOptions
    {
        IsStrict = isStrict
    };

    // /// <summary>
    // /// todo:
    // /// </summary>
    // public override bool CanWrite => false;

    /// <summary>
    /// todo:
    /// </summary>
    /// <param name="writer"></param>
    /// <param name="value"></param>
    /// <param name="serializer"></param>
    /// <exception cref="NotImplementedException"></exception>
    public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
    {
        _instance.Write(value, writer, serializer);
        //serializer.Serialize(writer, value);
    }

    /// <summary>
    /// todo:
    /// </summary>
    /// <param name="reader"></param>
    /// <param name="objectType"></param>
    /// <param name="existingValue"></param>
    /// <param name="serializer"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public override object ReadJson(JsonReader reader, Type objectType, object? existingValue,
        JsonSerializer serializer)
    {

        //var r3 = serializer.Deserialize<string>(reader);

//        var jo1 = serializer.Deserialize(reader, instance.PrimitiveType)!;
        //var jo = serializer.Deserialize<JValue>(reader)!;

        var ret2 = _instance.Read(reader, serializer);

        return ret2;
    }

    private ValueObjectConverterInnerBase _instance = null!;

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

        var ctor = genericType.GetConstructor(new[] { typeof(ConversionOptions) })!;

        _instance = (ValueObjectConverterInnerBase) ctor.Invoke(
            new[]
            {
                _conversionOptions
            });

        return true;
    }

    abstract class ValueObjectConverterInnerBase
    {
        public abstract object Read(JsonReader value, JsonSerializer jsonSerializer);
            // public abstract Type PrimitiveType { get; }

        public abstract void Write(object? value, JsonWriter writer, JsonSerializer serializer);
    }

    //[SuppressMessage("Microsoft.Usage", "CA1812:*", Justification = "It is instantiated by Reflection")]
    private class ValueObjectConverterInner<TValueType, TPrimitive> : ValueObjectConverterInnerBase where TPrimitive : notnull
    {
        private readonly ConversionOptions _options;
        private readonly Type _destinationType = typeof(TValueType);
        private readonly ConstructorInfo _constructor;
        private readonly Func<TPrimitive, TValueType> _fromDelegate;
        private readonly Func<TPrimitive> _valueDelegate;

        public ValueObjectConverterInner(ConversionOptions options)
        {
            _options = options;
            
            var types = new[] { typeof(TPrimitive) };
            
            _constructor = _destinationType.GetConstructor(
                BindingFlags.NonPublic | BindingFlags.Instance,
                null,
                types,
                null)
                ?? throw new InvalidOperationException($"Cannot find the constructor on ValueObject on type {_destinationType.FullName} that takes a type of {typeof(TPrimitive)}");

            _fromDelegate = CreateFromMethodDelegate() ?? throw new InvalidOperationException($"Cannot find the From method on the ValueObject of type {_destinationType.FullName}"); 
            _valueDelegate = CreateValueMethodDelegate() ?? throw new InvalidOperationException($"Cannot find the Value property on the ValueObject of type {_destinationType.FullName}"); 
        }

        private Func<TPrimitive, TValueType> CreateFromMethodDelegate()
        {
            Delegate openDelegate = Delegate.CreateDelegate(
                typeof(Func<TPrimitive, TValueType>),
                typeof(TValueType),
                "From",
                false) ?? throw new InvalidOperationException($"Cannot find the From method on the ValueObject of type {_destinationType.FullName}");

            return (Func<TPrimitive, TValueType>) openDelegate ;
        }
        private Func<TPrimitive> CreateValueMethodDelegate()
        {
            PropertyInfo prop = typeof(TValueType).GetProperty("Value") ?? throw new InvalidOperationException($"No Value property on {typeof(TValueType).Name}");

            MethodInfo methodInfo = prop.GetGetMethod() ?? throw new InvalidOperationException($"No get method on property on {typeof(TValueType).Name}");
            
            Delegate open2 = Delegate.CreateDelegate(
                    typeof(Func<TPrimitive>),
                    null,
                    methodInfo);

            return (Func<TPrimitive>) open2 ;
        }

        public override object Read(JsonReader reader, JsonSerializer serializer)
        {
            // if (v == null)
            // {
            //     throw new InvalidOperationException($"No value to read for value object '{typeof(TValueType)}'");
            // }

            try
            {
                var jo = serializer.Deserialize<TPrimitive>(reader)!;


                //var parms = new object[] { v.Value<TPrimitive>()! };
                var parms = new object[] { jo! };

                if (_options.IsStrict)
                {
                    var r2 = _fromDelegate!.Invoke(jo)!;
                    //var r2 = _fromDelegate!.Invoke(v.Value<TPrimitive>()!)!;
                    // var result = (TValueType) _fromMethod?.Invoke(null, parms)!
                    //     ?? throw new InvalidOperationException(
                    //         $"Value object cannot be converted from a {typeof(TValueType)} as there is no public static 'From' method defined.");

                    return (TValueType) r2;
//                    return (TValueType) result;
                }

                var x = _constructor?.Invoke(parms)
                    ?? throw new InvalidOperationException(
                        $"Value object cannot be converted from a {typeof(TValueType)} as there is no public constructor taking '{typeof(TPrimitive)}'");

                return (TValueType) x;
            }
            catch (Exception e) when (e is TargetInvocationException &&
                                      e.InnerException is ValueObjectValidationException)
            {
                throw e.InnerException;
            }
        }

        //public override Type PrimitiveType => typeof(TPrimitive);
        
        // we're given a ValueObject, so just write its 'Value'
        public override void Write(object? value, JsonWriter writer, JsonSerializer serializer)
        {
            if (value == null)
            {
                return;
            }

            var type = value.GetType();
            var p =type.GetProperty("Value", BindingFlags.Instance | BindingFlags.Public) ?? throw new Exception($"No property named Value on type passed which was {type.Name}");
            var vret= p.GetValue(value);
            //var v = _valueDelegate.Invoke();
            serializer.Serialize(writer, (TPrimitive)vret);
        }
    }
}