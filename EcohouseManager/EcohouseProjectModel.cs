using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Xml;
using System.Xml.Linq;

namespace EcohouseManager
{
    // Модель данных, представляющая проект.

    public class EcohouseProjectModel : INotifyPropertyChanged, IDataErrorInfo
    {
        // Шапочные параметры для расчёта, но для удобного предоставления их калькулятору.

        public static ObservableDictionary<string, string> Metadata { get; private set; }

        // Метаинформация о проекте.

        private string _customer;

        public string Customer
        {
            get { return _customer; }
            set
            {
                if (value != _customer)
                {
                    _customer = value;
                    OnPropertyChanged(nameof(Customer));
                    Metadata["customer"] = value;
                }
            }
        }

        private string _phone;

        public string Phone
        {
            get { return _phone; }
            set
            {
                if (value != _phone)
                {
                    _phone = value;
                    OnPropertyChanged(nameof(Phone));
                    Metadata["phone"] = value;
                }
            }
        }

        private string _location;

        public string Location
        {
            get { return _location; }
            set
            {
                if (value != _location)
                {
                    _location = value;
                    OnPropertyChanged(nameof(Location));
                    Metadata["location"] = value;
                }
            }
        }

        private decimal _distance;

        public decimal Distance
        {
            get { return _distance; }
            set
            {
                if (value != _distance)
                {
                    _distance = value;
                    OnPropertyChanged(nameof(Distance));
                    Metadata["distance"] = value.ToString();
                }
            }
        }

        private decimal _area;

        public decimal Area
        {
            get { return _area; }
            set
            {
                if (value != _area)
                {
                    _area = value;
                    OnPropertyChanged(nameof(Area));
                    Metadata["area"] = value.ToString();
                }
            }
        }

        // Шапочные параметры для расчёта, но для удобного предоставления их калькулятору.

        public static ObservableDictionary<string, decimal> CalculationHeaderParams { get; private set; } 
        
        // Поля, представляющие параметры шапки на странице расчёта.

        private decimal _houseLength;                // длина дома
        private decimal _houseWidth;                 // ширина дома
        private decimal _grillageMolding;            // погонаж ростверка
        private decimal _floor1Height;               // высота 1-го этажа
        private decimal _floor1BearingWallsMolding;  // погонаж несущих стен 1-го этажа
        private decimal _floor1CurtainWallsMolding;  // погонаж ненесущих стен 1-го этажа
        private decimal _floor1ExternalWallsMolding; // погонаж внешних стен 1-го этажа
        private decimal _floor2Height;               // высота 2-го этажа
        private decimal _floor2BearingWallsMolding;  // погонаж несущих стен 2-го этажа
        private decimal _floor2CurtainWallsMolding;  // погонаж ненесущих стен 2-го этажа
        private decimal _floor2ExternalWallsMolding; // погонаж внешних стен 2-го этажа
        private decimal _ridgeLength;                // длина конька
        private decimal _slopeLength;                // длина ската
        private decimal _slopeWidth;                 // ширина ската

        // Параметры шапки на странице расчёта.


        public decimal HouseLength
        {
            get { return _houseLength; }
            set
            {
                if (value != _houseLength)
                {
                    _houseLength = value;
                    OnPropertyChanged("HouseLength");
                    CalculationHeaderParams["house_length"] = value;
                }
            }
        }

        public decimal HouseWidth
        {
            get { return _houseWidth; }
            set
            {
                if (value != _houseWidth)
                {
                    _houseWidth = value;
                    OnPropertyChanged("HouseWidth");
                    CalculationHeaderParams["house_width"] = value;
                }
            }
        }

        public decimal GrillageMolding
        {
            get { return _grillageMolding; }
            set
            {
                if (value != _grillageMolding)
                {
                    _grillageMolding = value;
                    OnPropertyChanged("GrillageMolding");
                    CalculationHeaderParams["grillage_molding"] = value;
                }
            }
        }

        public decimal Floor1Height
        {
            get { return _floor1Height; }
            set
            {
                if (value != _floor1Height)
                {
                    _floor1Height = value;
                    OnPropertyChanged("Floor1Height");
                    CalculationHeaderParams["floor1_height"] = value;
                }
            }
        }

