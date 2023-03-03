using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Data;
using System.Windows;

namespace EcohouseManager
{
    public class StringToDoubleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double dbl = System.Convert.ToDouble(value);
            return $"{dbl:F3}";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string str = (string)value;
            return double.Parse(str != string.Empty ? str : "0");
        }
    }

    public class StringToDecimalConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            decimal dec = System.Convert.ToDecimal(value);
            return $"{dec:f3}";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string str = (string)value;
            return decimal.Parse(str != string.Empty ? str : "0");
        }
    }
}
