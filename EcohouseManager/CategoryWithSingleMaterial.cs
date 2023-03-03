using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace EcohouseManager
{
    /// <summary>
    /// Категория, содержащая только один материал (~99% всех категорий).
    /// </summary>

    public class CategoryWithSingleMaterial: TechTaskCategory
    {
        // Тип материала.

        public string MaterialType { get; private set; }

        // Выбранный материал из категории материалов.

        protected Material _selectedMaterial;

        public Material SelectedMaterial
        {
            get { return _selectedMaterial; }
            set
            {
                if (value != _selectedMaterial)
                {
                    _selectedMaterial = value;
                    HasChanges = true;
                }
            }
        }

        // Флаг совершения изменений в выбранном материале внутри.

        protected bool _hasChangeInMaterial;

        public bool HasChangeInMaterial
        {
            get { return _hasChangeInMaterial; }
            set
            {
                if (value != _hasChangeInMaterial)
                {
                    _hasChangeInMaterial = value;
                    HasChanges = value || MaterialWasReloaded;
                    // не нужно OnPropertyChanged(nameof(HasChangeInMaterial));
                }
            }
        }

        // Флаг, оповещающий об изменении выбора материала.

        protected bool _materialWasReloaded;

        public bool MaterialWasReloaded
        {
            get { return _materialWasReloaded; }
            set
            {
                if (value != _materialWasReloaded)
                {
                    _materialWasReloaded = value;
                    HasChanges = value || HasChangeInMaterial;
                    // не нужно OnPropertyChanged(nameof(HasChangeInMaterial));
                }
            }
        }

        // Конструктор.

        public CategoryWithSingleMaterial(string path, string materialType) 
            : base(path, false)
        {
            MaterialType = materialType;
        }

        // Создание материала. 

        public void CreateMaterial()
        {
            SelectedMaterial = MaterialFactory.CreateMaterial(this, MaterialType);
        }

        // Обнуление изменений в категории.

        public override void ClearChanges()
        {
            SelectedMaterial.ClearChanges();
            this.HasChangeInMaterial = false;
            this.MaterialWasReloaded = false;
        }
    }
}
