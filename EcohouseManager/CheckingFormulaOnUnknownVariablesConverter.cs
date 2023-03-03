using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using System.Windows.Data;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using System.Windows.Media.Imaging;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Collections.ObjectModel;
using System.Drawing;

namespace EcohouseManager
{
    /// <summary>
    /// Проверка формулы и выдача источника изображения для вывода 
    /// сообщения вида "обнаружены неизвестные переменные" или
    /// его отсутствия.
    /// </summary>

    public class CheckingFormulaOnUnknownVariablesConverter 
        : IValueConverter, INotifyPropertyChanged
    {
        private class ObservableVariablesCollection : List<string>
        {
            public event Action ObservableVariablesCollectionChanged;

            public void CopyFrom(IEnumerable<string> ieVars)
            {
                this.Clear();
                foreach (string var in ieVars)
                    this.Add(var);

                ObservableVariablesCollectionChanged();
            }
        }

        // Формула, в данный момент проверяемая данным конвертером.
        // Используется на случай обновления документа Variables.xml.

        private string _formula;

        // Регулярное выражение, проверяющее формулу на наличие переменных.

        private static Regex reCheckingFormula;

        // Массивы строк имён известных переменных/констант.

        private static ObservableVariablesCollection
            _knownVars, _knownConsts;

        // XЭлементы, из которых берутся переменные/константы.

        public static IEnumerable<XElement> xsVars
        {
            set
            {
                IEnumerable<string> newKnownVars =
                    //value.Select(xVar => xVar.Attribute("name").Value);
                    value.Select(xVar => xVar.Name.ToString());
                if (!newKnownVars.Equals(_knownVars))
                    _knownVars.CopyFrom(newKnownVars);
            }
        }

        public static IEnumerable<XElement> xsConsts
        {
            set
            {
                IEnumerable<string> newKnownConsts =
                    value.Select(xConst => xConst.Attribute("name").Value);
                if (!newKnownConsts.Equals(_knownConsts))
                    _knownConsts.CopyFrom(newKnownConsts);
            }
        }

        // Список неизвестных переменных/констант.

        private ObservableCollection<string> _unknownVars;

        // Текст, выводимый тултипом.

        private string _unknownVarsMessage;

        public string UnknownVarsMessage
        {
            get { return _unknownVarsMessage; }
            private set
            {
                _unknownVarsMessage = value;
                OnPropertyChanged("UnknownVarsMessage");
            }
        }

        private void RefreshUnknownVarsMessage()
        {
            string message = "";

            if (_unknownVars.Count != 0)
            {
                message = "Следующие переменные не обнаружены:\n";
                message += _unknownVars.Aggregate((msg, varName) =>
                { return msg + ";\n" + varName; });
            }
            else
                message = "Все переменные в формуле существуют.";

            UnknownVarsMessage = message;
        }

        // Статический конструктор.

        static CheckingFormulaOnUnknownVariablesConverter()
        {
           reCheckingFormula = new Regex(@"\b[A-Za-zА-Яа-я_][_\w]*\(?",
               RegexOptions.Compiled);

            _knownVars = new ObservableVariablesCollection();
            _knownConsts = new ObservableVariablesCollection();
        }

        // Обычный конструктор.

        public CheckingFormulaOnUnknownVariablesConverter()
        {
            _unknownVars = new ObservableCollection<string>();

            _knownVars.ObservableVariablesCollectionChanged += () =>
            { RefreshUnknownVars(); };
            _knownConsts.ObservableVariablesCollectionChanged += () =>
            { RefreshUnknownVars(); };
        }

        // Обновление списка неизвестных переменных.

        private bool RefreshUnknownVars()
        {
            _unknownVars.Clear();

            bool isThereAnyUnknownVar = false, varIsUnknown;

            string varName;
            foreach (Match varMatch in reCheckingFormula.Matches(_formula))
            {
                varName = varMatch.Value;
                // Нормально имена переменных не ищутся,
                // приходится пропускать имена команд.
                if (varName.Last() == '(')
                    continue;

                varIsUnknown = !_knownVars.Contains(varName) &&
                               !_knownConsts.Contains(varName) &&
                               !BestCalculatorEver.GetSupportedMathConstants()
                                                  .Contains(varName);
                isThereAnyUnknownVar |= varIsUnknown;
                if (varIsUnknown)
                    _unknownVars.Add(varName);
            }

            RefreshUnknownVarsMessage();

            return isThereAnyUnknownVar;
        }

        // Исследование формулы на предмет неизвестных переменных/констант.

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            _formula = value.ToString();

            bool isThereAnyUnknownVar = RefreshUnknownVars();

            BitmapImage bmpNotify;

            if (isThereAnyUnknownVar)
            {
                Uri uriWarning = new Uri("/Images/warning-128.png", UriKind.Relative);
                bmpNotify = new BitmapImage(uriWarning);
            }
            else
            {
                Uri uriOkay = new Uri("/Images/ok-128.png", UriKind.Relative);
                bmpNotify = new BitmapImage(uriOkay);
            }

            return bmpNotify; //!isThereAnyUnknownVar;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        // Уведомление подписчиков на событие изменения свойства.

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}
