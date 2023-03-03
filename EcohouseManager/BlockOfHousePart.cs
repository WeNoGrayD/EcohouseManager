using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Collections.ObjectModel;

namespace EcohouseManager 
{
    /* По идее, потом понадобится связать данные из текстбоксов и меток
       внутри таблицы с самими контролами, посмотрим. */

    public class BlockOfHousePart : INotifyPropertyChanged
    {
        // Грид блока, описывающего часть дома.

        public Grid MaterialsTable { get; set; }

        // Имя материала.

        public string Name { get; set; }
		
		// Итоги по блоку.
		
        private decimal _summary { get; set; }

		public decimal Summary
        {
            get { return _summary; }
            set
            {
                _summary = value;
                OnPropertyChanged("Summary");
            }
        }

        public ObservableCollection<EstimateItem> EstimateItems { get; set; }

        // Обычный конструктор.
        // Принимает на вход грид блока куска дома.

        public BlockOfHousePart(Grid materialsTable,
            string name,
            ObservableCollection<EstimateItem> estimateItems)
        {
            this.MaterialsTable = materialsTable;
            this.Name = name;

            // Привязка свойства итогов по блоку к изменениям итогов у каждого
            // пункта сметы.

            this.EstimateItems = estimateItems;
            EstimateItems.CollectionChanged += (s, e) =>
            {
                foreach (var item in e.NewItems.Cast<EstimateItem>())
                    item.PropertyChanged += (s1, e2) =>
                    {
                        if (e2.PropertyName == "Summary")
                            this.Summary = EstimateItems.Sum(ei => ei.Summary);
                    };
            };

			Summary = 0;
        }

        // Уведомление подписчиков на событие изменения свойства.

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}
