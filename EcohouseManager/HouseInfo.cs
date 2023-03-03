using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Xml;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace EcohouseManager
{
    // Параметры дома.

    public class HouseInfo : INotifyPropertyChanged
    {
        private decimal _length;
        private decimal _width;
        private decimal _pogonajRostverka;

        // Длина дома.

        public decimal Length
        {
            get { return _length; }
            set
            {
                _length = value;
                OnPropertyChanged("Length");
            }
        }

        // Ширина дома.

        public decimal Width
        {
            get { return _width; }
            set
            {
                _width = value;
                OnPropertyChanged("Width");
            }
        }

        // Погонаж ростверка.

        public decimal PogonajRostverka
        {
            get { return _pogonajRostverka; }
            set
            {
                _pogonajRostverka = value;
                OnPropertyChanged("PogonajRostverka");
            }
        }

        // Уведомление подписчиков на событие изменения свойства.

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

        // Информация об этажах.

        public FloorInfo FloorAtGround { get; private set; }
        public FloorInfo FloorUnderRoof { get; private set; }

        // Конструктор.

        public HouseInfo(decimal length, decimal width, decimal pogonaj,
                         int floorsCount)
        {
            Length = length;
            Width = width;
            PogonajRostverka = pogonaj;
        }

        // Конструктор, принимающий на вход xml-файл с информацией
        // о параметрах дома.

        public HouseInfo(XmlNode xmlHouse)
        {
            Length = decimal.Parse(xmlHouse.SelectSingleNode("length").InnerText);
            Width = decimal.Parse(xmlHouse.SelectSingleNode("width").InnerText);
            PogonajRostverka = decimal.Parse(xmlHouse.SelectSingleNode("pogonaj")
                                             .InnerText);

            XmlNodeList xmlFloors = xmlHouse.SelectNodes("floor");
            FloorAtGround = new FloorInfo(xmlFloors[0]);
            FloorUnderRoof = new FloorInfo(xmlFloors[0]);
        }

        // Представление данных в виде словаря для калькулятора.

        public Dictionary<string, decimal> AsDictionary()
        {
            Dictionary<string, decimal> projParams = new Dictionary<string, decimal>
            {
                { "length", Length },
                { "width", Width },
                { "pogon_rostw", PogonajRostverka },
                { "fl1_height", FloorAtGround.Height },
                { "fl1_pogon_NS", FloorAtGround.PogonajNesuschihSten },
                { "fl1_pogon_NeNS", FloorAtGround.PogonajNenesuschihSten },
                { "fl2_height", FloorUnderRoof.Height },
                { "fl2_pogon_NS", FloorUnderRoof.PogonajNesuschihSten },
                { "fl2_pogon_NeNS", FloorUnderRoof.PogonajNenesuschihSten },
            };
            return projParams;
        }
    }

    // Информация об этаже.

    public class FloorInfo
    {
        // Высота этажа.

        public decimal Height { get; set; }

        // Погонаж несущих стен.

        public decimal PogonajNesuschihSten { get; set; }

        // Погонаж ненесущих стен.

        public decimal PogonajNenesuschihSten { get; set; }

        // Конструктор, принимающий на вход xml-файл с информацией об этаже.

        public FloorInfo(XmlNode xmlFloor)
        {
            Height = decimal.Parse(xmlFloor.SelectSingleNode("height").InnerText);
            PogonajNesuschihSten = decimal.Parse(xmlFloor
                                                 .SelectSingleNode("pogonaj_nesuschih")
                                                 .InnerText);
            PogonajNenesuschihSten = decimal.Parse(xmlFloor
                                                   .SelectSingleNode("pogonaj_nenesuschih")
                                                   .InnerText);
        }
    }

}
