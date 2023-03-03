using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace EcohouseManager
{
    public static class DocumentExtensions
    {
        public static XmlElement ToXmlElement(this XElement el, XmlDocument ownerDocument)
        {
            return (XmlElement)ownerDocument.ReadNode(el.CreateReader());
        }

        public static XElement ToXElement(this XmlElement xmlElement)
        {
            XElement xElement;
            using (var nodeReader = new XmlNodeReader(xmlElement))
            {
                nodeReader.MoveToContent();
                xElement = XElement.Load(nodeReader);
            }
            return xElement;
        }
    }
}
