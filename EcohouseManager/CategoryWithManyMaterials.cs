using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Xml.Linq;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace EcohouseManager
{
    /// <summary>
    /// Категория, влкючающая несколько материалов. 
    /// </summary>

    public class CategoryWithManyMaterials : TechTaskCategory
    {
        // Тип материала.

        public string MaterialsType { get; private set; }

        // Выбранный материал из категории материалов.

        protected ObservableCollection<Material> _selectedMaterials;

        public ObservableCollection<Material> SelectedMaterials
        {
            get { return _selectedMaterials; }
            set
            {
                if (value != _selectedMaterials)
                    _selectedMaterials = value;
            }
        }

        // Перечисление, дающее информацию о действиях с предметом логгирования
        // (материалом в списке выбранных материалов).

        public enum LogAction
        {
            Native = 0, // без изменений
            Added = 1, // было произведено добавление
            Removed = 2 // было произведено удаление
        }

        // Класс, предоставляюдий информацию о записи в журнале изменений списка материалов.

        public class MaterialLogRecord : INotifyPropertyChanged
        {
            // Ссылка на материал, о котором сделана запись.

            public Material Target { get; private set; }

            // Наличие изменений в материале отслеживается только
            // в том случае, если материал считается "родным" для списка материалов.

            private bool _hasChanged;

            public bool HasChanged
            {
                get { return _hasChanged; }
                set
                {
                    if (value != _hasChanged)
                    {
                        _hasChanged = value;
                        OnPropertyChanged(nameof(HasChanged));
                    }
                }
            }

            private bool _reloaded;

            public bool Reloaded
            {
                get { return _reloaded; }
                set
                {
                    if (value != _reloaded)
                    {
                        _reloaded = value;
                        OnPropertyChanged(nameof(Reloaded));
                    }
                }
            }

            // Действие, которому был подвергнут материал.

            public LogAction Action { get; set; }

            // конструктор.

            public MaterialLogRecord(Material target, LogAction action)
            {
                Target = target;
                Action = action;
            }

            // Уведомление подписчиков на событие изменения свойства.

            public event PropertyChangedEventHandler PropertyChanged;

            protected void OnPropertyChanged([CallerMemberName]string prop = "")
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
            }
        }

        // Журнал изменений списка материалов в категории.

        public ObservableCollection<MaterialLogRecord> MaterialsLog { get; private set; }

        // Длина списка материалов до первого его изменения.

        public int NativeMaterialsCount { get; private set; }  

        // Конструктор.

        public CategoryWithManyMaterials(string path, string materialsType) 
            : base(path, false)
        {
            MaterialsType = materialsType;

            SelectedMaterials = new ObservableCollection<Material>();
            SelectedMaterials.CollectionChanged += (s, e) =>
            {
                switch (e.Action)
                {
                    case NotifyCollectionChangedAction.Add:
                        {
                            for (int i = 0; i < e.NewItems.Count; i++)
                                MaterialsLog.Add(new MaterialLogRecord
                                    ((Material)e.NewItems[i], LogAction.Added));
                            HasChanges = true;
                            break;
                        }
                    case NotifyCollectionChangedAction.Remove:
                        {
                            for (int i = 0; i < e.OldItems.Count; i++)
                                MaterialsLog.First(record => record.Target
                                             .Equals((Material)e.OldItems[i]))
                                             .Action = LogAction.Removed;
                            HasChanges = true;
                            break;
                        }
                    case NotifyCollectionChangedAction.Reset:
                        {
                            MaterialsLog.Clear();
                            HasChanges = false;
                            break;
                        }
                }
            };

            MaterialsLog = new ObservableCollection<MaterialLogRecord>();
            NativeMaterialsCount = 0;
        }

        /* 
         * Инициализация списка изменений материалов.
         * При старте этот список пуст.
         * При сохранении изменений проекта следует удалить всю неактуальную информацию
         * о действиях со списком материалов.
         */

        public void RefreshMaterialsLog()
        {
            string hc, rld;
            for (int i = 0; i < MaterialsLog.Count; i++)
                if (MaterialsLog[i].Action == LogAction.Added)
                {
                    MaterialLogRecord record = MaterialsLog[i];
                    record.Action = LogAction.Native;
                    Material target = MaterialsLog[i].Target;

                    record.HasChanged = false;
                    record.Reloaded = false;

                    hc = nameof(record.HasChanged);
                    rld = nameof(record.Reloaded);

                    target.PropertyChanged += (s, e) =>
                    {
                        record.HasChanged = target.HasAnyChanges;
                    };

                    target.DownloadedFromXml += () =>
                    {
                        record.Reloaded = true;
                    };

                    record.PropertyChanged += (s, e) =>
                    {
                        if (e.PropertyName == hc || e.PropertyName == rld)
                            this.HasChanges = (from logRecord in MaterialsLog
                                                select logRecord.HasChanged || logRecord.Reloaded)
                                               .Aggregate((hasChanges, isChanged) =>
                                                          hasChanges |= isChanged );
                    };
                }

            List<MaterialLogRecord> removedMaterials = 
                MaterialsLog.Where(lr => lr.Action == LogAction.Removed).ToList();
            for (int i = 0; i < removedMaterials.Count; i++)
                MaterialsLog.Remove(removedMaterials[i]);

            NativeMaterialsCount = SelectedMaterials.Count;

            HasChanges = false;
        }

        // Добавление нового материала.

        public void AddNewMaterial()
        {
            Material newMaterial = MaterialFactory.CreateMaterial(this, MaterialsType);
            SelectedMaterials.Add(newMaterial);
        }

        // Очистка коллекции.

        public void ClearFromMaterials()
        {
            SelectedMaterials.Clear();
        }

        // Обнуление изменений в категории.

        public override void ClearChanges()
        {
            foreach (Material material in SelectedMaterials)
                material.ClearChanges();
            this.RefreshMaterialsLog();
        }

        // Десериализация категории (запись в хмл).

        public override XElement WriteToXml()
        {
            XElement xCategory = new XElement(PathInProject,
                new XAttribute("name", Name),
                new XAttribute("contain_materials", true),
                new XAttribute("container_type", "single"),
                new XAttribute("materials_type", MaterialsType));

            IEnumerable<XElement> xsMaterials = from material in SelectedMaterials
                                                select material.WriteToXmlAsXElement();

            foreach (XElement xMaterial in xsMaterials)
                xCategory.Add(xMaterial);

            return xCategory;
        }
    }
}
