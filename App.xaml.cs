using Microsoft.Extensions.DependencyInjection;

namespace PM2E230148
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            MainPage = new NavigationPage(new Login());
        }

    }
}