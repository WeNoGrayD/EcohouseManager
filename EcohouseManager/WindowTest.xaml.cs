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
using System.Windows.Threading;
using System.Xml;
using System.Xml.Linq;
using System.Collections.ObjectModel;
using Microsoft.Win32;
using Xceed.Wpf.Toolkit;

namespace EcohouseManager
{
    /// <summary>
    /// Логика взаимодействия для WindowTest.xaml
    /// </summary>
    public partial class WindowTest : Window
    {
        // Таймер для уведомления о загрузке всех категорий.

        private DispatcherTimer OnLoadProjectTimer;

        // Флаг, указывающий на то, что проект пуст.

        private bool isProjectNew = true;

        // Путь к текущему файлу (если он не пуст).

        private string CurrentProjectPath;

        // Конструктор окна.

        public WindowTest()
        {
            InitializeComponent();

            EventManager.RegisterClassHandler(typeof(ListBox),
                ListBox.MouseWheelEvent, new MouseWheelEventHandler(Control_MouseWheelHandler), true);

            EcohouseProjectViewModel.InitializeViewModelComponents(this);

            OnLoadProjectTimer = new DispatcherTimer() { Interval = TimeSpan.FromMilliseconds(500) };
            OnLoadProjectTimer.Tick += (s, e) => 
            {
                EcohouseProjectModel ecoProj = EcohouseProjectViewModel.EcoProj;
                if (!ecoProj.CategoriesAreLoading.Values
                    .Aggregate((anyLoading, isLoading) => anyLoading |= isLoading))
                {
                    OnLoadProjectTimer.Stop();
                    EcohouseProjectViewModel.ProjectMustBeLoaded = false;
                    ecoProj.ClearChanges();
                }
            };

            BestCalculatorEver.UpdateProjectVariables(EcohouseProjectModel.CalculationHeaderParams);
        }

        private void BtnTest_Click(object sender, EventArgs e)
        {
            EcohouseProjectModel eco = EcohouseProjectViewModel.EcoProj;
            CategoryWithSingleMaterial cat1 = (CategoryWithSingleMaterial)
                EcohouseProjectViewModel.EcoProj.FindCategory(@"floors/floor1/plasterboard/plasterboard_walls");
            EnumerableMaterial mat1 = (EnumerableMaterial)cat1.SelectedMaterial;
            CategoryWithSingleMaterial cat2 = (CategoryWithSingleMaterial)
                EcohouseProjectViewModel.EcoProj.FindCategory(@"floors/floor2/plasterboard/plasterboard_walls");
            EnumerableMaterial mat2 = (EnumerableMaterial)cat2.SelectedMaterial;


            CategoryWithManyMaterials cat3 = (CategoryWithManyMaterials)
                EcohouseProjectViewModel.EcoProj.FindCategory(@"floors/floor1/windows");
            WindowMaterial[] mat3 = cat3.SelectedMaterials.Cast<WindowMaterial>().ToArray();
            CategoryWithManyMaterials cat4 = (CategoryWithManyMaterials)
                EcohouseProjectViewModel.EcoProj.FindCategory(@"floors/floor2/windows");
            WindowMaterial[] mat4 = cat4.SelectedMaterials.Cast<WindowMaterial>().ToArray();

            /*
            var cb1 = cb1stFloorExternalWallsCategory;
            var dc1 = cb1.DataContext;
            var cb2 = cb1stFloorBearingWallsCategory;
            var dc2 = cb2.DataContext;
            var cb3 = cb1stFloorCurtainWallsCategory;
            var dc3 = cb3.DataContext;
            var cb4 = cb1stFloorPlasterboardWallsCategory;
            var dc4 = cb4.DataContext;
            var cb5 = cb1stFloorPlasterboardCeilingCategory;
            var dc5 = cb5.DataContext;
            */

            //bool changes1 = eco.BaseCategory.SelectedMaterial.HasAnyChanges;
            //.eco.BaseCategory.SelectedMaterial.EstimateItems.Values.ToList()[0].Price = 12;
            //eco.BaseCategory.SelectedMaterial.EstimateItems.Values.ToList()[1].Markup = 100;
            //eco.BaseCategory.SelectedMaterial.EstimateItems.Values.ToList()[2].PhysicalQuantity = 34500;
        }

