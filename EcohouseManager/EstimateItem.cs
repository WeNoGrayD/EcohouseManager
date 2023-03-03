using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Xml;
using System.Xml.Linq;

namespace EcohouseManager
{
    // Класс, вмещающий информацию об одном материале внутри куска дома.

    public class EstimateItem : INotifyPropertyChanged, IDataErrorInfo
    {
        // Материал-владелец пункта сметы.

        public Material ParentMaterial { get; set; }

        // Поля.
        
        private decimal _price;
        private string _quantityFormula;
        private decimal _physicalQuantity;
        private decimal _markup;
        private decimal _summary;

        // Имя.

        public string Name { get; private set; }

        // Описание пункта сметы.

        public string Description { get; private set; }

        // Цена.

        public decimal Price
        {
            get { return _price; }
            set
            {
                if (value != _price)
                {
                    _price = value;
                    OnPropertyChanged(nameof(Price));

                    CalculateSummary();
                }
            }
        }

        // Допустимые варианты формулы количества.

        public Dictionary<EcohouseProjectModel.RoofType, string> PossibleQuantityFormulas;

        // Формула, по которой вычисляется количество.

        public string QuantityFormula
        {
            get { return _quantityFormula; }
            set
            {
                if (value != _quantityFormula)
                {
                    _quantityFormula = value;
                    CalculateQuantityWithFormula();
                }
            }
        }

        // Количество.

        public decimal PhysicalQuantity
        {
            get { return _physicalQuantity; }
            set
            {
                if (value != _physicalQuantity)
                {
                    _physicalQuantity = value;
                    OnPropertyChanged(nameof(PhysicalQuantity));

                    CalculateSummary(); ;
                }
            }
        }

        // Единица измерения.

        public string Measure { get; set; }

        // Наценка.

        public decimal Markup
        {
            get { return _markup * 100; }
            set
            {
                decimal bufMarkup;
                if ((bufMarkup = value / 100) != _markup)
                {
                    _markup = bufMarkup;
                    OnPropertyChanged(nameof(Markup));

                    CalculateSummary();
                }
            }
        }

        // Итоговая стоимость.

        public decimal Summary
        {
            get { return _summary; }
            set
            {
                if (value != _summary)
                {
                    _summary = value;
                    OnPropertyChanged(nameof(Summary));
                }
            }
        }

        // Словарь изменений в основных свойствах.

        public ObservableDictionary<string, bool> HasChangeIn { get; private set; }

        // Флаг, указыаающий на наличие изменений в пункте сметы.

        private bool _hasAnyChanges;

        public bool HasAnyChanges
        {
            get { return _hasAnyChanges; }
            set
            {
                if (value != _hasAnyChanges)
                {
                    _hasAnyChanges = value;
                    OnPropertyChanged(nameof(HasAnyChanges));
                }
            }
        }

        // Свойство и индексатор для валидации данных на странице расчёта.

        public string Error
		{
			get { throw new NotImplementedException(); }
		}
		
		public string this[string propName]
		{
			get
			{
				string error = string.Empty;
				switch (propName)
				{
					case "Name": break; 
					case "Price": 
					{
						if (Price < 0)
							error = "Цена меньше нуля!";
						break;
					}
					case "PhysicalQuantity": 
					{
						if (PhysicalQuantity < 0)
							error = "Количество меньше нуля!";
						break;
					}
					case "Summary": break;
				}
                return error;
			}
		}

        // Обычный конструктор.

        public EstimateItem()
        {
           PossibleQuantityFormulas = new Dictionary<EcohouseProjectModel.RoofType, string>();
           Summary = 0;

            HasChangeIn = new ObservableDictionary<string, bool>()
            {
                { nameof(Price), false },
                { nameof(PhysicalQuantity), false },
                { nameof(Markup), false }
            };
            HasChangeIn.PropertyChanged += (s, e) => 
                {
                    HasAnyChanges = HasChangeIn.Values
                        .Aggregate((hasChanges, prop) => hasChanges |= prop);
                };
        }

        // Пересчёт параметров по формулам.

        public void CalculateQuantityWithFormula()
        {
            decimal quantity = 0;
            
            if (!decimal.TryParse(QuantityFormula, out quantity))
                quantity = (decimal)BestCalculatorEver.Run(QuantityFormula);

            /*
             * Если отцовский материал является перечислимым, то 
             * следует помножить количество пункта сметы
             * на количество материала.
             */

            if (ParentMaterial is EnumerableMaterial)
                quantity *= ((EnumerableMaterial)ParentMaterial).Quantity;
            
            PhysicalQuantity = quantity;
        }

        // Подсчёт суммарной стоимости пункта сметы.

        public void CalculateSummary()
        {
            Summary = _price * _physicalQuantity * (1 + _markup);
        }

        // Загрузка из хмла.

