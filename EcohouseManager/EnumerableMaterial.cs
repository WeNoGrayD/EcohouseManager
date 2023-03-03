using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace EcohouseManager
{
    /// <summary>
    /// Исчислимый вид материалов.
    /// </summary>

    public class EnumerableMaterial : Material
    {
        // Количество материала.

        private decimal _quantity;

        public decimal Quantity
        {
            get { return _quantity; }
            set
            {
                if (value != _quantity)
                {
                    // Изменение количества дочерних пунктов сметы.

                    decimal coef = _quantity != 0 ? (decimal)value / (decimal)_quantity : value;
                    foreach (EstimateItem eItem in EstimateItems)
                        eItem.PhysicalQuantity *= coef;

                    _quantity = value;
                    OnPropertyChanged(nameof(Quantity));
                }
            }
        }

        // Конструктор.

        public EnumerableMaterial(TechTaskCategory parentCategory) : base(parentCategory)
        {
            Quantity = 0;

            // Подписка на изменение своего количества.

            string quantity = nameof(Quantity);
            HasChangeIn.Add(quantity, false);
            this.PropertyChanged += (s, e) =>
                {
                    if (e.PropertyName == quantity)
                    {
                        HasChangeIn[quantity] = true;
                        foreach (EstimateItem eItem in EstimateItems)
                            eItem.CalculateQuantityWithFormula();
                    }
                };
        }

        // Переопределённый метод очистки материала.

        public override void Clear()
        {
            base.Clear();
            Quantity = 0;
        }

        // Метод загрузки из хмла.

        public override void LoadFromXml(XmlElement xmlEnumMaterial, 
                                         bool isLoadedFromProject)
        {
            base.LoadFromXml(xmlEnumMaterial, isLoadedFromProject);

            if (isLoadedFromProject)
            {
                Quantity = Convert.ToDecimal(xmlEnumMaterial.SelectSingleNode("quantity").InnerText);
                this.Summary = Convert.ToDecimal(xmlEnumMaterial.SelectSingleNode("summary").InnerText);
            }
            else
                Quantity = 0;

            ClearChanges();
        }

        // Переопределённый метод обнуления изменений.

        public override void ClearChanges()
        {
            base.ClearChanges();
            HasChangeIn[nameof(Quantity)] = false;
        }

        // Запись в хмл.

        public override XElement WriteToXmlAsXElement()
        {
            XElement xEstimateItems = new XElement("est_items");
            foreach (EstimateItem eItem in EstimateItems)
                xEstimateItems.Add(eItem.WriteToXmlAsXElement());

            XElement xMaterial = new XElement("enum_material",
                new XElement("name") { Value = Name },
                new XElement("quantity") { Value = Quantity.ToString() },
                xEstimateItems,
                new XElement("summary") { Value = Summary.ToString() });

            return xMaterial;
        }
    }
}
