using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Xml;
using System.Xml.Linq;
using System.Collections.ObjectModel;
using System.IO;

namespace EcohouseManager
{
    /// <summary>
    /// Логика взаимодействия для CustomProjectManager.xaml
    /// </summary>
    /// 
    public partial class CustomProjectManager : Window
    {
        // Документы, с которыми работает менеджер.

        private XDocument xMaterials;

        private XDocument xConsts;

        // Контекстные меню, необходимые при создании trvMaterials.

        private ContextMenu cntxtCategory;
        private ContextMenu cntxtMaterial;
        private ContextMenu cntxtEstimateItem;

        // Конструктор окна.

        public CustomProjectManager()
        {
            InitializeComponent();

            // Показ категорий материалов в treeview.

            string pathMaterials = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            pathMaterials = pathMaterials.Remove(pathMaterials.IndexOf(@"\bin\Debug")) + @"\Materials.xml";
            xMaterials = new XDocument();
            xMaterials = XDocument.Load(pathMaterials);
            
            IEnumerable<XElement> housePartsCategories = xMaterials.Root.Elements();

            List<TreeViewItem> trvisCategories = new List<TreeViewItem>();

            List<TreeViewItem> trvisMaterials;
            cntxtCategory =
                (ContextMenu)trvMaterials.Resources["cntxtCategory"];
            cntxtMaterial =
                (ContextMenu)trvMaterials.Resources["cntxtMaterial"];
            cntxtEstimateItem =
                (ContextMenu)trvMaterials.Resources["cntxtEstimateItem"];

            foreach (XElement xCategory in housePartsCategories)
            {
                TreeViewItem trviCategory = new TreeViewItem()
                {
                    Header = "Категория: " + xCategory.Attribute("name").Value,
                    Tag = xCategory
                };
                trviCategory.ContextMenu = cntxtCategory;
                trvisCategories.Add(trviCategory);

                bool dependsOnRoofType = Convert.ToBoolean(xCategory.Attribute("depends_on_rooftype").Value);

                trvisMaterials = new List<TreeViewItem>();
                foreach (XElement xMaterial in xCategory.Elements())
                {
                    TreeViewItem trviMaterial = 
                        CreateTrviMaterial(xMaterial, dependsOnRoofType);
                    trvisMaterials.Add(trviMaterial);
                }

                trviCategory.ItemsSource = trvisMaterials;
            }

            trvMaterials.ItemsSource = trvisCategories;

            // Показ глобальных констант.
            // Источник данных - Variables.xml/consts.

            string pathConsts = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            pathConsts = pathConsts.Remove(pathConsts.IndexOf(@"\bin\Debug")) + @"\Variables.xml";
            xConsts = new XDocument();
            xConsts = XDocument.Load(pathConsts);
            
            IEnumerable<XElement> xsConsts = xConsts.Root.Elements();
            List<XVariable> consts = (from xConst in xsConsts
                                      select new XVariable(xConst, true))
                                      .ToList();
            lstvConstants.ItemsSource = consts;

            // Показ переменных проекта.
            // Источник данных - Core.xml/variables.

            string pathVars = System.IO.Path.GetDirectoryName
              (System.Reflection.Assembly.GetExecutingAssembly().Location) +
              @"\Core.xml";
            TextReader tr = new StringReader(Properties.Resources.xml); //new StreamReader(@"../../Properties/Resources.resx");//
            XDocument xProjVars = XDocument.Load(tr);
            //xProjVars = XDocument.Load(pathVars); 

            IEnumerable<XElement> xsVars = xProjVars.Root
                .Element("variables").Elements();
            List<XVariable> vars = (from xVar in xsVars
                                    select new XVariable(xVar, false))
                                    .ToList();
            lstvVariables.ItemsSource = vars;

            // Чтобы переменные создавались с уникальными именами,
            // найдём максимальное число "безымянных" переменных
            // (т.е. таких, которым имя не задавалось)

            System.Text.RegularExpressions.Regex findNamelessVariables =
                new System.Text.RegularExpressions
                    .Regex(@"\{без имени\s(\d+)\}");
            XVariable.NamelessVariablesCount = 0;

            uint namelessConstsCount = 0;
            IEnumerable<string> nameless = 
                from c in consts
                let d = findNamelessVariables.Match(c.Name.Value)
                        .Groups[1]?.Value
                where d != ""
                select d;
            if (nameless.Count() != 0)
                namelessConstsCount = nameless
                    .Select(d => uint.Parse(d)).Max();

            XVariable.NamelessVariablesCount = namelessConstsCount;

            // Установка известных переменных в конвертерах.

            CheckingFormulaOnUnknownVariablesConverter.xsConsts =
                xConsts.Root.Elements();
            CheckingFormulaOnUnknownVariablesConverter.xsVars =
                xsVars;
        }

