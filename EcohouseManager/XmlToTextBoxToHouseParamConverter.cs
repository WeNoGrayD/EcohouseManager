using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using System.Windows.Data;
using System.Xml;

namespace EcohouseManager
{
    /// <summary>
    /// Конвертер из тега хмла в текст текстбокса в шапочный параметр (decimal).
    /// </summary>

    public class XmlToTextBoxToHouseParamConverter : IValueConverter
    {
        // Словарь параметров шапки. 

        public static ObservableDictionary<string, decimal> CalculationHeaderParams { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (CalculationHeaderParams != null && EcohouseProjectViewModel.ProjectMustBeLoaded)
                CalculationHeaderParams[(string)parameter] = System.Convert
                    .ToDecimal(((XmlElement)value).InnerText);
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