        public decimal Floor1BearingWallsMolding
        {
            get { return _floor1BearingWallsMolding; }
            set
            {
                if (value != _floor1BearingWallsMolding)
                {
                    _floor1BearingWallsMolding = value;
                    OnPropertyChanged("Floor1BearingWallsMolding");
                    CalculationHeaderParams["floor1_bearing_walls_molding"] = value;
                }
            }
        }

        public decimal Floor1CurtainWallsMolding
        {
            get { return _floor1CurtainWallsMolding; }
            set
            {
                if (value != _floor1CurtainWallsMolding)
                {
                    _floor1CurtainWallsMolding = value;
                    OnPropertyChanged("Floor1CurtainWallsMolding");
                    CalculationHeaderParams["floor1_curtain_walls_molding"] = value;
                }
            }
        }

        public decimal Floor1ExternalWallsMolding
        {
            get { return _floor1ExternalWallsMolding; }
            set
            {
                if (value != _floor1ExternalWallsMolding)
                {
                    _floor1ExternalWallsMolding = value;
                    OnPropertyChanged("Floor1ExternalWallsMolding");
                    CalculationHeaderParams["floor1_external_walls_molding"] = value;
                }
            }
        }

        public decimal Floor2Height
        {
            get { return _floor2Height; }
            set
            {
                if (value != _floor2Height)
                {
                    _floor2Height = value;
                    OnPropertyChanged("Floor2Height");
                    CalculationHeaderParams["floor2_height"] = value;
                }
            }
        }

        public decimal Floor2BearingWallsMolding
        {
            get { return _floor2BearingWallsMolding; }
            set
            {
                if (value != _floor2BearingWallsMolding)
                {
                    _floor2BearingWallsMolding = value;
                    OnPropertyChanged("Floor2BearingWallsMolding");
                    CalculationHeaderParams["floor2_bearing_walls_molding"] = value;
                }
            }
        }

        public decimal Floor2CurtainWallsMolding
        {
            get { return _floor2CurtainWallsMolding; }
            set
            {
                if (value != _floor2CurtainWallsMolding)
                {
                    _floor2CurtainWallsMolding = value;
                    OnPropertyChanged("Floor2CurtainWallsMolding");
                    CalculationHeaderParams["floor2_curtain_walls_molding"] = value;
                }
            }
        }

        public decimal Floor2ExternalWallsMolding
        {
            get { return _floor2ExternalWallsMolding; }
            set
            {
                if (value != _floor2ExternalWallsMolding)
                {
                    _floor2ExternalWallsMolding = value;
                    OnPropertyChanged("Floor2ExternalWallsMolding");
                    CalculationHeaderParams["floor2_external_walls_molding"] = value;
                }
            }
        }

        public decimal RidgeLength
        {
            get { return _ridgeLength; }
            set
            {
                if (value != _ridgeLength)
                {
                    _ridgeLength = value;
                    OnPropertyChanged("RidgeLength");
                    CalculationHeaderParams["ridge_length"] = value;
                }
            }
        }

        public decimal SlopeLength
        {
            get { return _slopeLength; }
            set
            {
                if (value != _slopeLength)
                {
                    _slopeLength = value;
                    OnPropertyChanged("slopeLength");
                    CalculationHeaderParams["slope_length"] = value;

                }
            }
        }

        public decimal SlopeWidth
        {
            get { return _slopeWidth; }
            set
            {
                if (value != _slopeWidth)
                {
                    _slopeWidth = value;
                    OnPropertyChanged("SlopeWidth");
                    CalculationHeaderParams["slope_width"] = value;

                }
            }
        }

        // Типы крыши.

        public enum RoofType
		{
            None,       // Не выбран
			TwoFloors,  // Два этажа
			FourSlopes, // Четыре ската
            RoofLorry,  // Полуторка
            RidgeRoof   // От конька
		}

        // Выбранный тип крыши.

        private RoofType _selectedRoofType;

        public RoofType SelectedRoofType
        {
            get { return _selectedRoofType; }
            set
            {
                if (value != _selectedRoofType)
                {
                    _selectedRoofType = value;
                    OnPropertyChanged("SelectedRoofType");

                    if (value != RoofType.FourSlopes)
                    {
                        //RidgeLength = SlopeWidth;
                    }
                }
            }
        }

