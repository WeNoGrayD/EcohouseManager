using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Xml;
using System.Xml.Linq;
using System.Collections.ObjectModel;

namespace EcohouseManager
{
    /// <summary>
    /// Модель материала для основной части программы. Применяется в модели EcohouseProjectModel.
    /// </summary>

    public class Material : INotifyPropertyChanged
    {
        // Родительская категория.

        public TechTaskCategory ParentCategory { get; set; }

        // Флаг: загружен ли материал из проекта (true) или из материалов (false).

        public bool IsLoadedFromProject { get; set; } 

        // Название материала.

        protected string _name;

        public string Name
        {
            get { return _name; }
            protected set
            {
                if (value != _name)
                {
                    _name = value;
                    OnPropertyChanged("Name");
                }
            }
        }

        // Список пунктов сметы.

        public ObservableCollection<EstimateItem> EstimateItems { get; private set; }

        // Итоги по материалу.

        private decimal _summary { get; set; }

        public decimal Summary
        {
            get { return _summary; }
            set
            {
                _summary = value;
                OnPropertyChanged("Summary");
            }
        }

        // Словарь изменений в пунктах сметы.

        public ObservableDictionary<string, bool> HasChangeIn { get; protected set; }

        // Флаг, указыаающий на наличие изменений в пункте сметы.

        protected bool _hasAnyChanges;

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

        // Флаг, сообщающий о том, что материал находится в категории,
        // относящейся к этажам дома.

        private bool _isEItemsFormulasNeedBeProcessed;

        // Заменяемые переменные.

        private string[] _replacedVars = null;

        // Событие загрузки материала из хмла.

        public event Action DownloadedFromXml;

        // Конструктор.

        public Material(TechTaskCategory parentCategory)
        {
            ParentCategory = parentCategory;

            EstimateItems = new ObservableCollection<EstimateItem>();

            HasChangeIn = new ObservableDictionary<string, bool>();
            HasChangeIn.PropertyChanged += (s, e) =>
            {
                HasAnyChanges = HasChangeIn.Values
                    .Aggregate((hasChanges, prop) => hasChanges |= prop);
            };

            // Замена "особых значений" переменных для категории-этажа.

            System.Text.RegularExpressions.Regex reFloor =
                new System.Text.RegularExpressions.Regex(@"floor\d");
            System.Text.RegularExpressions.Match mtFloor =
                reFloor.Match(ParentCategory.GetAbsolutePath());
            bool didCategoryIsFloor = string.IsNullOrEmpty(mtFloor.Value) ? false : true;
                 _isEItemsFormulasNeedBeProcessed = didCategoryIsFloor && 
                                                   ParentCategory.DependsOnRoofType;
            if (_isEItemsFormulasNeedBeProcessed)
            {
                short floorNumber = (short)char.GetNumericValue(mtFloor.Value.Last());
                if (floorNumber == 1)
                {
                    _replacedVars = new string[4]
                    {
                                "floor1_height",
                                "floor1_bearing_walls_molding",
                                "floor1_curtain_walls_molding",
                                "floor1_external_walls_molding"
                    };
                }
                else
                {
                    _replacedVars = new string[4]
                    {
                                "floor2_height",
                                "floor2_bearing_walls_molding",
                                "floor2_curtain_walls_molding",
                                "floor2_external_walls_molding"
                    };
                }
            }
        }

        // Очищение материала.

        public virtual void Clear()
        {
            Name = null;

            // Необходимо отвязать слежку за изменениями старых пунктов сметы.

            foreach (EstimateItem ei in EstimateItems)
                HasChangeIn.Remove(ei.Name);

            // Очистка списка пунктов сметы.

            EstimateItems.Clear();

            // Пеобходимо отметить, что чистый материал означает отсутствие изменений.

            _hasAnyChanges = false;

            // Итоги тоже надо обнулить.

            Summary = 0;
        }

        // Загрузка из хмла.

        public virtual void LoadFromXml(XmlElement xmlMaterial, 
								        bool isLoadedFromProject)
        {
            this.Clear();

            Name = xmlMaterial.SelectSingleNode("name").InnerText;
			IsLoadedFromProject = isLoadedFromProject;

            // При загрузке материала необходимо отметить, что
            // новый материал означает отсутствие изменений.

            _hasAnyChanges = false;

            XmlNodeList xmlsEstimateItems = xmlMaterial.SelectNodes("est_items/est_item");
            EstimateItem eItem;

            foreach (XmlNode xmlEstimateItem in xmlsEstimateItems)
            {
                eItem = CreateAndAddEstimateItem( ei =>
                    ei.LoadFromXml((XmlElement)xmlEstimateItem, isLoadedFromProject)
                );
            }

            if (isLoadedFromProject)
                this.Summary = Convert.ToDecimal(xmlMaterial.SelectSingleNode("summary").InnerText);

            DownloadedFromXml?.Invoke();

            ClearChanges();
        }

        // Создание пункта сметы со всеми привязками.

        public EstimateItem CreateAndAddEstimateItem(Action<EstimateItem> loadEItemAction)
        {
            EstimateItem eItem = new EstimateItem() { ParentMaterial = this };
            EstimateItems.Add(eItem);

            string smr = nameof(eItem.Summary);
            eItem.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == smr)
                    this.Summary = EstimateItems.Sum(ei => ei.Summary);
            };

            // Непосредственно загрузка пункта сметы.

            loadEItemAction(eItem);

            HasChangeIn.Add(eItem.Name, false);
            string hac = nameof(eItem.HasAnyChanges);
            eItem.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == hac)
                    HasChangeIn[((EstimateItem)s).Name] = ((EstimateItem)s).HasAnyChanges;
            };

            // Замена формул, связанных с этажами.

            if (_isEItemsFormulasNeedBeProcessed)
            {
                EcohouseProjectModel.RoofType[] roofTypes =
                    eItem.PossibleQuantityFormulas.Keys.ToArray();
                string[] formulas = eItem.PossibleQuantityFormulas.Values.ToArray();
                string formula;
                StringBuilder sbFormula;
                foreach (EcohouseProjectModel.RoofType roofType in roofTypes)
                {
                    formula = eItem.PossibleQuantityFormulas[roofType];
                    sbFormula = new StringBuilder(formula);
                    sbFormula.Replace("высота_этажа", _replacedVars[0]);
                    sbFormula.Replace("этаж_пм_несущ_стен", _replacedVars[1]);
                    sbFormula.Replace("этаж_пм_ненесущ_стен", _replacedVars[2]);
                    sbFormula.Replace("этаж_пм_внеш_стен", _replacedVars[3]);
                    eItem.PossibleQuantityFormulas[roofType] = sbFormula.ToString();
                }

                eItem.UpdateQuantityFormula();

                eItem.ClearChanges();
            }

            return eItem;
        }

        // Обнуление изменений в материале.

        public virtual void ClearChanges()
        {
            HasAnyChanges = false;
            foreach (EstimateItem eItem in EstimateItems)
                eItem.ClearChanges();
        }

        // Запись в хмл.

        public virtual XElement WriteToXmlAsXElement()
        {
            XElement xEstimateItems = new XElement("est_items");
            foreach (EstimateItem eItem in EstimateItems)
                xEstimateItems.Add(eItem.WriteToXmlAsXElement());

            XElement xMaterial = new XElement("material",
                new XElement("name") { Value = Name },
                xEstimateItems,
                new XElement("summary") { Value = Summary.ToString() });

            return xMaterial;
        }

        // Добавление 

        // Уведомление подписчиков на событие изменения свойства.

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}
