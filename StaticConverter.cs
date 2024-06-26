using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace pckkey
{
    public static class StaticConverter
    {
        public static int ToInt32(this string value)
        {
            int.TryParse(value, out int res);
            return res;
        }

        public static string ToGBK(this byte[] value) => Encoding.GetEncoding(936).GetString(value)
                .Split(new string[] { "\0" }, StringSplitOptions.RemoveEmptyEntries)[0];
        public static string RemoveFirstSeparator(this string value)
        {
            if (value.Length > 2 && value.StartsWith("\\"))
                value = value.Remove(0, 1);
            return value;
        }

        public static string RemoveFirst(this string value, string pattern)
        {
            int index = value.IndexOf(pattern);
            if (index > -1)
                value = value.Remove(index, pattern.Length);
            return value;
        }

        public static byte[] FromGBK(this string value) => Encoding.GetEncoding(936).GetBytes(value + '\0');
    }

    public class StringFormatToIntConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string)
            {
                if (value.ToString().Contains(",") || value.ToString().Contains("."))
                    return float.TryParse(value.ToString(), out float number) ? number : 0;
                else
                    return int.TryParse(value.ToString(), out int number) ? number : 0;
            }
            else
            {
                return value;
            }
        }
    }
}
