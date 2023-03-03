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
    /// Материал окна.
    /// </summary>

    class WindowMaterial : EnumerableMaterial
    {
        // Ресурс для пункта сметы-плёнки.

        public static EstimateItem EstimateItemFilmResource { get; set; }

        // Собственный пункт сметы-плёнки.

        private EstimateItem _eItemFilm;

        public EstimateItem EItemFilm
        {
            get { return _eItemFilm; }
            set
            {
                if (value != _eItemFilm)
                {
                    _eItemFilm = value;
                    OnPropertyChanged(nameof(HasFilm));
                }
            }
        }

        // Флаг наличия плёнки на кровлю.

        private bool _hasFilm;

        public bool HasFilm
        {
            get { return _hasFilm; }
            set
            {
                if (value != _hasFilm)
                {
                    _hasFilm = value;
                    OnPropertyChanged(nameof(HasFilm));
                }
            }
        }

        // Конструктор.

        public WindowMaterial(TechTaskCategory parentCategory) : base(parentCategory)
        {
            HasChangeIn.Add(nameof(HasFilm), false);

            HasFilm = false;

            /*
             * Изменение количества плёнок в зависимости от требования их наличия.
             */

            string hf = nameof(HasFilm);
            this.PropertyChanged += (s, e) =>
            {
                if (_eItemFilm != null &&
                    (e.PropertyName == hf || e.PropertyName == nameof(EItemFilm)))
                {
                    if (e.PropertyName == hf)
                        HasChangeIn[hf] = true;
                    _eItemFilm.CalculateQuantityWithFormula();
                    _eItemFilm.PhysicalQuantity *= Convert.ToInt32(HasFilm);
                }
            };
        }

        // Переопределённый метод очистки материала.

        public override void Clear()
        {
            base.Clear();
            HasFilm = false;
        }

        // Метод загрузки из хмла.

        public override void LoadFromXml(XmlElement xmlWindowMaterial,
                                         bool isLoadedFromProject)
        {
            base.LoadFromXml(xmlWindowMaterial, isLoadedFromProject);

            if (isLoadedFromProject)
            {
                EItemFilm = EstimateItems.FirstOrDefault(ei => ei.Name == "Плёнка на окна");
                HasFilm = Convert.ToBoolean(xmlWindowMaterial?.GetAttribute("has_film") ?? "false");
                this.Summary = Convert.ToDecimal(xmlWindowMaterial.SelectSingleNode("summary").InnerText);
            }
            else
            {
                EItemFilm = CreateAndAddEstimateItem((ei) =>
                {
                    EstimateItemFilmResource.CopyTo(ref ei);
                });
                HasFilm = false;
            }

            ClearChanges();
        }

        // Переопределённый метод обнуления изменений.

        public override void ClearChanges()
        {
            base.ClearChanges();
            HasChangeIn[nameof(HasFilm)] = false;
            this.HasChangeIn[HasChangeIn.Keys.Last()] = false;
        }

        // Запись в хмл.

        public override sealed XElement WriteToXmlAsXElement()
        {
            XElement xEstimateItems = new XElement("est_items");
            foreach (EstimateItem eItem in EstimateItems)
                xEstimateItems.Add(eItem.WriteToXmlAsXElement());

            XElement xMaterial = new XElement("window",
                new XAttribute("has_film", HasFilm.ToString()),
                new XElement("name") { Value = Name },
                new XElement("quantity") { Value = Quantity.ToString() },
                xEstimateItems,
                new XElement("summary") { Value = Summary.ToString() });

            return xMaterial;
        }
    }
}