        // Пересчёт количества пункта сметы.

        private void btnRecalculateParams_Click(object sender, RoutedEventArgs e)
        {
            EstimateItem eItem = (EstimateItem)((Button)sender).DataContext;
            eItem.CalculateQuantityWithFormula();
        }

        
        private void btnOpenEstimatePage_Click(object sender, RoutedEventArgs e)
        {
            //GenerateEstimatePage();
        }
        

        /*
        private void GenerateEstimatePage()
        {
            spEstimate.Children.Clear();

            Label lbCategoryName;
            Label lbSummary;

            foreach (Category cat in Categories)
            {
                lbCategoryName = new Label();
                lbCategoryName.Content = cat.Name;
                spEstimate.Children.Add(lbCategoryName);

                foreach (BlockOfHousePart material in cat.Materials)
                {
                    StackPanel spMaterial =
                            (StackPanel)tabpEstimate.Resources["spMaterial"];

                    spMaterial.DataContext = material.EstimateItems;
                    spMaterial.Tag = material.Name;
                    spEstimate.Children.Add(spMaterial);

                    lbSummary = new Label();
                    lbSummary.Content = $"Итого по {material.Name}" +
                        $": {material.Summary:f3}";
                    spEstimate.Children.Add(lbSummary);
                }
            }
        }
        */

        // Создание нового проекта.

        private void CreateNewProject(object sender, RoutedEventArgs e)
        {
            isProjectNew = true;
            EcohouseProjectViewModel.ProjectMustBeLoaded = true;

            string pathProjectPattern = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            pathProjectPattern = pathProjectPattern.Remove(pathProjectPattern.IndexOf(@"\bin\Debug")) + @"\ProjectPattern.xml";
            XmlDocument xmlProjectDoc = new XmlDocument();
            xmlProjectDoc.Load(pathProjectPattern);

            ((XmlDataProvider)this.Resources["ProjectProvider"]).Document = xmlProjectDoc;

            CategoryWithManyMaterials windowsCategory1 = (CategoryWithManyMaterials)
                EcohouseProjectViewModel.EcoProj.FindCategory(@"floors/floor1/windows");
            windowsCategory1.ClearFromMaterials();
            CategoryWithManyMaterials windowsCategory2 = (CategoryWithManyMaterials)
                EcohouseProjectViewModel.EcoProj.FindCategory(@"floors/floor2/windows");
            windowsCategory2.ClearFromMaterials();

            EcohouseProjectViewModel.ProjectMustBeLoaded = false;
            EcohouseProjectViewModel.EcoProj.ClearChanges();
        }

        // Открытие сохранённого проекта.

