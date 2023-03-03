using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Globalization;
using System.Windows.Controls;

namespace EcohouseManager
{
    // Конвертер из строки в элемент перечисления типов крыши.

    public class StringToRoofTypeConverter
         : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (parameter == null)
            {
                switch ((EcohouseProjectModel.RoofType)value)
                {
                    case EcohouseProjectModel.RoofType.TwoFloors: { return "Два этажа"; }
                    case EcohouseProjectModel.RoofType.FourSlopes: { return "Четыре ската"; }
                    case EcohouseProjectModel.RoofType.RidgeRoof: { return "От конька"; }
                    case EcohouseProjectModel.RoofType.RoofLorry: { return "Полуторка"; }
                    default: { return "{не выбран}"; }
                }
            }
            else
                return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return EcohouseProjectModel.RoofType.None;
            if (parameter == null || (string)parameter == "select")
            {
                switch ((string)((ComboBoxItem)value).Content)
                {
                    case "Два этажа": { return EcohouseProjectModel.RoofType.TwoFloors; }
                    case "Четыре ската": { return EcohouseProjectModel.RoofType.FourSlopes; }
                    case "От конька": { return EcohouseProjectModel.RoofType.RidgeRoof; }
                    case "Полуторка": { return EcohouseProjectModel.RoofType.RoofLorry; }
                    default: { return EcohouseProjectModel.RoofType.None; }
                }
            }
            else if ((string)parameter == "xml")
            {
                return Enum.Parse(typeof(EcohouseProjectModel.RoofType), (string)value);
            }
            else return EcohouseProjectModel.RoofType.None;
        }
    }
}
