using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace EcohouseManager
{
    /// <summary>
    /// Фабрика категорий.
    /// </summary>

    public class CategoryFactory
    {
        public static TechTaskCategory CreateCategory(XmlElement xmlParentCategory,
            TechTaskCategory parent)
        {
            TechTaskCategory parentCategory = null;

            string parentCategoryPath = xmlParentCategory.Name;
            bool containMaterials = Convert.ToBoolean(((XmlElement)xmlParentCategory)
                                            .GetAttribute("contain_materials"));
            if (!containMaterials)
            {
                parentCategory = new TechTaskCategory(parentCategoryPath, true) { Parent = parent };

                XmlNodeList xmlChildrenCategories = xmlParentCategory.SelectNodes("*");
                TechTaskCategory childCategory;

                foreach (XmlNode xmlChildCategory in xmlChildrenCategories)
                {
                    childCategory = CreateCategory((XmlElement)xmlChildCategory, parentCategory);
                    parentCategory.SubCategories.Add(xmlChildCategory.Name, childCategory);
                }
            }
            else
            {
                string materialType = xmlParentCategory.GetAttribute("materials_type");

                string dependsAttr = xmlParentCategory.GetAttribute("depends_on_rooftype");
                bool depends = string.IsNullOrEmpty(dependsAttr) ? false : Convert.ToBoolean(dependsAttr);

                switch (xmlParentCategory.GetAttribute("container_type"))
                {
                    case "single":
                        {
                            CategoryWithSingleMaterial singleCategory = new CategoryWithSingleMaterial
                                (parentCategoryPath, materialType)
                            {
                                Parent = parent,
                                DependsOnRoofType = depends
                            };
                            singleCategory.CreateMaterial();

                            string hac = nameof(singleCategory.SelectedMaterial.HasAnyChanges);
                            singleCategory.SelectedMaterial.PropertyChanged += (s, e) =>
                            {
                                if (e.PropertyName == hac)
                                    singleCategory.HasChangeInMaterial = singleCategory
                                        .SelectedMaterial.HasAnyChanges; 
                            };
                            singleCategory.SelectedMaterial.DownloadedFromXml += () =>
                            {
                                singleCategory.MaterialWasReloaded = true;
                            };

                            parentCategory = singleCategory;
                            break;
                        }
                    case "many":
                        {
                            parentCategory = new CategoryWithManyMaterials
                                (parentCategoryPath, materialType)
                            {
                                Parent = parent,
                                DependsOnRoofType = depends
                            };

                            break;
                        }
                    default: { return null; }
                }
            }

            parentCategory.Name = (xmlParentCategory).GetAttribute("name");

            return parentCategory;
        }
    }
}
