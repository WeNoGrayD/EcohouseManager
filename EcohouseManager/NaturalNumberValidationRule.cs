using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace EcohouseManager
{
    // Класс содержит правила валидации вводимых 
    // в текстбоксы параметров дома/материалов.
    // Первое правило клуба натуральных чисел - не быть неприводимой к числу строкой.
    // Второе правило клуба натуральных чисел - не быть ненатуральным числом.

    class NaturalNumberValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            double natNum = 0;
            string errMes = "";
            if (!double.TryParse((string)value, out natNum))
            {
                errMes = "Введены недопустимые символы.";
                return new ValidationResult(false, errMes);
            }

            if (natNum < 0)
            {
                errMes = "Значение параметра должно быть больше либо равно нулю.";
                return new ValidationResult(false, errMes);
            }

            return new ValidationResult(true, errMes);
        }
    }
}
