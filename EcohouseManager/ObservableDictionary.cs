using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace EcohouseManager
{
    // Мне нужен тип, состоящий из двух изменяемых переменных.
    // Такого нет.
    // Поэтому вот мой.

    public class KeyValueTuple<TKey, TValue>
    {
        public TKey Key { get; set; }

        public TValue Value { get; set; }

        public KeyValueTuple(TKey key, TValue value)
        {
            Key = key;
            Value = value;
        }
    }

    public class ObservableDictionary<TKey, TValue> : ObservableCollection<KeyValueTuple<TKey, TValue>>,
                                                      INotifyPropertyChanged
        where TKey : IEquatable<TKey>
        where TValue : IEquatable<TValue>
    {
        // Внутренний словарь.

        private Dictionary<TKey, TValue> _content;

        // Индексатор, дающий доступ к значениям словаря по ключу.

        public TValue this[TKey key]
        {
            get { return _content[key]; }
            set
            {
                if (!_content[key].Equals(value))
                {
                    _content[key] = value;
                    this.First(kvp => kvp.Key.Equals(key)).Value = value;
                    OnPropertyChanged(key);
                }
            }
        }

        // Простой доступ к ключам словаря.

        public IEnumerable<TKey> Keys { get { return _content.Keys; } }

        // Простой доступ к значениям словаря.

        public IEnumerable<TValue> Values { get { return _content.Values; } }

        // Конструктор.

        public ObservableDictionary() : base()
        {
            _content = new Dictionary<TKey, TValue>();
            this.CollectionChanged += (s, e) =>
            {
                switch (e.Action)
                {
                    case System.Collections.Specialized.NotifyCollectionChangedAction.Reset:
                        {
                            _content.Clear();
                            break;
                        }
                    case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
                        {
                            foreach (KeyValueTuple<TKey, TValue> newItem in
                                     e.NewItems.Cast<KeyValueTuple<TKey, TValue>>())
                                _content.Add(newItem.Key, newItem.Value);
                            break;
                        }
                    case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
                        {
                            foreach (KeyValueTuple<TKey, TValue> oldItem in
                                     e.OldItems.Cast<KeyValueTuple<TKey, TValue>>())
                                _content.Remove(oldItem.Key);
                            break;
                        }
                }
            };
        }

        // Перегрузка метода Add.

        public void Add(TKey key, TValue value)
        {
            this.Add(new KeyValueTuple<TKey, TValue>(key, value));
        }

        // Перегрузка метода Remove.

        public void Remove (TKey key)
        {
            if (!_content.ContainsKey(key))
                return;

            this.Remove(this.First(i => i.Key.Equals(key)));
        }

        // Попытка получения значения из словаря.

        public bool TryGetValue (TKey key, out TValue value)
        {
            return _content.TryGetValue(key, out value);
        }

        // Поиск ключа в словаре.

        public bool ContainsKey(TKey key)
        {
            return _content.ContainsKey(key);
        }

        // Уведомление подписчиков на событие изменения свойства.

        public new event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(TKey prop)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop.ToString()));
        }
    }
}
