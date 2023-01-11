using Xamarin.Forms;
using EternityRadioApp;
using EternityRadioApp.Content.Settings;
using EternityRadioApp.ViewModel;

namespace EternityRadioApp
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            RegisterRoutes();
        }

        private void RegisterRoutes()
        {
            Routing.RegisterRoute("landing", typeof(LandingPage));
            Routing.RegisterRoute("category", typeof(DownloadCategoryPage));
            Routing.RegisterRoute("message", typeof(MessagePage));
            Routing.RegisterRoute("settings", typeof(SettingsPage));

        

        }
    }
}
