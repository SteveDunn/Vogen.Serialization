using System;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;

namespace Vogen.Serialization.JsonNet;

    /// <summary>
    /// Converts objects to types that are Vogen value objects, so that they can be deserialized.
    /// </summary>
    public class NewtonsoftStringBasedValueObjectConverter : TypeConverter
    {
        private readonly MethodInfo? _fromStringMethod;

        /// <summary>
        /// //todo:
        /// </summary>
        /// <param name="destinationType">todo:</param>
        public NewtonsoftStringBasedValueObjectConverter(Type destinationType)
        {
            _fromStringMethod = TryGetBuilderMethod("FromString", destinationType) ??
                                TryGetBuilderMethod("FromText", destinationType);
        }

        private static MethodInfo? TryGetBuilderMethod(string methodName, Type destinationType)
        {
            return destinationType.GetMethod(
                methodName,
                BindingFlags.Public | BindingFlags.DeclaredOnly | BindingFlags.Static);
        }

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(string) && _fromStringMethod != null)
            {
                return true;
            }

            return base.CanConvertFrom(context, sourceType);
        }

        public override object? ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value is string)
            {
                return _fromStringMethod?.Invoke(null, new[] { value }) ?? throw new InvalidOperationException(
                    "Value type cannot be converted from a string as there is no public static 'FromString' method defined.");
            }

            return base.ConvertFrom(context, culture, value);
        }
    }