        private TreeViewItem CreateTrviMaterial(XElement xMaterial,
                                                bool dependsOnRoofType)
        {
            StackPanel trviMaterialHeader =
                    (StackPanel)this.Resources["trviMaterialHeader"];
            trviMaterialHeader.DataContext = xMaterial.Element("name");
            
            TreeViewItem trviMaterial = new TreeViewItem()
            {
                Header = trviMaterialHeader,
                Tag = xMaterial
            };
            
            trviMaterial.ContextMenu = cntxtMaterial;

            List<TreeViewItem> xsEstimateItems = new List<TreeViewItem>();
            TreeViewItem trviEstimateItem;

            if (xMaterial.Element("est_items") == null)
                xMaterial.Add(new XElement("est_items"));
            foreach (XElement xEI in xMaterial
                                    .Element("est_items")
                                    .Elements("est_item"))
            {
                XEstimateItem xEstimateItem = new XEstimateItem(xEI, dependsOnRoofType);
                trviEstimateItem = CreateTrviEstimateItem(xEstimateItem);
                xsEstimateItems.Add(trviEstimateItem);
            }
            trviMaterial.ItemsSource = xsEstimateItems;

            return trviMaterial;
        }

        private TreeViewItem CreateTrviEstimateItem(XEstimateItem xEstimateItem)
        {
            StackPanel trviEstimateItemHeader =
                    (StackPanel)this.Resources["trviEstimateItemHeader"];
            trviEstimateItemHeader.DataContext = xEstimateItem.Name;

            TreeViewItem trviEstimateItem = new TreeViewItem()
            {
                Header = trviEstimateItemHeader,
                Tag = xEstimateItem
            };
            trviEstimateItem.ContextMenu = cntxtEstimateItem;

            return trviEstimateItem;
        }

        // Если выбран материал, то следует обновить список его пунктов сметы.

        private void TrvMaterials_SelectedItemChanged
            (object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            TreeViewItem trvi = e.NewValue as TreeViewItem;
            XEstimateItem xEI;
            if ((xEI = trvi.Tag as XEstimateItem) != null)
            {
                spEditingEstimateItem.IsEnabled = true;
                spEditingEstimateItem.DataContext = (XEstimateItem)trvi.Tag;

                // Скрытие лишних формул.

                if (!xEI.DependsOnRoofType)
                {
                    lbQuantityTitle.Content = "Формула количества";
                    foreach (int i in Enumerable.Range(1, 3))
                        grQuantityFormulas.RowDefinitions[i].Height =
                            new GridLength(0);
                }
                else
                {
                    lbQuantityTitle.Content = "Формулы количества";
                    foreach (int i in Enumerable.Range(1, 3))
                        grQuantityFormulas.RowDefinitions[i].Height =
                            new GridLength(1, GridUnitType.Star);
                }
            }
        }

        // Добавить материал в категорию.

