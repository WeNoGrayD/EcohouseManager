using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Xml;
using System.Windows.Data;
using System.IO;

namespace EcohouseManager
{
    public static class EcohouseProjectViewModel
    {
        public static WindowTest EcoProjView { get; private set; }

        public static EcohouseProjectModel EcoProj { get; private set; }

        public static XmlDocument xmlMaterialsDoc { get; private set; }
        public static XmlDocument xmlProjectDoc { get; private set; }

        public static bool ProjectMustBeLoaded { get; set; }

        static EcohouseProjectViewModel()
        {
            

            

            //EcoProj.PropertyChanged += (s, pcea) => { EcoProjView.SaveChanges(); };

            //xmlMaterialsDoc = new XmlDocument();
            //string pathToMaterials = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + @"\Project.xml";
            //xmlMaterialsDoc.Load(pathToMaterials);

            //xmlProjectDoc = new XmlDocument();
            //string pathToProject = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + @"\Project.xml";
            //xmlProjectDoc.Load(pathToProject);

            

            //view.cbBaseCategory.GetBindingExpression(ComboBox.SelectedItemProperty).
        }

        // Инициализация некоторых компонент вью-модели после инициализации компонент
        // вью (приложения).

        public static void InitializeViewModelComponents(WindowTest ecoProjView)
        {
            // Установка главного окна данного представления.

            EcoProjView = ecoProjView;

            // Загрузка документов, необходимых для создания базовой модели данных.

            xmlMaterialsDoc = ((XmlDataProvider)
                EcoProjView.Resources["MaterialsProvider"]).Document;

            xmlProjectDoc = ((XmlDataProvider)
                EcoProjView.Resources["ProjectProvider"]).Document;

            // Создание модели данных.

            EcoProj = new EcohouseProjectModel();
            EcoProjView.DataContext = EcoProj;

            // Загрузка источника элементов для листбоксов материалов и пунктов сметы
            // на странице расчёта.

            List<CategoryWithSingleMaterial> cats1 = new List<CategoryWithSingleMaterial>
                (EcoProj.FindAllLeafs().Where(cat => cat is CategoryWithSingleMaterial)
                                       .Cast<CategoryWithSingleMaterial>());

            ecoProjView.lstbCategoriesSingle.ItemsSource = from cat in cats1
                                                           select cat.SelectedMaterial;

            ecoProjView.lstb1stFloorWindowsCategory.ItemsSource =
                ((CategoryWithManyMaterials)EcoProj.FindCategory(@"floors/floor1/windows"))
                .SelectedMaterials;

            ecoProjView.lstb2ndFloorWindowsCategory.ItemsSource =
                ((CategoryWithManyMaterials)EcoProj.FindCategory(@"floors/floor2/windows"))
                .SelectedMaterials;

            // Установка связанной коллекции шапочных параметров
            // для соответствующего конвертера.

            XmlToTextBoxToHouseParamConverter.CalculationHeaderParams = 
                EcohouseProjectModel.CalculationHeaderParams;
            XmlToTextBoxToMetadataConverter.Metadata = 
                EcohouseProjectModel.Metadata;
        }

        public static void CreateNewProject()
        {
            //xmlProj = new XmlDocument();
        }

        public static void LoadProject()
        {
            //xmlProj = new XmlDocument();

            XmlWriter xmlProjReader = XmlWriter.Create("");
        }

