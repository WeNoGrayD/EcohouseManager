using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using System.Xml;

namespace EcohouseManager
{
    /// <summary>
    /// Конвертер, предназначенный для получения материалов при загрузке проекта.
    /// </summary>

    public class XmlToComboBoxToFromMaterialMultiConverter : IMultiValueConverter
    {
        // Конвертация в тег комбобокса.

        public object Convert(object[] values, Type targetType, object parameter, 
                              CultureInfo culture)
        {
            // Если модель проетка == null, то возвращаемся, нам тут делать нечего.
            // Если проект не должен загружаться, также возвращаемся.

            if (values[1] == null)
                return null;

            CategoryWithSingleMaterial targetCategory = (CategoryWithSingleMaterial)values[1];

            // Если хил элемента не прогрузился, то возвращаемся.

            XmlElement xmlMaterial = values[0] as XmlElement;
            if (xmlMaterial == null)
            {
                if (EcohouseProjectViewModel.ProjectMustBeLoaded)
                    targetCategory.SelectedMaterial.Clear();
                EcohouseProjectViewModel.EcoProj.CategoriesAreLoading[targetCategory.GetAbsolutePath()] = false;
                return null;
            }

            // Если файл проекта не пустой или конкретно этот материал
            // Был выбран, то загружаем его в модель.

            bool isLoaded = System.Convert.ToBoolean(((XmlElement)xmlMaterial.ParentNode).GetAttribute("is_loaded"));
            if (EcohouseProjectViewModel.ProjectMustBeLoaded && isLoaded)
            {

                // Обнуляем все изменения в категории.

                targetCategory.HasChanges = false;

                // Загрузка происходит из проекта.

                Material targetMaterial = targetCategory.SelectedMaterial;

                targetMaterial.IsLoadedFromProject = isLoaded;
                targetMaterial.LoadFromXml(xmlMaterial, isLoaded);

                /*
                 * Если материал расширяется чем-то ещё (фундамент, например, ростверком),
                 * то добавляем в него пункты сметы из материала-расширения.
                 */

                string extensionPath;
                if (!string.IsNullOrEmpty(extensionPath = xmlMaterial.GetAttribute("extensible_by")))
                {
                    XmlElement xmlExtension = (XmlElement)EcohouseProjectViewModel
                        .xmlMaterialsDoc.DocumentElement.SelectSingleNode(extensionPath);

                    foreach(XmlNode xmlEstimateItem in xmlExtension.SelectNodes(@"est_items/est_item"))
                    {
                        targetMaterial.CreateAndAddEstimateItem( ei =>
                            ei.LoadFromXml((XmlElement)xmlEstimateItem, true));
                    }
                }
            }

            // Обновление в словаре загрузок категорий данной категории статуса загрузки.

            EcohouseProjectViewModel.EcoProj.CategoriesAreLoading[targetCategory.GetAbsolutePath()] = false;

            return values[0];
        }

        // Конвертация из тега комбобокса. Не производится, поэтому не внедрена.

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, 
                                   CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
