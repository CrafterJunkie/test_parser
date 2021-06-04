using System;
using System.ComponentModel;
using System.Globalization;

namespace Test_Parser
{
    [TypeConverter(typeof(FloatConverter))]
    public class FloatConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            return double.Parse(value.ToString(), CultureInfo.InvariantCulture);
        }
    }
}