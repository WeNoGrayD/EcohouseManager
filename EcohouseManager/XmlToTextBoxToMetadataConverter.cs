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
    public class XmlToTextBoxToMetadataConverter : IValueConverter
    {
        public static ObservableDictionary<string, string> Metadata { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (Metadata != null && EcohouseProjectViewModel.ProjectMustBeLoaded)
                Metadata[(string)parameter] = ((XmlElement)value).InnerText;
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