        public static void InitializeTechTaskPage()
        {
			XmlDataProvider xmlMaterialsProvider =
				(XmlDataProvider)EcoProjView.Resources["MaterialsProvider"];
			XmlDataProvider xmlProjectProvider =
				(XmlDataProvider)EcoProjView.Resources["ProjectProvider"];
			XmlToComboBoxToFromMaterialMultiConverter conv = (XmlToComboBoxToFromMaterialMultiConverter)
                EcoProjView.spTechTask.Resources["XmlToCBToMaterialConverter"];
			
			XmlDocument xmlMaterialsDoc = new XmlDocument();
			xmlMaterialsDoc.Load("Materials.xml");
			
			XmlNode xmlMaterials = xmlMaterialsDoc.SelectSingleNode("materials");

            //BindComboBoxToCategory(EcoProjView.cbBaseCategory, null, "base");

            EcoProjView.cbBaseCategory.SelectionChanged += (s, e) =>
            {
                var tap = EcoProjView.cbBaseCategory.SelectedItem;
            };

            void BindComboBoxToCategory(ComboBox targetCB, 
									    string pathInMaterials, string pathInProject)
			{
				MultiBinding mbCategory = new MultiBinding() 
				{ Converter = conv };
				
				Binding bFromXml = new Binding()
				{ Source = xmlProjectProvider, XPath = @"house/" + pathInProject + @"/material",
                  Mode = BindingMode.OneWay };
				Binding bToMaterial = new Binding()
				{ Source = EcoProj.FindCategory(pathInProject), 
				  Path = new PropertyPath("SelectedMaterial"), 
			      Mode = BindingMode.OneWayToSource,
			      UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged};
				  
				//mbCategory.Bindings.Add(bFromXml);
				//mbCategory.Bindings.Add(bToMaterial);
				
				//targetCB.SetBinding(ComboBox.SelectedItemProperty, mbCategory);
			}
        }

        // Инициализация блоков параметров дома и этажей.

