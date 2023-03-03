using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace EcohouseManager
{
    // Класс параметра для привязки к контролу на форме.
    // Заменяет собой, по сути, свойство.

    public class CalculationHeaderParam : INotifyPropertyChanged
    {
        private decimal _value;

        public decimal Value
        {
            get { return _value; }
            set
            {
                if (value != _value)
                {
                    _value = value;
                    OnPropertyChanged("Value");
                    EcohouseProjectModel.CalculationHeaderParams[ParamName] = value;
                }
            }
        }

        public string ParamName;

        public CalculationHeaderParam(string paramName)
        {
            this.ParamName = paramName;
        }

        // Уведомление подписчиков на событие изменения свойства.

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}
