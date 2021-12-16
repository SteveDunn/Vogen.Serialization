using System;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;

namespace Vogen.Serialization.JsonNet;

    /// <summary>
    /// Converts objects to types that are Vogen value objects, so that they can be deserialized.
    /// </summary>
    public class NewtonsoftConverter : TypeConverter
    {
        private readonly MethodInfo? _builderMethod;

        /// <summary>
        /// //todo:
        /// </summary>
        /// <param name="destinationType">todo:</param>
        public NewtonsoftConverter(Type destinationType)
        {
            _builderMethod = TryGetBuilderMethod("From", destinationType);
        }

        private static MethodInfo? TryGetBuilderMethod(string methodName, Type destinationType)
        {
            return destinationType.GetMethod(
                methodName,
                BindingFlags.Public | BindingFlags.DeclaredOnly | BindingFlags.Static);
        }

        /// <summary>
        /// todo:
        /// </summary>
        /// <param name="context"></param>
        /// <param name="sourceType"></param>
        /// <returns></returns>
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            ValueObjectAttribute? voAttribute =
                Attribute.GetCustomAttribute(sourceType, typeof(ValueObjectAttribute)) as ValueObjectAttribute;

            if (voAttribute != null)
            {
                return true;
            }

            return base.CanConvertFrom(context, sourceType);
        }

        /// <summary>
        /// todo:
        /// </summary>
        /// <param name="context"></param>
        /// <param name="culture"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public override object? ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            //if (value is string)
            {
                return _builderMethod?.Invoke(null, new[] { value }) ?? throw new InvalidOperationException(
                    "Value type cannot be converted from a string as there is no public static 'FromString' method defined.");
            }

            // return base.ConvertFrom(context, culture, value);
        }
    }
