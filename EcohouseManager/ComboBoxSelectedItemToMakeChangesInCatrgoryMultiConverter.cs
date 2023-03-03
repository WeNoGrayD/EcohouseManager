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
    /// Конвертер, предназначенный для получения материалов при изменении
    /// выбора в комбобоксе на странице ТЗ.
    /// </summary>

    public class ComboBoxSelectedItemToMakeChangesInCatrgoryMultiConverter : IMultiValueConverter
    {
        // Целевая категория, с которой связан этот конвертер.

        private TechTaskCategory _targetCategory;

        /* 
         * Целевой материал, с которым связан этот конвертер.
         * Поскольку у категории может быть один или несколько материалов,
         * то целевой приходится сохранять.
         */

        private Material _targetMaterial;

        // Конвертация в селектедитем комбобокса.

        public object Convert(object[] values, Type targetType, object parameter,
                              CultureInfo culture)
        {
            // Сохранить целевую категорию можно только 
            // при конвертации В селектедитем.

            if (values[1] != _targetCategory)
                _targetCategory = values[1] as TechTaskCategory;
            if (values[2] != _targetMaterial)
                _targetMaterial = values[2] as Material;

            return null;
        }

        // Конвертация из селектедитема комбобокса.

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter,
                                    CultureInfo culture)
        {
            // Если хмл материала не загружен на время инициализации
            // данного комбобокса, то возвращаем всё как есть.

            if (value == null || _targetCategory == null || _targetMaterial == null)
            {
                if (_targetCategory == null)
                    return new object[] { false, null };
                if (_targetCategory is CategoryWithSingleMaterial)
                    return new object[] 
                    {
                        ((CategoryWithSingleMaterial)_targetCategory).HasChangeInMaterial,
                        null
                    };
                if (_targetCategory is CategoryWithManyMaterials)
                    return new object[]
                    {
                        ((CategoryWithSingleMaterial)_targetCategory).HasChanges,
                        null
                    };
            }

            XmlElement xmlMaterial = (XmlElement)value;

            // Если выбор материала не изменился, то возвращаемся.

            if (xmlMaterial.SelectSingleNode("name").InnerText ==
                _targetMaterial.Name)
            {
                if (_targetCategory == null)
                    return new object[] { false, null };
                if (_targetCategory is CategoryWithSingleMaterial)
                    return new object[]
                    {
                        ((CategoryWithSingleMaterial)_targetCategory).HasChangeInMaterial,
                        null
                    };
                if (_targetCategory is CategoryWithManyMaterials)
                    return new object[]
                    {
                        ((CategoryWithSingleMaterial)_targetCategory).HasChanges,
                        null
                    };
            }

            // Загрузка происходит из файла материалов.

            _targetMaterial.IsLoadedFromProject = false;
            _targetMaterial.LoadFromXml(xmlMaterial, false);

            /*
             * Если материал расширяется чем-то ещё (фундамент, например, ростверком),
             * то добавляем в него пункты сметы из материала-расширения.
             */

            string extensionPath;
            if (!string.IsNullOrEmpty(extensionPath = xmlMaterial.GetAttribute("extensible_by")))
            {
                XmlElement xmlExtension = (XmlElement)EcohouseProjectViewModel
                    .xmlMaterialsDoc.DocumentElement.SelectSingleNode(extensionPath);

                foreach (XmlNode xmlEstimateItem in xmlExtension.SelectNodes(@"est_items/est_item"))
                {
                    _targetMaterial.CreateAndAddEstimateItem( ei =>
                            ei.LoadFromXml((XmlElement)xmlEstimateItem, false));
                }
            }

            // Возвращаем изменение материала: да, был загружен новый.

            return new object[] { true, null };
        }
    }
}
