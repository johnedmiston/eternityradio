using EternityRadioApp.Model;
using EternityRadioApp.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EternityRadioApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MainPage : ContentPage
    {

        private readonly NLog.ILogger Logger = NLog.LogManager.GetCurrentClassLogger();


        public MainPage()
        {
            InitializeComponent();

            Logger.Debug("Main Page Loaded");
        }

        public MainViewModel Context
        {
            get { return BindingContext as MainViewModel; }
        }

        protected override void OnAppearing()
        {
           
            Context.SelectedMenu = null;
            base.OnAppearing();

        }

        private void MenuContent_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            if (e.CurrentSelection.Count == 0)
                return;

            var menu = e.CurrentSelection[0] as AppMenu;

            if (menu.Name == "Radio Streams")
                Navigation.PushAsync(new StreamPage(), true);
            else
                Navigation.PushAsync(new DownloadCategoryPage(), true);
        }
    }
}