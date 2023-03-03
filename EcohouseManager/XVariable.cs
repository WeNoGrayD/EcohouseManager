using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace EcohouseManager
{

    /// <summary>
    /// Представление узла хмла-переменной в виде, пригодном
    /// для привязки к lstvVariables/lstvConstants в менеджере проекта.
    /// </summary>

    public class XVariable
    {
        public XElement Content { get; private set; }

        // Имя.

        public XAttribute Name { get; private set; }

        // Описание переменной.

        public XAttribute Description { get; private set; }

        // Число итемов-переменных без названия.

        public static uint NamelessVariablesCount { private get;  set; }

        // Конструктор, создающий пустой узел переменной.

        public XVariable()
        {
            NamelessVariablesCount++;
            Content = new XElement("const",
                    new XAttribute("name", "{без имени " + 
                                   NamelessVariablesCount + "}" ),
                    new XAttribute("description", "{нет описания}" )
                                  ) { Value = "0" };
            Name = Content.Attribute("name");
            Description = Content.Attribute("description");
        }

        // Конструктор, делящий готовый узел пункта сметы на несколько кусочков.

        public XVariable(XElement xVar, bool isConst)
        {
            Content = xVar;
            if (isConst)
            {
                Name = Content.Attribute("name");
                Description = Content.Attribute("description");
            }
            else
            {
                Name = new XAttribute("truename", Content.Name.ToString() );
                Description = Content.Attribute("name");
            }
        }
    }
}