        private void LoadProject(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openProjDialog = new OpenFileDialog();
            openProjDialog.Filter = "Файлы проекта \"Экодом\"|*.ecoproj|Файлы Extended Markup Language|*.xml";
            openProjDialog.DefaultExt = "ecoproj";
            openProjDialog.Title = "Открыть проект";
            openProjDialog.CheckFileExists = true;
            openProjDialog.CheckPathExists = true;
            //openProjDialog.InitialDirectory = "";

            if (openProjDialog.ShowDialog() == false)
                return;

            ((XmlDataProvider)this.Resources["ProjectProvider"]).Document =
             new XmlDocument();

            // Очистка всех материалов.

            foreach (TechTaskCategory leafCategory in EcohouseProjectViewModel.EcoProj.FindAllLeafs())
            {
                if (leafCategory is CategoryWithSingleMaterial)
                    ((CategoryWithSingleMaterial)leafCategory).SelectedMaterial.Clear();
                if (leafCategory is CategoryWithManyMaterials)
                    ((CategoryWithManyMaterials)leafCategory).ClearFromMaterials();

                /* 
                 * Установка в словаре загрузок категорий статусы загрузки всех категорий = true. 
                 */

                EcohouseProjectViewModel.EcoProj.CategoriesAreLoading[leafCategory.GetAbsolutePath()] = true;
            }

            XmlDocument xmlProjectDoc = new XmlDocument();
            xmlProjectDoc.Load(openProjDialog.FileName);

            // Изменение метаданных о проекте.

            ObservableDictionary<string, string> metadata = EcohouseProjectModel.Metadata;
            XmlNodeList xmlsMetadata = xmlProjectDoc.DocumentElement.SelectNodes(@"meta/*");
            XmlElement xmlMetaInfo;
            for (int i = 0; i < xmlsMetadata.Count; i++)
            {
                xmlMetaInfo = (XmlElement)xmlsMetadata[i];
                metadata[xmlMetaInfo.Name] = xmlMetaInfo.InnerText;
            }

            // Загрузка материалов окон.

            for (int fl = 1; fl <= 2; fl++)
            {
                XmlNodeList xmlsWindows = xmlProjectDoc.DocumentElement
                .SelectNodes(@"house/floors/floor" + fl + @"/windows/window");

                CategoryWithManyMaterials windowsCategory = (CategoryWithManyMaterials)
                    EcohouseProjectViewModel.EcoProj.FindCategory(@"floors/floor" + fl + @"/windows");
                windowsCategory.ClearFromMaterials();

                for (int i = 0; i < xmlsWindows.Count; i++)
                {
                    windowsCategory.AddNewMaterial();
                    windowsCategory.SelectedMaterials[i].LoadFromXml((XmlElement)xmlsWindows[i], true);
                }
                windowsCategory.RefreshMaterialsLog();

                EcohouseProjectViewModel.EcoProj.CategoriesAreLoading
                    [windowsCategory.GetAbsolutePath()] = false;
            }

            // Оповещение компонентов вью-модели (конвертеров, выполняющих загрузку модели)
            // о том, что проект ДОЛЖЕН загрузиться.

            EcohouseProjectViewModel.ProjectMustBeLoaded = true;

            ((XmlDataProvider)this.Resources["ProjectProvider"]).Document =
                xmlProjectDoc;

            isProjectNew = false;
            CurrentProjectPath = openProjDialog.FileName;

            EcohouseProjectViewModel.ProjectMustBeLoaded = false;
            EcohouseProjectViewModel.EcoProj.ClearChanges();

            // Запуск таймера на уведомление о загрузке всех категорий.

            //OnLoadProjectTimer.Start();
        }

        /* 
         * Сохранение изменений в проекта:
         *   -- после выбора материала, добавления/удаления окна, отметки чекбокса,
         * ввода какого-либо значения на странице заполнения ТЗ;
         *   -- после изменения шапочного параметра или изменения параметров
         * пунктов сметы на странице расчёта.
         */