        public static void InitializeHouseParamsBlock(Window mw,
                                                      StackPanel paramsSP,
                                                      XmlElement xmlHouse)
        {
            //WindowTest wt = ((WindowTest)mw);

            //HouseInfo hInfo = new HouseInfo(xmlHouse);
            //wt.HInfo = hInfo;
            //wt.DataContext = wt.HInfo;

            //Grid grFloor1 = wt.cntrFloor1.Content as Grid,
            //     grFloor2 = wt.cntrFloor2.Content as Grid;

            //grFloor1.Name = "grFloor1";
            //grFloor1.DataContext = wt.HInfo.FloorAtGround;

            //grFloor2.Name = "grFloor2";
            //grFloor2.DataContext = wt.HInfo.FloorUnderRoof;

            /*
            Grid globalParamsSP = (Grid)mw.Resources["houseParamsBlock"];
            globalParamsSP.Name = "spGlobalParams";

            Label globalParamsLB = globalParamsSP.Children.OfType<Label>().First();
            globalParamsLB.Name = "lbGlobalParams";
            globalParamsLB.Content = "Детали";

            Grid globalParamsGR = globalParamsSP.Children.OfType<Grid>().First();
            globalParamsGR.Name = "grGlobalParams";
            BindElementToGlobalParams();

            paramsSP.Children.Add(globalParamsSP);

            int floorsCount = 0;

            foreach (FloorInfo fInfo in hInfo.Floors)
            {
                Grid floorParamsSP = (Grid)mw.Resources["houseParamsBlock"];
                floorParamsSP.Name = "spFloor" + ++floorsCount;

                Label floorIndexLB = floorParamsSP.Children.OfType<Label>().First();
                floorIndexLB.Name = "lbFloor" + floorsCount;
                floorIndexLB.Content = "Этаж " + floorsCount;

                Grid floorParamsGR = floorParamsSP.Children.OfType<Grid>().First();
                floorParamsGR.Name = "grFloor" + floorsCount;
                BindElementToFloor(floorParamsGR, fInfo);

                paramsSP.Children.Add(floorParamsSP);
            }

            return;

            void BindElementToGlobalParams()
            {
                globalParamsGR.DataContext = hInfo;

                for (int i = 1; i <= 3; i++)
                {
                    Binding elmtBindingToGlobalParam = new Binding();
                    elmtBindingToGlobalParam.Mode = BindingMode.TwoWay;
                    elmtBindingToGlobalParam.RelativeSource = new RelativeSource
                        (RelativeSourceMode.FindAncestor, typeof(Grid), 1);
                    elmtBindingToGlobalParam.Path = new PropertyPath("DataVariablesContext.");

                    Label paramNameLB = globalParamsGR.Children.OfType<Label>()
                                        .ElementAt(i - 1);
                    TextBox paramValueTB = globalParamsGR.Children.OfType<TextBox>()
                                           .ElementAt(i - 1);

                    switch (i)
                    {
                        case 1:
                            {
                                paramNameLB.Name = "globalLengthName";
                                paramNameLB.Content = "Длина";
                                paramValueTB.Name = "globalLengthValue";

                                elmtBindingToGlobalParam.Path.Path += "Length";

                                break;
                            }
                        case 2:
                            {
                                paramNameLB.Name = "globalWidthName";
                                paramNameLB.Content = "Ширина";
                                paramValueTB.Name = "globalWidthValue";

                                elmtBindingToGlobalParam.Path.Path += "Width";

                                break;
                            }
                        case 3:
                            {
                                paramNameLB.Name = "globalPogonajName";
                                paramNameLB.Content = "Погонаж ростверка";
                                paramValueTB.Name = "globalPogonajValue";

                                elmtBindingToGlobalParam.Path.Path += "PogonajRostverka";

                                break;
                            }
                    }

                    elmtBindingToGlobalParam.UpdateSourceTrigger =
                        UpdateSourceTrigger.LostFocus;
                    elmtBindingToGlobalParam.Converter =
                        new StringToDecimalConverter();

                    paramValueTB.SetBinding
                        (TextBox.TextProperty, elmtBindingToGlobalParam);
                    var t = paramValueTB.GetBindingExpression(TextBox.TextProperty);
                    paramValueTB.GetBindingExpression(TextBox.TextProperty)
                                .UpdateSource();
                }
            }

            void BindElementToFloor(Grid elmt, FloorInfo fInfo)
            {
                elmt.DataContext = fInfo;

                for (int i = 1; i <= 3; i++)
                {
                    Binding elmtBindingToFloor = new Binding();
                    elmtBindingToFloor.Mode = BindingMode.TwoWay;
                    elmtBindingToFloor.RelativeSource = new RelativeSource
                        (RelativeSourceMode.FindAncestor, typeof(Grid), 1);
                    elmtBindingToFloor.Path = new PropertyPath("DataVariablesContext.");

                    Label paramNameLB = elmt.Children.OfType<Label>()
                                        .ElementAt(i - 1);
                    TextBox paramValueTB = elmt.Children.OfType<TextBox>()
                                           .ElementAt(i - 1);

                    switch (i)
                    {
                        case 1:
                            {
                                paramNameLB.Name = "globalHeightName";
                                paramNameLB.Content = "Высота";
                                paramValueTB.Name = "globalHeightValue";

                                elmtBindingToFloor.Path.Path += "Height";
                                break;
                            }
                        case 2:
                            {
                                paramNameLB.Name = "globalPNSName";
                                paramNameLB.Content = "Погонаж несущих стен";
                                paramValueTB.Name = "globalPNSValue";

                                elmtBindingToFloor.Path.Path +=
                                                          "PogonajNesuschihSten";

                                break;
                            }
                        case 3:
                            {
                                paramNameLB.Name = "globalPNNSName";
                                paramNameLB.Content = "Погонаж ненесущих стен";
                                paramValueTB.Name = "globalPNNSValue";

                                elmtBindingToFloor.Path.Path +=
                                                          "PogonajNenesuschihSten";

                                break;
                            }
                    }

                    elmtBindingToFloor.UpdateSourceTrigger = UpdateSourceTrigger
                                                             .LostFocus;
                    elmtBindingToFloor.Converter = new StringToDecimalConverter();

                    paramValueTB.SetBinding
                    (TextBox.TextProperty, elmtBindingToFloor);
                    paramValueTB.GetBindingExpression(TextBox.TextProperty)
                                .UpdateSource();
                }
            }
            */
        }