        public void LoadFromXml(XmlElement xmlEstimateItem, bool isLoadedFromProject)
        {
            Name = xmlEstimateItem.SelectSingleNode("name").InnerText;
            Description = xmlEstimateItem["description"].InnerText;
            decimal bufPrice;
            if (!decimal.TryParse(xmlEstimateItem["price"].InnerText, out bufPrice))
                Price = 0;
            else
                Price = bufPrice;
            Markup = Convert.ToDecimal(xmlEstimateItem["markup"].InnerText);
            Measure = xmlEstimateItem["measure"].InnerText;

            PossibleQuantityFormulas.Clear();
            foreach (XmlElement possubleQuantityFormula in xmlEstimateItem.SelectNodes(@"quantity_formulas/quantity"))
            {
                string roofType = possubleQuantityFormula.GetAttribute("rooftype");
                PossibleQuantityFormulas.Add((EcohouseProjectModel.RoofType)
                                              Enum.Parse(typeof(EcohouseProjectModel.RoofType), roofType),
                                              possubleQuantityFormula.InnerText);
            }

            /*
             * Загрузка формул (и итогов в случае загрузки из проекта) пункта сметы.
             */

            decimal summary = 0;
            if (isLoadedFromProject)
                decimal.TryParse(xmlEstimateItem.SelectSingleNode("summary").InnerText, out summary);


            if (PossibleQuantityFormulas.Count == 1)
			{
                // Если материал был загружен, то формула загружаетсся вместе со значением кол-ва.

                if (isLoadedFromProject)
                {
                    _physicalQuantity = Convert.ToDecimal(xmlEstimateItem.SelectSingleNode("quantity").InnerText);
                    _quantityFormula = PossibleQuantityFormulas[EcohouseProjectModel.RoofType.None];
                    Summary = summary;
                }
                // Иначе - загружается формула и вычисляется количество.
                else
                    QuantityFormula = PossibleQuantityFormulas[EcohouseProjectModel.RoofType.None];
            }
			else
			{
                EcohouseProjectModel.RoofType selectedRoofType = EcohouseProjectViewModel
                    .EcoProj.SelectedRoofType;

                // Если материал зависим от типа крыши и в проекте был выбран тип кровли,
                // то формула загружается, иначе цена просто равняется нулю
                // до тех пор, пока не будет выбран тип крыши.

                bool roofTypeisSelected = selectedRoofType != EcohouseProjectModel.RoofType.None;
                if (isLoadedFromProject)
                {
                    _physicalQuantity = Convert.ToDecimal(xmlEstimateItem.SelectSingleNode("quantity").InnerText);
                    if (roofTypeisSelected)
                        _quantityFormula = PossibleQuantityFormulas[selectedRoofType];
                    else
                        _quantityFormula = PossibleQuantityFormulas[EcohouseProjectModel.RoofType.TwoFloors];
                    Summary = summary;

                }
                else if (roofTypeisSelected)
                    QuantityFormula = PossibleQuantityFormulas[selectedRoofType];
                else
                    QuantityFormula = PossibleQuantityFormulas[EcohouseProjectModel.RoofType.TwoFloors]; 
            }

            ClearChanges();

            return;
        }

        // Метод обновления формулы количества пункта сметы.

        public void UpdateQuantityFormula()
        {
            EcohouseProjectModel.RoofType selectedRoofType = EcohouseProjectViewModel
                .EcoProj.SelectedRoofType;
            bool roofTypeisSelected = selectedRoofType != EcohouseProjectModel.RoofType.None;
            if (roofTypeisSelected)
                QuantityFormula = PossibleQuantityFormulas[selectedRoofType];
        }

        // Обнуление изменений в списке свойств.

        public void ClearChanges()
        {
            string[] propNames = HasChangeIn.Keys.ToArray();
            foreach (string propName in propNames)
                HasChangeIn[propName] = false;
        }

        // Копирование пункта сметы.

        public void CopyTo(ref EstimateItem eItem)
        {
            eItem.Name = this.Name;
            eItem.Description = this.Description;
            eItem._price = this._price;
            eItem._quantityFormula = this._quantityFormula;
            eItem._physicalQuantity = this._physicalQuantity;
            eItem.Measure = this.Measure;
            eItem._markup = this._markup;
            eItem._summary = this._summary;

            foreach (EcohouseProjectModel.RoofType roofType in this.PossibleQuantityFormulas.Keys)
                eItem.PossibleQuantityFormulas.Add(roofType, this.PossibleQuantityFormulas[roofType]);

            return;
        }

        // Хмл-представление материала.

        public XElement WriteToXmlAsXElement()
        {
            XElement xQuantityFormulas = new XElement("quantity_formulas");
            foreach (EcohouseProjectModel.RoofType rooftypeKey in PossibleQuantityFormulas.Keys)
                xQuantityFormulas.Add(
                    new XElement("quantity", 
                        new XAttribute("rooftype", rooftypeKey.ToString()))
                        { Value = PossibleQuantityFormulas[rooftypeKey] });

            XElement xEstimateItem = new XElement("est_item");
            xEstimateItem.Add(
                new XElement("name") { Value = Name },
                new XElement("description") { Value = Description },
                new XElement("price") { Value = Price.ToString() },
                new XElement("quantity") { Value = PhysicalQuantity.ToString() },
                new XElement("measure") { Value = Measure },
                new XElement("markup") { Value = Markup.ToString() },
                new XElement("summary") { Value = Summary.ToString() },
                xQuantityFormulas
                );

            return xEstimateItem;
        }

        // Уведомление подписчиков на событие изменения свойства.

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            // Обновление списка изменений в пункте сметы.

            if (HasChangeIn.ContainsKey(prop))
                HasChangeIn[prop] = true;

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}