        private void SaveChanges(object sender, RoutedEventArgs e)
        {
            if (isProjectNew)
            {
                SaveProjectAs(sender, e);
                return;
            }

            XmlDocument xmlProjectDoc = ((XmlDataProvider)
                this.Resources["ProjectProvider"]).Document;

            // Изменение метаданных о проекте.

            ObservableDictionary<string, string> metadata = EcohouseProjectModel.Metadata;
            XmlElement xmlMetaInfo;
            foreach (string metaInfoKey in metadata.Keys)
            {
                xmlMetaInfo = (XmlElement)xmlProjectDoc.DocumentElement
                    .SelectSingleNode(@"meta/" + metaInfoKey);
                xmlMetaInfo.InnerText = metadata[metaInfoKey];
            }

            // Изменение параметров расчёта.

            ObservableDictionary<string, bool> projHasChangesInParams = EcohouseProjectViewModel.EcoProj
                .HasChangesInParams;
            XmlElement xmlParam;
            foreach (string paramName in projHasChangesInParams.Where(ch => ch.Value).Select(ch => ch.Key))
            {
                xmlParam = (XmlElement)
                    xmlProjectDoc.DocumentElement.SelectSingleNode(@"house_params/" + paramName);
                xmlParam.InnerText = EcohouseProjectModel.CalculationHeaderParams[paramName].ToString();
            }

            Dictionary<string, bool> projHasChangesInModel = EcohouseProjectViewModel.EcoProj
                .HasChangesInModel;
            XmlElement xmlCategory;
            foreach (string catPath in projHasChangesInModel.Where(ch => ch.Value).Select(ch => ch.Key))
            {
                xmlCategory = (XmlElement)
                    xmlProjectDoc.DocumentElement.SelectSingleNode(@"house/" + catPath);

                TechTaskCategory cat = EcohouseProjectViewModel.EcoProj.FindCategory(catPath);
                string materialsType = "";

                if (cat is CategoryWithSingleMaterial)
                {
                    CategoryWithSingleMaterial singleCat = (CategoryWithSingleMaterial)cat;
                    materialsType = singleCat.MaterialType;

                    if (singleCat.MaterialWasReloaded)
                    {
                        XmlNode prevSelectedMaterial = xmlCategory.SelectSingleNode(materialsType);
                        if (prevSelectedMaterial != null)
                            xmlCategory.RemoveChild(prevSelectedMaterial);
                        xmlCategory.AppendChild(DocumentExtensions.ToXmlElement
                            (singleCat.SelectedMaterial.WriteToXmlAsXElement(), xmlProjectDoc));
                    }
                    else if (singleCat.HasChangeInMaterial)
                        InvestigateMaterialOnChangesAndWriteToXml(singleCat.SelectedMaterial);
                    
                    xmlCategory.SetAttribute("is_loaded", true.ToString());
                }
                else
                {
                    CategoryWithManyMaterials manyCat = (CategoryWithManyMaterials)cat;
                    materialsType = manyCat.MaterialsType;
                    XmlNodeList xmlsMaterials = xmlCategory.SelectNodes(materialsType);
                    int j = 0;
                    for (int i = 0; i < manyCat.MaterialsLog.Count; i++)
                    {
                        var logRecord = manyCat.MaterialsLog[i];

                        /*
                         * Сначала проверяются имевшиеся материалы.
                         * Если материал был изменён, то меняем его.
                         * Если материал был удалён, то удаляем.
                         */

                        if (i < manyCat.NativeMaterialsCount)
                        {
                            if (logRecord.Action == CategoryWithManyMaterials.LogAction.Native)
                            {
                                if (logRecord.Reloaded)
                                {
                                    XmlNode prevSelectedMaterial = xmlsMaterials[j];
                                    xmlCategory.RemoveChild(prevSelectedMaterial);
                                    if (manyCat.NativeMaterialsCount == 1)
                                        xmlCategory.AppendChild(DocumentExtensions.ToXmlElement
                                            (logRecord.Target.WriteToXmlAsXElement(), xmlProjectDoc));
                                    else if (j > 0)
                                        xmlCategory.InsertAfter(DocumentExtensions.ToXmlElement
                                            (logRecord.Target.WriteToXmlAsXElement(), xmlProjectDoc),
                                            xmlsMaterials[j - 1]);
                                    else if (j == 0)
                                        xmlCategory.InsertBefore(DocumentExtensions.ToXmlElement
                                            (logRecord.Target.WriteToXmlAsXElement(), xmlProjectDoc),
                                            xmlsMaterials[1]);
                                }
                                else if (logRecord.HasChanged) 
                                    InvestigateMaterialOnChangesAndWriteToXml(manyCat.SelectedMaterials[j], j);

                                j++;
                            }

                            else if (logRecord.Action == CategoryWithManyMaterials.LogAction.Removed)
                                xmlCategory.RemoveChild(xmlsMaterials[i]);
                        }

                        /*
                         * Затем проверяются поступившие материалы.
                         * Они могут быть в процессе удалены,
                         * поэтому нужно удостовериться, что они остались в списке.
                         */

                        else if (logRecord.Action == CategoryWithManyMaterials.LogAction.Added)
                            xmlCategory.AppendChild(DocumentExtensions.ToXmlElement
                                (logRecord.Target.WriteToXmlAsXElement(), xmlProjectDoc));
                    }

                    if (manyCat.SelectedMaterials.Count == 0)
                        xmlCategory.SetAttribute("is_loaded", false.ToString());
                    else
                        xmlCategory.SetAttribute("is_loaded", true.ToString());
                }

                void InvestigateMaterialOnChangesAndWriteToXml(Material material, int i = 0)
                {
                    XmlElement xmlMaterial = (XmlElement)xmlCategory.SelectNodes(materialsType)[i];
                    if (material.HasAnyChanges)
                    {
                        // Перезапись пунктов сметы.

                        XmlNodeList xmlsEstimateItems = xmlMaterial.SelectNodes("est_items/est_item");
                        int k = 0;
                        EstimateItem eItem;
                        XmlElement xmlEItem,
                                   xmlEIPrice,
                                   xmlEIQuantity,
                                   xmlEIMarkup,
                                   xmlEISummary;
                        foreach (string eItemName in material.HasChangeIn.Keys.Except
                                 (new string[] { "Quantity", "HasFilm" }) )
                        {
                            if (material.HasChangeIn[eItemName])
                            {
                                eItem = material.EstimateItems.First(ei => ei.Name == eItemName);
                                xmlEItem = (XmlElement)xmlsEstimateItems[k];

                                if (eItem.HasChangeIn["Price"])
                                {
                                    xmlEIPrice = (XmlElement)xmlEItem.SelectSingleNode("price");
                                    xmlEIPrice.InnerText = eItem.Price.ToString();
                                }

                                if (eItem.HasChangeIn["PhysicalQuantity"])
                                {
                                    xmlEIQuantity = (XmlElement)xmlEItem.SelectSingleNode("quantity");
                                    xmlEIQuantity.InnerText = eItem.PhysicalQuantity.ToString();
                                }

                                if (eItem.HasChangeIn["Markup"])
                                {
                                    xmlEIMarkup = (XmlElement)xmlEItem.SelectSingleNode("markup");
                                    xmlEIMarkup.InnerText = eItem.Markup.ToString();
                                }

                                xmlEISummary = (XmlElement)xmlEItem.SelectSingleNode("summary");
                                xmlEISummary.InnerText = eItem.Summary.ToString();
                            }

                            k++;
                        }

                        /*
                         * Изменение спецпараметров материала.
                         */

                        if (material is EnumerableMaterial)
                        {
                            if (material.HasChangeIn["Quantity"])
                            {
                                XmlElement xmlQuantity = (XmlElement)xmlMaterial.SelectSingleNode("quantity");
                                if (xmlQuantity == null)
                                {
                                    xmlQuantity = xmlProjectDoc.CreateElement("quantity");
                                    xmlMaterial.AppendChild(xmlQuantity);
                                }
                                xmlQuantity.InnerText = ((EnumerableMaterial)material).Quantity.ToString();
                            }

                            if (material is WindowMaterial && material.HasChangeIn["HasFilm"])
                            {
                                XmlAttribute xmlHasFilm = xmlMaterial.GetAttributeNode("has_film");
                                if (!xmlMaterial.HasAttribute("has_film"))
                                {
                                    xmlHasFilm = xmlProjectDoc.CreateAttribute("has_film");
                                    xmlMaterial.SetAttributeNode(xmlHasFilm);
                                }
                                xmlHasFilm.InnerText = ((WindowMaterial)material).HasFilm.ToString();
                            }
                        }
                    }

                    XmlElement xmlSummary = (XmlElement)xmlMaterial.SelectSingleNode("summary");
                    if (xmlSummary == null)
                    {
                        xmlSummary = xmlProjectDoc.CreateElement("summary");
                        xmlMaterial.AppendChild(xmlSummary);
                    }
                    xmlSummary.InnerText = material.Summary.ToString();
                }
            }

            /*
             * Обновление логов изменений материалов у многоматериальных категорий.
             */

            List<CategoryWithManyMaterials> manyCats =
                (from manyCatPath in projHasChangesInModel
                                    .Where(ch => ch.Value).Select(ch => ch.Key)
                select EcohouseProjectViewModel.EcoProj
                        .FindCategory(manyCatPath)).Where(cat => cat is CategoryWithManyMaterials)
                .Cast<CategoryWithManyMaterials>().ToList();
                    
            foreach (CategoryWithManyMaterials manyCat in manyCats)
                manyCat.RefreshMaterialsLog();

            ((XmlElement)xmlProjectDoc.DocumentElement.SelectSingleNode(@"house/roof"))
                .SetAttribute("rooftype", EcohouseProjectViewModel.EcoProj.SelectedRoofType.ToString());

            xmlProjectDoc.Save(CurrentProjectPath);

            EcohouseProjectViewModel.EcoProj.ClearChanges();
        }

