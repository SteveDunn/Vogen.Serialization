using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using Newtonsoft.Json;

namespace Vogen.Serialization.JsonNet;

/// <summary>
/// todo:
/// </summary>
public class ValueObjectConverter : JsonConverter
{
    private class ConversionOptions
    {
        public bool IsStrict { get; set; }
    }

    private readonly ConversionOptions _conversionOptions;
    private readonly Dictionary<Type, InnerSerializer> _innerSerializers;

    /// <summary>
    /// todo:
    /// </summary>
    /// <param name="isStrict"></param>
    public ValueObjectConverter(bool isStrict = false)
    {
        Trace.WriteLine("xxxxx ValueObjectConverter constructor");

        _innerSerializers = new Dictionary<Type, InnerSerializer>();

        _conversionOptions = new ConversionOptions
        {
            IsStrict = isStrict
        };
    }

    /// <summary>
    /// todo:
    /// </summary>
    /// <param name="writer"></param>
    /// <param name="value"></param>
    /// <param name="serializer"></param>
    /// <exception cref="NotImplementedException"></exception>
    public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
    {
        if (value is null)
        {
            throw new InvalidOperationException("value is null!");
        }
        
        if (!_innerSerializers.TryGetValue(value.GetType(), out var inner))
        {
            throw new InvalidOperationException($"No inner serializer - cannot write {value.GetType()}");
        }

        inner.Write(value, writer, serializer);
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
    public override object ReadJson(
        JsonReader reader,
        Type objectType,
        object? existingValue,
        JsonSerializer serializer)
    {
        if (!_innerSerializers.TryGetValue(objectType, out var inner))
        {
            throw new InvalidOperationException("No inner serializer!");
        }

        return inner.Read(reader, serializer);
    }

    /// <summary>
    /// todo:
    /// </summary>
    /// <param name="objectType"></param>
    /// <returns></returns>
    /// <exception cref="JsonException"></exception>
    public override bool CanConvert(Type objectType)
    {
        Trace.WriteLine($"In CanConvert for {objectType}");

        ValueObjectAttribute? voAttribute =
            Attribute.GetCustomAttribute(objectType, typeof(ValueObjectAttribute)) as ValueObjectAttribute;

        if (voAttribute == null)
        {
            return false;
        }

        if (_innerSerializers.ContainsKey(objectType))
        {
            return true;
        }

        Type typeOfPrimitive = voAttribute.UnderlyingType;

        Type genericType = typeof(InnerSerializer<,>).MakeGenericType(objectType, typeOfPrimitive);

        var ctor = genericType.GetConstructor(new[] { typeof(ConversionOptions) })!;

        var serializer = (InnerSerializer) ctor.Invoke(
            new[]
            {
                _conversionOptions
            });
        
        _innerSerializers.Add(objectType, serializer);

        Trace.WriteLine($"++ CanConvert => true");

        return true;
    }

    abstract class InnerSerializer
    {
        public abstract object Read(JsonReader value, JsonSerializer jsonSerializer);

        public abstract void Write(object? value, JsonWriter writer, JsonSerializer serializer);
    }

    private class InnerSerializer<TValueType, TPrimitive> : InnerSerializer where TPrimitive : notnull
    {
        private readonly ConversionOptions _options;
        private readonly Type _destinationType = typeof(TValueType);
        private readonly ConstructorInfo _constructor;
        private readonly Func<TPrimitive, TValueType> _fromDelegate;
        private readonly Func<TPrimitive> _valueDelegate;

        public InnerSerializer(ConversionOptions options)
        {
            Trace.WriteLine($"InnerSerializer ({GetType()}) constructor");

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

            return (Func<TPrimitive, TValueType>) openDelegate;
        }
        private Func<TPrimitive> CreateValueMethodDelegate()
        {
            PropertyInfo prop = typeof(TValueType).GetProperty("Value") ?? throw new InvalidOperationException($"No Value property on {typeof(TValueType).Name}");

            MethodInfo methodInfo = prop.GetGetMethod() ?? throw new InvalidOperationException($"No get method on property on {typeof(TValueType).Name}");

            Delegate valuePropertyDelegate = Delegate.CreateDelegate(
                    typeof(Func<TPrimitive>),
                    null,
                    methodInfo);

            return (Func<TPrimitive>) valuePropertyDelegate;
        }

        public override object Read(JsonReader reader, JsonSerializer serializer)
        {
            try
            {
                var jo = serializer.Deserialize<TPrimitive>(reader)!;

                if (_options.IsStrict)
                {
                    return _fromDelegate.Invoke(jo)!;
                }

                var parms = new object[] { jo! };

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

        public override void Write(object? value, JsonWriter writer, JsonSerializer serializer)
        {
            // todo: cache
            if (value == null)
            {
                return;
            }

            var type = value.GetType();

            var property = type.GetProperty("Value", BindingFlags.Instance | BindingFlags.Public)
                ?? throw new Exception($"No property named Value on type passed which was {type.Name}");

            serializer.Serialize(writer, (TPrimitive) property.GetValue(value));
        }
    }
}