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

namespace EcohouseManager
{
    /// <summary>
    /// Логика взаимодействия для EditingEstimateItem.xaml
    /// </summary>
    public partial class EditingEstimateItem : Window
    {
        private XEstimateItem xEditingEstimateItem;
        private XEstimateItem xBuffer;

        public EditingEstimateItem(XEstimateItem xEstimateItem)
        {
            InitializeComponent();
            xEditingEstimateItem = xEstimateItem;
            //xBuffer = new XEstimateItem();
            xBuffer.Copy(xEstimateItem);
            this.DataContext = xBuffer;
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            xEditingEstimateItem.Copy(xBuffer);
            this.DialogResult = true;
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }
    }
}