        /* 
         * Сохранение изменений в проекта:
         *   -- после выбора материала, добавления/удаления окна, отметки чекбокса,
         * ввода какого-либо значения на странице заполнения ТЗ;
         *   -- после изменения шапочного параметра или изменения параметров
         * пунктов сметы на странице расчёта.
         */

        private void SaveProjectAs(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveProjDialog = new SaveFileDialog();
            saveProjDialog.Filter = "Файлы проекта \"Экодом\"|*.ecoproj";
            saveProjDialog.DefaultExt = "ecoproj";
            saveProjDialog.Title = "Открыть проект";
            //saveProjDialog.CheckPathExists = true;
            //saveProjDialog.InitialDirectory = "";

            if (saveProjDialog.ShowDialog() == false)
                return;

            string newProjectName = saveProjDialog.FileName;
            CurrentProjectPath = newProjectName;

            string pathProjectPattern = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            pathProjectPattern = pathProjectPattern.Remove(pathProjectPattern.IndexOf(@"\bin\Debug")) + @"\ProjectPattern.xml";
            XmlDocument xmlProjectDoc = new XmlDocument();
            xmlProjectDoc.Load(pathProjectPattern);

            // Изменение метаданных о проекте.

            ObservableDictionary<string, string> metadata = EcohouseProjectModel.Metadata;
            XmlElement xmlMetaInfo;
            foreach (string metaInfoKey in metadata.Keys)
            {
                xmlMetaInfo = (XmlElement)xmlProjectDoc.DocumentElement
                    .SelectSingleNode(@"meta/" + metaInfoKey);
                xmlMetaInfo.InnerText = metadata[metaInfoKey];
            }

            XmlElement xmlParam;
            foreach (string paramName in EcohouseProjectModel.CalculationHeaderParams.Keys)
            {
                xmlParam = (XmlElement)
                    xmlProjectDoc.DocumentElement.SelectSingleNode(@"house_params/" + paramName);
                xmlParam.InnerText = EcohouseProjectModel.CalculationHeaderParams[paramName].ToString();
            }
            
            foreach (TechTaskCategory cat in EcohouseProjectViewModel.EcoProj.FindAllLeafs())
            {
                string catPath = cat.GetAbsolutePath();
                XmlElement xmlCategory = (XmlElement)
                    xmlProjectDoc.DocumentElement.SelectSingleNode(@"house/" + catPath);

                if (cat is CategoryWithSingleMaterial)
                {
                    Material mat = ((CategoryWithSingleMaterial)cat).SelectedMaterial;

                    if (mat.Name != null)
                    {
                        xmlCategory.SetAttribute("is_loaded", true.ToString());
                        xmlCategory.AppendChild(DocumentExtensions
                            .ToXmlElement(mat.WriteToXmlAsXElement(), xmlProjectDoc));
                    }
                    else
                        xmlCategory.SetAttribute("is_loaded", false.ToString());
                }
                else
                {
                    ObservableCollection<Material> mats = ((CategoryWithManyMaterials)cat)
                        .SelectedMaterials;

                    if (mats.Count != 0)
                    {
                        xmlCategory.SetAttribute("is_loaded", true.ToString());
                        foreach (Material mat in mats)
                            xmlCategory.AppendChild(DocumentExtensions
                                .ToXmlElement(mat.WriteToXmlAsXElement(), xmlProjectDoc));
                    }
                    else
                        xmlCategory.SetAttribute("is_loaded", false.ToString());
                }
            }

            ((XmlElement)xmlProjectDoc.DocumentElement.SelectSingleNode(@"house/roof"))
                .SetAttribute("rooftype", EcohouseProjectViewModel.EcoProj.SelectedRoofType.ToString());

            XmlDataProvider xmlProjectProvider = ((XmlDataProvider) 
                this.Resources["ProjectProvider"]);
            xmlProjectProvider.Document = xmlProjectDoc;

            xmlProjectDoc.Save(newProjectName);

            isProjectNew = false;

            // Оповещение компонентов вью-модели (конвертеров, выполняющих загрузку модели)
            // о том, что проект НЕ ДОЛЖЕН загружаться.

            //EcohouseProjectViewModel.ProjectMustBeLoaded = false;
        }

