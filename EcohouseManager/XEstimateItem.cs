using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace EcohouseManager
{
    /// <summary>
    /// Модель узла пункта сметы для lstvEstimateItems в менеджере кастомного проекта.
    /// </summary>

    public class XEstimateItem
    {
        public XElement Content { get; private set; }

        // Имя.

        public XElement Name { get; private set; }

        // Описание пункта сметы.

        public XElement Description { get; private set; }

        // Формулы, по которой вычисляется цена.

        public XElement PriceFormula { get; private set; }

        // Формула, по которым вычисляется количество.

        public XElement QuantityFormula1 { get; private set; }
        public XElement QuantityFormula2 { get; private set; }
        public XElement QuantityFormula3 { get; private set; }
        public XElement QuantityFormula4 { get; private set; }
        private XElement Markup { get; set; }

        // Единица измерения количества.

        public XElement Measure { get; set; }

        // Зависимость от типа крыши.

        public bool DependsOnRoofType { get; private set; }

        // Конструктор, создающий пустой узел пункта сметы.

        public XEstimateItem(bool dependsOnRoofType)
        {
            Name = new XElement("name") { Value = "{без имени}" };
            Description = new XElement("description") { Value = "{нет описания}" };
            PriceFormula = new XElement("price") { Value = "0" };
            Measure = new XElement("measure") { Value = "{нет}" };
            Markup = new XElement("markup") { Value = "35,0" };
            XElement quantityFormulas = new XElement("quantity_formulas");

            Content = new XElement("est_item",
                                   Name,
                                   Description,
                                   PriceFormula,
                                   Measure,
                                   Markup,
                                   quantityFormulas
                                  );

            DependsOnRoofType = dependsOnRoofType;
            if (dependsOnRoofType)
            {
                QuantityFormula1 = new XElement("quantity", new XAttribute("rooftype", "TwoFloors")) { Value = "0" };
                quantityFormulas.Add(QuantityFormula1);
                QuantityFormula2 = new XElement("quantity", new XAttribute("rooftype", "FourSlopes")) { Value = "0" };
                quantityFormulas.Add(QuantityFormula2);
                QuantityFormula3 = new XElement("quantity", new XAttribute("rooftype", "RoofLorry")) { Value = "0" };
                quantityFormulas.Add(QuantityFormula3);
                QuantityFormula4 = new XElement("quantity", new XAttribute("rooftype", "RidgeRoof")) { Value = "0" };
                quantityFormulas.Add(QuantityFormula4);
            }
            else
            {
                QuantityFormula1 = new XElement("quantity", new XAttribute("rooftype", "None")) { Value = "0" };
                quantityFormulas.Add(QuantityFormula1);
            }
        }

        // Конструктор, делящий готовый узел пункта сметы на несколько кусочков.
        
        public XEstimateItem(XElement xEstimateItem, bool dependsOnRoofType)
        {
            Content = xEstimateItem;
            if (Content.Element("name") == null)
                Content.Add(new XElement("name") { Value = "{без имени}" });
            Name = Content.Element("name");
            if (Content.Element("description") == null)
                Content.Add(new XElement("description") { Value = "{нет описания}" });
            Description = Content.Element("description");
            if (Content.Element("price") == null)
                Content.Add(new XElement("price") { Value = "0" });
            PriceFormula = Content.Element("price");
            if (Content.Element("measure") == null)
                Content.Add(new XElement("measure") { Value = "{нет}" });
            Measure = Content.Element("measure");
            if (Content.Element("markup") == null)
                Content.Add(new XElement("markup") { Value = "35,0" });
            Markup = Content.Element("markup");

            DependsOnRoofType = dependsOnRoofType;

            if (Content.Element("quantity_formulas") == null)
            {
                XElement quantityFormulas = new XElement("quantity_formulas");
                Content.Add(quantityFormulas);

                if (dependsOnRoofType)
                {
                    quantityFormulas.Add(new XElement("quantity", new XAttribute("rooftype", "TwoFloors")) { Value = "0" });
                    quantityFormulas.Add(new XElement("quantity", new XAttribute("rooftype", "FourSlopes")) { Value = "0" });
                    quantityFormulas.Add(new XElement("quantity", new XAttribute("rooftype", "RoofLorry")) { Value = "0" });
                    quantityFormulas.Add(new XElement("quantity", new XAttribute("rooftype", "RidgeRoof")) { Value = "0" });
                }
                else
                    quantityFormulas.Add(new XElement("quantity", new XAttribute("rooftype", "None")) { Value = "0" });
            }

            if (dependsOnRoofType)
            {
                QuantityFormula1 = Content.Element("quantity_formulas").Elements("quantity")
                    .First(pf => pf.Attribute("rooftype").Value == "TwoFloors");
                QuantityFormula2 = Content.Element("quantity_formulas").Elements("quantity")
                    .First(pf => pf.Attribute("rooftype").Value == "FourSlopes");
                QuantityFormula3 = Content.Element("quantity_formulas").Elements("quantity")
                    .First(pf => pf.Attribute("rooftype").Value == "RoofLorry");
                QuantityFormula4 = Content.Element("quantity_formulas").Elements("quantity")
                    .First(pf => pf.Attribute("rooftype").Value == "RidgeRoof");
            }
            else
            {
                QuantityFormula1 = Content.Element("quantity_formulas").Element("quantity");
            }
        }

        // Копирование узла без потери ссылочной целостности.

        public XEstimateItem Copy(XEstimateItem xOriginal)
        {
            Name.Value = xOriginal.Name.Value;
            Description.Value = xOriginal.Description.Value;
            PriceFormula.Value = xOriginal.PriceFormula.Value;
            QuantityFormula1.Value = xOriginal.QuantityFormula1.Value;
            QuantityFormula2.Value = xOriginal.QuantityFormula2.Value;
            QuantityFormula3.Value = xOriginal.QuantityFormula3.Value;
            QuantityFormula4.Value = xOriginal.QuantityFormula4.Value;
            Measure.Value = xOriginal.Measure.Value;
            Markup.Value = xOriginal.Markup.Value; 

            return this;
        }
    }
}
