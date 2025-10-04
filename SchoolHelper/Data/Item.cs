using System.ComponentModel;

namespace SchoolHelper
{
    public class Item : INotifyPropertyChanged
    {
        private string _name;
        public string Name
        {
            get => _name;
            set { _name = value; OnPropertyChanged(nameof(Name)); }
        }

        private double _price;
        public double Price
        {
            get => _price;
            set { _price = value; OnPropertyChanged(nameof(Price)); }
        }

        private bool _isPurchased;
        public bool IsPurchased
        {
            get => _isPurchased;
            set { _isPurchased = value; OnPropertyChanged(nameof(IsPurchased)); }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}