        /*
         * Метод, проталкивающийся от контролов, в которых возникло событие прокрутки 
         * (и к которым прикреплён этот обработчик), к скроллвьюеру.
         */

        public void Control_MouseWheelHandler(object sender, MouseWheelEventArgs e)
        {
            //ScrollViewer scv = null;

            /*
            if (sender is StackPanel)
            {
                switch (((StackPanel)sender).Name)
                {
                    case nameof(spTechTask): { scv = scvTechTask; break; }
                    case nameof(spCalculations): { scv = scvCalculations; break; }
                    default: return;
                }
            }
            else
            {
                if (sender is ListBox)
                    e.Handled = false;
                return;
            }
            */

            /*
             * У листбокса есть свой обработчик события прокрутки, поэтому 
             * необходимо указать, что событие прокрутки таки ещё не завершилось.
             */

            if (sender is ListBox)
                e.Handled = false;
            /*
            else if (sender is ListBoxItem)
            {
                ListBoxItem lstbi = (ListBoxItem)sender;
                ContentPresenter lstbiContentPresenter = FindVisualChild<ContentPresenter>(lstbi);
                DataTemplate lstbiDataTemplate = lstbiContentPresenter.ContentTemplate;
                List<Control> childrenControls = new List<Control>();
                UIHelper.FindMatchedChildren<Control>(lstbiDataTemplate.LoadContent(),
                    "", ref childrenControls);
                if (!childrenControls.Contains(_focusedCtrl))
                    return;

                _focusedCtrl.RaiseEvent(new RoutedEventArgs(Mouse.MouseWheelEvent, _focusedCtrl));
                e.Handled = true;

                childItem FindVisualChild<childItem>(DependencyObject obj)
                    where childItem : DependencyObject
                {
                    for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
                    {
                        DependencyObject child = VisualTreeHelper.GetChild(obj, i);
                        if (child != null && child is childItem)
                        {
                            return (childItem)child;
                        }
                        else
                        {
                            childItem childOfChild = FindVisualChild<childItem>(child);
                            if (childOfChild != null)
                                return childOfChild;
                        }
                    }
                    return null;
                }
            }
            */

            /*
            if (!(sender is ScrollViewer))
                return;

            scv = (ScrollViewer)sender;

            this.Dispatcher.Invoke(() => scv.ScrollToVerticalOffset(scv.VerticalOffset - e.Delta));
            scv.UpdateLayout();
            e.Handled = true;*/
        }

