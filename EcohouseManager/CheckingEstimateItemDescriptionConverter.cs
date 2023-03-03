using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using System.Windows.Data;

namespace EcohouseManager
{
    /// <summary>
    /// Конвертер "наличия описания" пункта сметы в булевое значение.
    /// </summary>
    
    public class CheckingEstimateItemDescriptionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string isEstimateItemHasDescription = value.ToString();
            return !isEstimateItemHasDescription.Equals("{нет описания}");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
