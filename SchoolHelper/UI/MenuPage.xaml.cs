namespace SchoolHelper
{
    public partial class MenuPage : ContentPage
    {
        public event EventHandler ItemSelected;

        public MenuPage()
        {
            InitializeComponent();
        }

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();
            if (BindingContext is MainViewModel vm)
            {
                vm.PropertyChanged += (s, e) =>
                {
                    if (e.PropertyName == nameof(MainViewModel.CurrentShoppingList))
                    {
                        ItemSelected?.Invoke(this, EventArgs.Empty);
                    }
                };
            }
        }
    }
}