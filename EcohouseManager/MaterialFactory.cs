using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace EcohouseManager
{
    /// <summary>
    /// Фабрика материалов.
    /// </summary>

    public class MaterialFactory
    {
        public static Material CreateMaterial(TechTaskCategory parentCategory, string materialType)
        {
            switch (materialType)
            {
                case "material": { return new Material(parentCategory); }
                case "enum_material": { return new EnumerableMaterial(parentCategory); }
                case "window": { return new WindowMaterial(parentCategory); }
                default: return null;
            }
        }
    }
}
