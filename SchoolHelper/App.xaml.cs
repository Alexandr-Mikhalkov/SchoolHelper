using System.Globalization;

namespace SchoolHelper
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            var culture = new CultureInfo("be-BY");
            CultureInfo.DefaultThreadCurrentCulture = culture;
            CultureInfo.DefaultThreadCurrentUICulture = culture;
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(new MainWindow());
        }
    }
}