        private void AddMaterialToCategory(object sender, RoutedEventArgs e)
        {
            TreeViewItem trviCategory = (TreeViewItem)
                ((ContextMenu)(sender as MenuItem).Parent).PlacementTarget;

            XElement xCategory = (XElement)trviCategory.Tag;

            string materialType = xCategory.Attribute("materials_type").Value; 
            XElement xMaterial = new XElement(materialType,
                new XElement("name") { Value = "{без имени}" },
                new XElement("est_items"));
            xCategory.Add(xMaterial);

			ContextMenu cntxtMaterial =
                (ContextMenu)trvMaterials.Resources["cntxtMaterial"];
            StackPanel trviMaterialHeader = 
                (StackPanel)this.Resources["trviMaterialHeader"];
            trviMaterialHeader.DataContext = xMaterial.Element("name");
			
            TreeViewItem trviMaterial = new TreeViewItem()
            {
                Header = trviMaterialHeader,
                Tag = xMaterial
            };
			trviMaterial.ContextMenu = cntxtMaterial;
            trviMaterial.ItemsSource = new List<TreeViewItem>();
			
            ((List<TreeViewItem>)trviCategory.ItemsSource)?.Add(trviMaterial);
            trviCategory.Items.Refresh();
        }

        // Добавить пункт сметы в материал.

        private void AddEstimateItemToMaterial(object sender, RoutedEventArgs e)
        {
            TreeViewItem trviMaterial = (TreeViewItem)
                ((ContextMenu)(sender as MenuItem).Parent).PlacementTarget;
            XElement xMaterial = (XElement)trviMaterial.Tag;
            XEstimateItem xEstimateItem = CreateXEstimateItem(trviMaterial);
            trviMaterial.Items.Refresh();
        }

        // Создание нового пункта сметы.

        private XEstimateItem CreateXEstimateItem(TreeViewItem trviMaterial)
        {
            XElement xMaterial = (XElement)trviMaterial.Tag;
            bool dependsOnRoofType = Convert.ToBoolean(xMaterial.Parent.Attribute("depends_on_rooftype").Value);
            XEstimateItem xEstimateItem = new XEstimateItem(dependsOnRoofType);
            
            xMaterial.Element("est_items").Add(xEstimateItem.Content);
            TreeViewItem trviEstimateItem =
                CreateTrviEstimateItem(xEstimateItem);
            ((List<TreeViewItem>)trviMaterial.ItemsSource).Add(trviEstimateItem);
            return xEstimateItem;
        }

        // Удалить материал из категории.

        private void DeleteMaterialFromCategory(object sender, RoutedEventArgs e)
        {
            TreeViewItem trviMaterial = (TreeViewItem)
                ((ContextMenu)(sender as MenuItem).Parent).PlacementTarget;
            XElement xMaterial = (XElement)trviMaterial.Tag;
            xMaterial.Remove();
            List<TreeViewItem> trviItemsSource = null;
            TreeViewItem trviCategory = ((List<TreeViewItem>)trvMaterials.ItemsSource)
                .Find(trvi => 
                {
                    trviItemsSource = (List<TreeViewItem>)trvi.ItemsSource;
                    bool? hasItems = trviItemsSource?.Contains(trviMaterial);
                    return hasItems ?? false;
                });
            ((List<TreeViewItem>)trviCategory.ItemsSource).Remove(trviMaterial);
            trviCategory.Items.Refresh();
        }

        // Редактировать пункт сметы в материале.

        private void EditEstimateItem(object sender, RoutedEventArgs e)
        {
            ListViewItem lstviEstimateItem = (ListViewItem)
                ((ContextMenu)(sender as MenuItem).Parent).PlacementTarget;
            XEstimateItem xEstimateItem = (XEstimateItem)lstviEstimateItem.Content;

            EditingEstimateItem editWindow = new EditingEstimateItem(xEstimateItem);
            editWindow.ShowDialog();
        }

        // Удалить пункт сметы из материала.

        private void DeleteEstimateItemFromMaterial(object sender, RoutedEventArgs e)
        {
            TreeViewItem trviEstimateItem = (TreeViewItem)
                ((ContextMenu)(sender as MenuItem).Parent).PlacementTarget;

            XEstimateItem xEstimateItem = (XEstimateItem)trviEstimateItem.Tag;
            xEstimateItem.Content.Remove();

            List<TreeViewItem> trviItemsSource = null;
            TreeViewItem trviMaterial = null;
            foreach (TreeViewItem trviCategory in trvMaterials.Items)
            {
                trviMaterial = ((List<TreeViewItem>)trviCategory.ItemsSource)
                    .Find(trvi =>
                {
                    trviItemsSource = (List<TreeViewItem>)trvi.ItemsSource;
                    bool? hasItems = trviItemsSource?.Contains(trviEstimateItem);
                    return hasItems ?? false;
                });

                if (trviMaterial != null)
                    break;
            }

            ((List<TreeViewItem>)trviMaterial.ItemsSource)
                .Remove(trviEstimateItem);
            trviMaterial.Items.Refresh();
        }

