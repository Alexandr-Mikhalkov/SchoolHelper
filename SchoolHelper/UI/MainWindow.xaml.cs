namespace SchoolHelper 
{
    public partial class MainWindow : FlyoutPage
    {
        public MainWindow()
        {
            InitializeComponent();

            var viewModel = new MainViewModel();

            menuPage.BindingContext = viewModel;
            (this.Detail as NavigationPage).CurrentPage.BindingContext = viewModel;

            menuPage.ItemSelected += (sender, e) => IsPresented = false;
        }
    }
}