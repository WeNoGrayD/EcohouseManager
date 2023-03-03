using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using System.Windows.Data;
using System.Xml;
using System.Windows;

namespace EcohouseManager
{
    /// <summary>
    /// Конвертер из хмла в перечисление RoofType.
    /// В хмле выбранный тип кровли хранится в виде строчного значения элемента перечисления.
    /// </summary>
    
    public class XmlToRoofTypeMultiConverter : IMultiValueConverter
    {
        // Конвертация в тег комбобокса.

        public object Convert(object[] values, Type targetType, object parameter,
                              CultureInfo culture)
        {
            if (values[0] == DependencyProperty.UnsetValue)
                return null;

            string rooftype = ((XmlAttribute)values[0]).Value;
            ((EcohouseProjectModel)values[1]).SelectedRoofType = (EcohouseProjectModel.RoofType)
                Enum.Parse(typeof(EcohouseProjectModel.RoofType), rooftype);
            return rooftype;
        }

        // Конвертация из тега комбобокса. Не нужна, посему не внедрена.

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter,
                                   CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