        // Обновить файлы.

        private void Update_File(object sender, RoutedEventArgs e)
        {
            string fileToUpdate;
            if (sender.Equals(this))
                fileToUpdate = "both";
            else
                fileToUpdate = ((FrameworkElement)sender)?.Tag.ToString();

            string pathMaterials = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            pathMaterials = pathMaterials.Remove(pathMaterials.IndexOf(@"\bin\Debug")) + @"\Materials.xml";
            string pathConsts = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            pathConsts = pathConsts.Remove(pathConsts.IndexOf(@"\bin\Debug")) + @"\Variables.xml";

            switch (fileToUpdate)
            {
                case "Materials.xml":
                    {
                        xMaterials.Save(pathMaterials);
                        return;
                    }
                case "Variables.xml":
                    {
                        xConsts.Save(pathConsts);
                        BestCalculatorEver.UpdateProjectConstants();
                        CheckingFormulaOnUnknownVariablesConverter.xsConsts =
                            xConsts.Root.Elements();
                        imgQuantityFormula1.GetBindingExpression(Image.SourceProperty)
                            .UpdateTarget();
                        imgQuantityFormula2.GetBindingExpression(Image.SourceProperty)
                            .UpdateTarget();
                        imgQuantityFormula3.GetBindingExpression(Image.SourceProperty)
                            .UpdateTarget();
                        imgQuantityFormula4.GetBindingExpression(Image.SourceProperty)
                            .UpdateTarget();
                        return;
                    }
                case "both":
                    {
                        xMaterials.Save(pathMaterials);
                        xConsts.Save(pathConsts);
                        BestCalculatorEver.UpdateProjectConstants();
                        return;
                    }
            }
        }

        // Закрыть окно и, при надобности, сохранить файлы.

        private void CloseAndSave(object sender, RoutedEventArgs e)
        {
            string caption = "Сохранить и выйти?",
                   dialog = "Вы собираетесь выйти. Сохранить файлы менеджера?";
            MessageBoxResult isNeedtoSaveAndOrClose =
                MessageBox.Show(dialog, caption, 
                                MessageBoxButton.YesNoCancel,
                                MessageBoxImage.Question);
            switch (isNeedtoSaveAndOrClose)
            {
                case MessageBoxResult.Yes:
                    { Update_File(this, null); break; }
                case MessageBoxResult.No: break;
                case MessageBoxResult.Cancel: return;
            }
            this.Close();
        }

        private void LstviConst_ReleaseContext
            (object sender, MouseButtonEventArgs e)
        {
            if (e.Handled)
                return;

            ListViewItem lstviConst = (ListViewItem)sender;
            if (lstviConst == null)
                return;

            if (!lstviConst.IsFocused)
                lstviConst.Focus();
        }

        // Добавление константы в lstvConstants и хмл.

        private void AddConstant(object sender, RoutedEventArgs e)
        {
            XVariable xVar = new XVariable();
            xConsts.Root.Add(xVar.Content);
            ((List<XVariable>)lstvConstants.ItemsSource).Add(xVar);
            lstvConstants.Items.Refresh();
        }

        // Удаление константы из lstvConstants и хмла.

        private void DeleteConstant(object sender, RoutedEventArgs e)
        {
            ListViewItem lstviVar = (ListViewItem)
                ((ContextMenu)(sender as MenuItem).Parent).PlacementTarget;
            XVariable xVar = (XVariable)lstviVar.Content;
            xVar.Content.Remove();
            ((List<XVariable>)lstvConstants.ItemsSource).Remove(xVar);
            lstvConstants.Items.Refresh();
        }

        // Предоставление списка поддерживаемых команд.

        private void SupportingCommands_Show(object sender, RoutedEventArgs e)
        {
            (new SupportingCommand()).Show();
        }
    }
}