        private void AddWindow(object sender, RoutedEventArgs e)
        {
            CategoryWithManyMaterials windowsCategory = null;

            if (((Button)sender).Name == nameof(btnAddWindowIn1stFloor))
            {
                windowsCategory = (CategoryWithManyMaterials)
                    EcohouseProjectViewModel.EcoProj.FindCategory("floors/floor1/windows");
            }
            if (((Button)sender).Name == nameof(btnAddWindowIn2stFloor))
            {
                windowsCategory = (CategoryWithManyMaterials)
                    EcohouseProjectViewModel.EcoProj.FindCategory("floors/floor2/windows");
            }

            windowsCategory.AddNewMaterial();
        }

        private void ProjectManager_ShowDialog(object sender, RoutedEventArgs e)
        {
            (new CustomProjectManager()).ShowDialog();
        }

        private void DeleteWindow(object sender, RoutedEventArgs e)
        {
            //var t1 = ((Button)sender).TemplatedParent;
            //var t2 = ((ListBoxItem)((Panel)((Button)sender).Parent).TemplatedParent).;
            //StackPanel spWindow = (StackPanel)((Button)sender).Parent;
            //ListBox lstbWindows = (ListBox)spWindow.TemplatedParent;
            //ListBoxItem lstbiWindow = (ListBoxItem)spWindow.Parent;
            WindowMaterial window = (WindowMaterial)((Button)sender).DataContext;
            CategoryWithManyMaterials windowsCategory = (CategoryWithManyMaterials)
                window.ParentCategory;
            windowsCategory.SelectedMaterials.Remove(window);
        }
    }
}