        // Категории материалов.

        public Dictionary<string, TechTaskCategory> Categories { get; private set; }

        // Флаги окончания загрузки материалов при загрузке проекта.

        public Dictionary<string, bool> CategoriesAreLoading { get; set; }

        // Флаг изменений в проекте.

        private bool _hasAnyChange;

        public bool HasAnyChange
        {
            get { return _hasAnyChange; }
            set
            {
                if (value != _hasAnyChange)
                {
                    _hasAnyChange = value;
                    OnPropertyChanged(nameof(HasAnyChange));
                }
            }
        }

        // Словарь флагов изменений в переменных расчёта.

        public ObservableDictionary<string, bool> HasChangesInParams { get; private set; }

        /* 
         * Словарь флагов изменений в переменных расчёта.
         * Используется для более быстрого поиска категорий с изменениями.
         */

        public Dictionary<string, bool> HasChangesInModel { get; private set; }

        // Свойство и индексатор для валидации данных.

        public string Error
		{
			get { return "Ошибочка вышла!"; }
		}
		
		public string this[string propName]
		{
			get
			{
				string error = string.Empty;
				switch (propName)
				{
					case "HouseLength":
                    case "HouseWidth":
                    case "Floor1Height":  
					case "Floor1BearingWallsMolding":
                    case "Floor1CurtainWallsMolding":
                    case "Floor1ExternalWallsMolding":
                    case "Floor2Height":
                    case "Floor2BearingWallsMolding":
                    case "Floor2CurtainWallsMolding":
                    case "Floor2ExternalWallsMolding":
                    case "RidgeLength":
                    case "SlopeLength":
                    case "SlopeWidth":
                    {
						error = "Параметр меньше нуля!";
						break;
					}
				}
                return error;
			}
		}

		// Конструктор.

