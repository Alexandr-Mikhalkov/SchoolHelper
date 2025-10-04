using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace SchoolHelper
{
    public class ShoppingList : INotifyPropertyChanged
    {
        private string _name;
        public string Name
        {
            get => _name;
            set
            {
                if (_name != value)
                {
                    _name = value;
                    OnPropertyChanged(nameof(Name));
                }
            }
        }

        public ObservableCollection<Item> Items { get; set; }

        public double TotalCost => Items.Where(i => i.IsPurchased).Sum(i => i.Price);

        public ShoppingList()
        {
            Items = new ObservableCollection<Item>();
            Items.CollectionChanged += (s, e) =>
            {
                if (e.NewItems != null)
                {
                    foreach (Item item in e.NewItems)
                        item.PropertyChanged += Item_PropertyChanged;
                }

                if (e.OldItems != null)
                {
                    foreach (Item item in e.OldItems)
                        item.PropertyChanged -= Item_PropertyChanged;
                }

                OnPropertyChanged(nameof(TotalCost));
            };
        }

        private void Item_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Item.IsPurchased) || e.PropertyName == nameof(Item.Price))
            {
                OnPropertyChanged(nameof(TotalCost));
            }
        }

        public void ReattachItemEventHandlers()
        {
            foreach (var item in Items)
            {
                item.PropertyChanged -= Item_PropertyChanged;
                item.PropertyChanged += Item_PropertyChanged;
            }

            OnPropertyChanged(nameof(TotalCost));
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