        private static int CountOfBlocks { get; set; } = 0;

        private static int CountOfMaterials { get; set; } = 0;

        // Спавн блоков частей дома.
        // Внутри спавнятся строки с отдельными материалами - строками в смете.

        public static BlockOfHousePart SpawnBlockOfHousePart(Window mw,
                                                             XmlElement xmlHousePart)
        {
            Grid blockOfHousePartGR = (Grid)mw.Resources["BlockOfHousePart"];
            ObservableCollection<EstimateItem> estimateItems = 
				new ObservableCollection<EstimateItem>();
            string blockName = xmlHousePart.HasAttribute("name") ? 
                xmlHousePart.GetAttribute("name") : 
                ((XmlElement)xmlHousePart.ParentNode).GetAttribute("name");
            BlockOfHousePart blockOfHousePart = 
                new BlockOfHousePart(blockOfHousePartGR, blockName, 
                                     estimateItems);
            blockOfHousePartGR.DataContext = blockOfHousePart;

            // Ввод имени части дома в метку на заголовке блока.

            TextBlock housePartName = blockOfHousePartGR.Children.OfType<TextBlock>()
																 .First();
            housePartName.Name += CountOfBlocks;
            housePartName.Text = blockOfHousePart.Name;

            // Создание группы материалов.
            // Для каждого материала создаётся метка и 3 текстбокса.

            Grid groupOfMaterials = blockOfHousePartGR.Children.OfType<Grid>()
															   .First();
            groupOfMaterials.Name += CountOfBlocks;
            CountOfBlocks++;
            groupOfMaterials.DataContext = estimateItems;

            /* 
             * Коль имеется несколько физических величин,
             * следует вписывать нужную в заголовок столбца с оной.
             */

            /*
            Label lbPhysQuantity = UIHelper
                .FindChild<Label>(blockOfHousePartGR, "PhysQuantity");
            lbPhysQuantity.Content = xmlHousePart
                                     .SelectSingleNode("phys_quantity").InnerText;       
            */

            XmlElement xmlEstimatedItems = (XmlElement)xmlHousePart.SelectSingleNode("material/est_items");

            Label SummaryLB = blockOfHousePartGR.Children.OfType<Label>()
                              .First(lb => lb.Name == "Summary");

            for (int i = 0; i < xmlEstimatedItems.ChildNodes.Count; i++)
            {
                XmlElement xmlEstimateItem = (XmlElement)xmlEstimatedItems.ChildNodes[i];

                EstimateItem mp = new EstimateItem();
                estimateItems.Add(mp);
                mp.LoadFromXml(xmlEstimateItem, true);

                RowDefinition row = new RowDefinition();
                row.Height = GridLength.Auto;
                groupOfMaterials.RowDefinitions.Add(row);

                Grid grEstimateItem =
                    (Grid)mw.Resources["grEstimateItem"];
                grEstimateItem.Name += CountOfMaterials;
                grEstimateItem.DataContext = estimateItems[i];

                Grid.SetRow(grEstimateItem, i);

                /*

                TextBlock txtMaterialName = grEstimateItem
                                      .Children.OfType<DockPanel>().First()
                                      .Children.OfType<TextBlock>().First();
                txtMaterialName.Name = "txtMN" + CountOfMaterials;

                List<TextBox> tbsMaterialParams = new List<TextBox>();
                foreach (DockPanel dp in grEstimateItem.Children.OfType<DockPanel>())
                    tbsMaterialParams.AddRange(dp.Children.OfType<TextBox>()
                                     .ToArray());

                for (int j = 1; j <= 3; j++)
                    tbsMaterialParams[j - 1].Name = 
                        "tbMP" + CountOfMaterials + "_" + j;
                        */

                groupOfMaterials.Children.Add(grEstimateItem);

                CountOfMaterials++;
            }

            return blockOfHousePart;
        }
    }
}