        public EcohouseProjectModel()
        {
            Metadata = new ObservableDictionary<string, string>()
            {
                { "customer", "" },
                { "phone", "" },
                { "location", "" },
                { "distance", "" },
                { "area", "" }
            };

            // Обработка изменений метаданных.

            Metadata.PropertyChanged += (s, e) =>
            {
                string metaInfo = Metadata[e.PropertyName]; ;
                switch (e.PropertyName)
                {
                    case "customer":
                        { this.Customer = metaInfo; break; }
                    case "phone":
                        { this.Phone = metaInfo; break; }
                    case "location":
                        { this.Location = metaInfo; break; }
                    case "distance":
                        {
                            if (metaInfo == "")
                                this.Distance = 0;
                            else
                                this.Distance = Convert.ToDecimal(metaInfo); break;
                        }
                    case "area":
                        {
                            if (metaInfo == "")
                                this.Area = 0;
                            else
                                this.Area = Convert.ToDecimal(metaInfo); break;
                        }
                }
            };

            CalculationHeaderParams = new ObservableDictionary<string, decimal>()
            {
                { "house_length", 0 },
                { "house_width", 0 },
                { "grillage_molding", 0 },
                { "floor1_height", 0 },
                { "floor1_bearing_walls_molding", 0 },
                { "floor1_curtain_walls_molding", 0 },
                { "floor1_external_walls_molding", 0 },
                { "floor2_height", 0 },
                { "floor2_bearing_walls_molding", 0 },
                { "floor2_curtain_walls_molding", 0 },
                { "floor2_external_walls_molding", 0 },
                { "ridge_length", 0 },
                { "slope_length", 0 },
                { "slope_width", 0 }
            };

            // Изменения параметров расчёта должны отслеживаться.

            HasChangesInParams = new ObservableDictionary<string, bool>();
            foreach (string param in CalculationHeaderParams.Keys)
                HasChangesInParams.Add(param, false);

            // Обработка изменений шапочных параметров.

            CalculationHeaderParams.PropertyChanged += (s, e) =>
            {
                HasChangesInParams[e.PropertyName] = true;

                decimal paramNewValue = CalculationHeaderParams[e.PropertyName]; ;
                switch (e.PropertyName)
                {
                    case "house_length":
                        { this.HouseLength = paramNewValue; break; }
                    case "house_width":
                        { this.HouseWidth = paramNewValue; break; }
                    case "grillage_molding":
                        { this.GrillageMolding = paramNewValue; break; }
                    case "floor1_height":
                        { this.Floor1Height = paramNewValue; break; }
                    case "floor1_bearing_walls_molding":
                        { this.Floor1BearingWallsMolding = paramNewValue; break; }
                    case "floor1_curtain_walls_molding":
                        { this.Floor1CurtainWallsMolding = paramNewValue; break; }
                    case "floor1_external_walls_molding":
                        { this.Floor1ExternalWallsMolding = paramNewValue; break; }
                    case "floor2_height":
                        { this.Floor2Height = paramNewValue; break; }
                    case "floor2_bearing_walls_molding":
                        { this.Floor2BearingWallsMolding = paramNewValue; break; }
                    case "floor2_curtain_walls_molding":
                        { this.Floor2CurtainWallsMolding = paramNewValue; break; }
                    case "floor2_external_walls_molding":
                        { this.Floor2ExternalWallsMolding = paramNewValue; break; }
                    case "ridge_length":
                        { this.RidgeLength = paramNewValue; break; }
                    case "slope_length":
                        { this.SlopeLength = paramNewValue; break; }
                    case "slope_width":
                        { this.SlopeWidth = paramNewValue; break; }
                }
            };

            // Обработка изменений словаря изменений шапочных параметров.

            HasChangesInParams.PropertyChanged += (s, e) =>
            {
                this.HasAnyChange = HasChangesInParams.Values
                            .Aggregate((res, isChanged) => res |= isChanged) ||
                                     HasChangesInModel.Values
                            .Aggregate((res, IsChanged) => res |= IsChanged);
            };

            XmlDocument xmlMaterialDoc = EcohouseProjectViewModel.xmlMaterialsDoc,
                        xmlProjectDoc = EcohouseProjectViewModel.xmlProjectDoc;

            Categories = new Dictionary<string, TechTaskCategory>();
            CategoriesAreLoading = new Dictionary<string, bool>();

            TechTaskCategory rootCategory = new TechTaskCategory("root", true);
                
            foreach (XmlNode xmlSuperCategory in xmlProjectDoc.DocumentElement.SelectNodes("house/*"))
			{
				TechTaskCategory subCategory = CategoryFactory
                            .CreateCategory((XmlElement)xmlSuperCategory, rootCategory);
				Categories.Add(xmlSuperCategory.Name, subCategory);
            }

            foreach (KeyValuePair<string, TechTaskCategory> categoryEntry in Categories)
                rootCategory.SubCategories.Append(categoryEntry);

            // Создание ресурса - пункта сметы-плёнки для окон.

            EstimateItem eItemFilm = new EstimateItem();
            eItemFilm.LoadFromXml((XmlElement)xmlMaterialDoc
                                  .DocumentElement.SelectSingleNode("resources/window_film/est_items/est_item"), 
                                  false);
            WindowMaterial.EstimateItemFilmResource = eItemFilm;

            /* 
             * Необходимо установить "наличие изменений" всех категорий-листьев
             * в отсутствие, потому что изначально устанавливаются в наличие,
             * что исправлять - лишняя морока.
             */

            /*
             * Также необходимо подписать все материалы крышезависимых категория
             * на изменение типа кровли.
             */

            HasChangesInModel = new Dictionary<string, bool>();
            string hc;
            foreach (TechTaskCategory category in FindAllLeafs())
            {
                category.HasChanges = false;

                string catPath = category.GetAbsolutePath();

                // Добавление в словарь загрузок категорий.

                CategoriesAreLoading.Add(catPath, false);

                // Добавление в словарь изменений категорий.

                HasChangesInModel.Add(catPath, false);

                // Подписка на изменения категорий (выбранного материала)
                // и изменения материалов (и, соответственно, их пунктов смет).

                hc = nameof(category.HasChanges);
                category.PropertyChanged += (s, e) =>
                {
                    if (e.PropertyName == hc)
                    {
                        HasChangesInModel[catPath] = category.HasChanges;
                        this.HasAnyChange = HasChangesInParams.Values
                            .Aggregate((res, isChanged) => res |= isChanged) ||
                                             HasChangesInModel.Values
                            .Aggregate((res, IsChanged) => res |= IsChanged);
                    }
                };
            }

            /* Обработчик изменения параметров модели. */

            this.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(SelectedRoofType))
                {
                    foreach (TechTaskCategory category in FindAllLeafs())
                    {
                        if (category is CategoryWithSingleMaterial && category.DependsOnRoofType)
                        {
                            foreach (EstimateItem eItem in ((CategoryWithSingleMaterial)category)
                                                           .SelectedMaterial.EstimateItems)
                                eItem.UpdateQuantityFormula();
                        }
                        else if (category is CategoryWithManyMaterials && category.DependsOnRoofType)
                        {
                            foreach (Material material in ((CategoryWithManyMaterials)category)
                                                          .SelectedMaterials)
                                foreach (EstimateItem eItem in material.EstimateItems)
                                    eItem.UpdateQuantityFormula();
                        }

                    }
                }
            };

