//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан программой.
//     Исполняемая версия:4.0.30319.42000
//
//     Изменения в этом файле могут привести к неправильной работе и будут потеряны в случае
//     повторной генерации кода.
// </auto-generated>
//------------------------------------------------------------------------------

namespace EcohouseManager.Properties {
    using System;
    
    
    /// <summary>
    ///   Класс ресурса со строгой типизацией для поиска локализованных строк и т.д.
    /// </summary>
    // Этот класс создан автоматически классом StronglyTypedResourceBuilder
    // с помощью такого средства, как ResGen или Visual Studio.
    // Чтобы добавить или удалить член, измените файл .ResX и снова запустите ResGen
    // с параметром /str или перестройте свой проект VS.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "17.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    public class Resources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        /// <summary>
        ///   Возвращает кэшированный экземпляр ResourceManager, использованный этим классом.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("EcohouseManager.Properties.Resources", typeof(Resources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Перезаписывает свойство CurrentUICulture текущего потока для всех
        ///   обращений к ресурсу с помощью этого класса ресурса со строгой типизацией.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Поиск локализованного ресурса типа System.Drawing.Bitmap.
        /// </summary>
        public static System.Drawing.Bitmap ok_128 {
            get {
                object obj = ResourceManager.GetObject("ok_128", resourceCulture);
                return ((System.Drawing.Bitmap)(obj));
            }
        }
        
        /// <summary>
        ///   Поиск локализованного ресурса типа System.Drawing.Bitmap.
        /// </summary>
        public static System.Drawing.Bitmap warning {
            get {
                object obj = ResourceManager.GetObject("warning", resourceCulture);
                return ((System.Drawing.Bitmap)(obj));
            }
        }
        
        /// <summary>
        ///   Поиск локализованного ресурса типа System.Drawing.Bitmap.
        /// </summary>
        public static System.Drawing.Bitmap warning_128 {
            get {
                object obj = ResourceManager.GetObject("warning_128", resourceCulture);
                return ((System.Drawing.Bitmap)(obj));
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на &lt;name&gt;&lt;/name&gt;&lt;quantity&gt;&lt;/quantity&gt;&lt;price&gt;&lt;/price&gt;&lt;summary&gt;&lt;/summary&gt;.
        /// </summary>
        public static string windowNodeTemplate {
            get {
                return ResourceManager.GetString("windowNodeTemplate", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на &lt;?xml version=&quot;1.0&quot; encoding=&quot;utf-8&quot;?&gt;
        ///&lt;!--Проект--&gt;
        ///&lt;root&gt;
        ///&lt;house name = &quot;Проект&quot;&gt; 
        ///		&lt;!--Имя клиента--&gt;
        ///		&lt;customer name = &quot;Имя клиента&quot;&gt;&lt;/customer&gt;
        ///		&lt;!--Телефон--&gt;
        ///		&lt;phone name = &quot;Телефон&quot;&gt;&lt;/phone&gt;
        ///
        ///		&lt;!--Населенный пункт--&gt;
        ///		&lt;location name = &quot;Населенный пункт&quot;&gt;&lt;/location&gt;
        ///		&lt;!--Расстояние в километрах--&gt;
        ///		&lt;distance name = &quot;Расстояние в километрах&quot;&gt;&lt;/distance&gt;
        ///
        ///		&lt;!--Площадь в метрах квадратных--&gt;
        ///		&lt;area name = &quot;Площадь&quot;&gt;&lt;/area&gt;
        ///
        ///	&lt;!--Фундамент--&gt;
        ///	&lt;base name = &quot;Фундамент&quot;&gt;&lt;/base&gt;        /// [остаток строки не уместился]&quot;;.
        /// </summary>
        public static string xml {
            get {
                return ResourceManager.GetString("xml", resourceCulture);
            }
        }
    }
}
