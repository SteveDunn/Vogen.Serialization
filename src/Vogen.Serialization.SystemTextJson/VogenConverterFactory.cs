using System;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Vogen.Serialization.SystemTextJson;

/// <summary>
/// blah //todo:
/// </summary>
public class VogenSerializationOptions
{
    /// <summary>
    /// blah //todo:
    /// </summary>
    public bool IsStrict { get; set; }
}

/// <summary>
/// blah //todo:
/// </summary>
public class VogenConverterFactory : JsonConverterFactory
{
    private readonly VogenSerializationOptions _vogenSerializerOptions;
    private static readonly ConcurrentDictionary<Type, bool> _canConvertLookup = new();
    private static readonly ConcurrentDictionary<Type, Func<CombinedSerializerOptions, object>> _builders = new();

    /// <summary>
    /// todo://
    /// </summary>
    /// <param name="options">if false, uses the 'From' method with any validation, otherwise uses the constructor with no validation.</param>
    public VogenConverterFactory(VogenSerializationOptions options) => _vogenSerializerOptions = options;

    /// <summary>
    /// todo://
    /// </summary>
    public VogenConverterFactory() : this(new VogenSerializationOptions {IsStrict = false})
    {
    }
    
    /// <summary>
    /// Returns true if the type has a ValueObjectAttribute.
    /// </summary>
    /// <param name="typeToConvert"></param>
    /// <returns>true or false //todo:</returns>
    public override bool CanConvert(Type typeToConvert)
    {
        return _canConvertLookup.GetOrAdd(typeToConvert, CanConvertInternal);

        static bool CanConvertInternal(Type typeToConvert)
        {
            ValueObjectAttribute? voAttribute =
                Attribute.GetCustomAttribute(typeToConvert, typeof(ValueObjectAttribute)) as ValueObjectAttribute;

            if (voAttribute == null)
            {
                return false;
            }

            return true;
        }
    }

    /// <summary>
    /// Creates a converter //todo:
    /// </summary>
    /// <param name="typeToConvert">the type to convert</param>
    /// <param name="jsonOptions">the jsonOptions</param>
    /// <returns>a converter</returns>
    public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions jsonOptions)
    {
        var builder = _builders.GetOrAdd(typeToConvert, createBuilderDelegate);

        var instance = builder(new CombinedSerializerOptions(jsonOptions, _vogenSerializerOptions));

        return (JsonConverter) instance;
    }

    class CombinedSerializerOptions
    {
        public CombinedSerializerOptions(JsonSerializerOptions jsonSerializerOptions, VogenSerializationOptions vogenSerializationOptions)
        {
            JsonSerializerOptions = jsonSerializerOptions;
            VogenSerializationOptions = vogenSerializationOptions;
        }

        public JsonSerializerOptions JsonSerializerOptions { get; }
        public VogenSerializationOptions VogenSerializationOptions { get; }
    }

    private Func<CombinedSerializerOptions, object> createBuilderDelegate(Type typeToConvert)
    {
        Type typeOfValueObject = typeToConvert;

        ValueObjectAttribute voAttribute =
            (ValueObjectAttribute) Attribute.GetCustomAttribute(typeToConvert, typeof(ValueObjectAttribute));

        Type typeOfPrimitive = voAttribute.UnderlyingType;

        Type genericType = typeof(ValueObjectConverterInner<,>).MakeGenericType(typeOfValueObject, typeOfPrimitive);

        var ctor = genericType.GetConstructor(new[] { typeof(CombinedSerializerOptions) })!;

        var parameter = Expression.Parameter(typeof(CombinedSerializerOptions), "options");
        NewExpression newExp = Expression.New(ctor, parameter);

        var lambda = Expression.Lambda<Func<CombinedSerializerOptions, object>>(newExp, parameter);

        return lambda.Compile();
    }

    [SuppressMessage("Microsoft.Usage", "CA1812:*", Justification = "It is instantiated by Reflection")]
    private class ValueObjectConverterInner<TValueType, TPrimitive> : JsonConverter<TValueType>
    where TPrimitive : notnull
    {
        private readonly JsonConverter<TValueType>? _valueConverter;
        private readonly Type _destinationType = typeof(TValueType);
        private readonly MethodInfo? _fromMethod;
        private readonly PropertyInfo _readerMethod;
        private readonly ConstructorInfo _constructor;
        private readonly VogenSerializationOptions _vogenOptions;

        public ValueObjectConverterInner(CombinedSerializerOptions options)
        {
            _vogenOptions = options.VogenSerializationOptions;
            
            // For performance, use the existing converter if available.
            var jsonConverter = options.JsonSerializerOptions.GetConverter(typeof(JsonConverter<TValueType>));

            _valueConverter = jsonConverter as JsonConverter<TValueType>;

            _fromMethod = _destinationType.GetMethod(
                    "From",
                    BindingFlags.Public | BindingFlags.FlattenHierarchy | BindingFlags.Static)
                ?? throw new InvalidOperationException($"Cannot find the From method on the ValueObject of type {_destinationType.FullName}");

            var types = new[] {typeof(TPrimitive)};

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

        public override TValueType Read(ref Utf8JsonReader reader,
            Type typeToConvert,
            JsonSerializerOptions options)
        {
            object? v;

            if (_valueConverter != null)
            {
                reader.Read();
                v = _valueConverter.Read(ref reader, typeof(TPrimitive), options);
            }
            else
            {
                v = JsonSerializer.Deserialize<TPrimitive>(ref reader, options);
            }

            if (v == null)
            {
                throw new InvalidOperationException($"No value to read for value object '{typeof(TValueType)}'");
            }

            try
            {
                if (_vogenOptions.IsStrict)
                {
                    var result = _fromMethod?.Invoke(null, new[] { v })
                        ?? throw new InvalidOperationException(
                            $"Value object cannot be converted from a {typeof(TValueType)} as there is no public static 'From' method defined.");

                    return (TValueType) result;
                }

                var x = _constructor?.Invoke(new[] { v })
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

        public override void Write(Utf8JsonWriter writer, TValueType value, JsonSerializerOptions options)
        {
            var valueRead = _readerMethod.GetValue(value);
            JsonSerializer.Serialize(writer, valueRead);
        }
    }
}