            /* Обработчик изменения шапочных параметров. */

            EcohouseProjectModel.CalculationHeaderParams.PropertyChanged += (s, e) =>
            {
                foreach (TechTaskCategory category in FindAllLeafs())
                {
                    if (category is CategoryWithSingleMaterial && category.DependsOnRoofType)
                    {
                        foreach (EstimateItem eItem in ((CategoryWithSingleMaterial)category)
                                                       .SelectedMaterial.EstimateItems)
                            eItem.CalculateQuantityWithFormula();
                    }
                    else if (category is CategoryWithManyMaterials && category.DependsOnRoofType)
                    {
                        foreach (Material material in ((CategoryWithManyMaterials)category)
                                                      .SelectedMaterials)
                            foreach (EstimateItem eItem in material.EstimateItems)
                                eItem.CalculateQuantityWithFormula();
                    }

                }
            };
        }

        // Поиск нужной категории по пути категории в Project.xml.

        public TechTaskCategory FindCategory(string path)
        {
            string[] splittedPath = path.Split('/');
            TechTaskCategory superCategory = Categories.First
                (cat => cat.Value.PathInProject == splittedPath[0]).Value;
            foreach (string subPath in splittedPath.Skip(1))
            {
                superCategory = superCategory.SubCategories
                    .First(subCat => subCat.Value.PathInProject == subPath).Value;
            }
            return superCategory;
        }

        // Поиск все категорий-листьев.

        public IEnumerable<TechTaskCategory> FindAllLeafs()
        {
            List<TechTaskCategory> leafs = new List<TechTaskCategory>();

            foreach (TechTaskCategory category in Categories.Values)
                leafs.AddRange(SearchLeafs(category));

            return leafs;

            List<TechTaskCategory> SearchLeafs(TechTaskCategory superCategory)
            {
                List<TechTaskCategory> currentLeafs = new List<TechTaskCategory>();

                if (superCategory.SubCategories != null)
                {
                    foreach (TechTaskCategory subCategory in superCategory.SubCategories.Values)
                        currentLeafs.AddRange(SearchLeafs(subCategory));
                }
                else
                    currentLeafs.Add(superCategory);

                return currentLeafs;
            }
        }

        // Обнуление зафиксированных изменений во всех категориях и расчётных параметрах.

        public void ClearChanges()
        {
            foreach (TechTaskCategory category in FindAllLeafs())
                category.ClearChanges();

            foreach (string paramName in CalculationHeaderParams.Keys)
                HasChangesInParams[paramName] = false;
        }

        // Загрузка из хмла.

        /*
    public static EcohouseProjectModel LoadFromXml(XmlNode xmlProj)
    {
        string name, priceFormula, quantityFormula;
        double price = 0,
               quantity = 0;

        name = xmlMaterial["name"].InnerText;
        priceFormula = xmlMaterial["price"].InnerText;
        quantityFormula = xmlMaterial["quantity"].InnerText;

        if (!double.TryParse(priceFormula, out price))
            price = BestCalculatorEver.Run(priceFormula, null);
        if (!double.TryParse(quantityFormula, out quantity))
            quantity = BestCalculatorEver.Run(quantityFormula, null);

        return new MaterialParams()
            {
                PriceFormula = priceFormula,
                Price = price,
                QuantityFormula = quantityFormula,
                PhysicalQuantity = quantity
            };
    }
    */


        // Уведомление подписчиков на событие изменения свойства.

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}
