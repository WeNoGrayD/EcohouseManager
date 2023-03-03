using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace EcohouseManager
{
    /// <summary>
    /// Класс, представляющий категорию материалов в проекте.
    /// </summary>

    public class TechTaskCategory
    {
        // Название категории.

        public string Name { get; set; }

        // Путь к связанному тегу в хмле проекта.

        public string PathInProject { get; protected set; }

		// Родительская категория.
		
		public TechTaskCategory Parent { get; set; }

        // Зависимость от типа крыши.

        public bool DependsOnRoofType { get; set; }

        // Словарь подкатегорий у категории-контейнера.

        public Dictionary<string, TechTaskCategory> SubCategories { get; protected set; }

        // Флаг совершения изменений в категории.
        // Сделан общим для всех категорий, но используется только
        // у категорий с материалами.

        protected bool _hasChanges;

        public bool HasChanges
        {
            get { return _hasChanges; }
            set
            {
                if (value != _hasChanges)
                {
                    _hasChanges = value;
                    OnPropertyChanged(nameof(HasChanges));
                }
            }
        }

        // Конструктор.

        public TechTaskCategory(string path, bool isContainer)
        {
            HasChanges = false;
            PathInProject = path;
            if (isContainer)
                SubCategories = new Dictionary<string, TechTaskCategory>();
        }

        // Получение абсолютного пути категории.

        public string GetAbsolutePath()
        {
            return Parent.PathInProject != "root" ? Parent.GetAbsolutePath() + "/" + PathInProject :
                                                    PathInProject;
        }

        // Обнуление изменений в категории. Не определён.

        public virtual void ClearChanges() { throw new NotImplementedException(); }

        // Десериализация категории (запись в хмл).

        public virtual XElement WriteToXml()
        {
            XElement xCategory = new XElement(PathInProject,
                new XAttribute("name", Name),
                new XAttribute("contain_materials", false));

            IEnumerable<XElement> xsSubCategories = from subCategory in SubCategories
                                                   select subCategory.Value.WriteToXml();

            foreach (XElement xSubCategory in xsSubCategories)
                xCategory.Add(xSubCategory);

            return xCategory;
        }

        // Уведомление подписчиков на событие изменения свойства.

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}
