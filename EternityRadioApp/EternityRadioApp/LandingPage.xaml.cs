using EternityRadioApp.ViewModel;
using MediaManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EternityRadioApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LandingPage : ContentPage
    {

       

        public LandingPageViewModel Context
        {
            get
            {
                return BindingContext as LandingPageViewModel;
            }
        }

        public LandingPage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            CrossMediaManager.Current.Stop();
            Repository.Current.UnHookMedia();


            if (Repository.Current.Setting<bool>("WarnDataUsage"))
            {
                Application.Current.MainPage.DisplayAlert("Welcome!", "Thank you for using Eternity Radio. Please note that listening to our content over your 3g/4g connection may result in high data plan usage.", "Ok");
                Repository.Current.Setting("WarnDataUsage", false);
            }

            Context.RecentContent = Repository.Current.Histories;
            Context.RefreshCommand.Execute(null);



             //  (BindingContext as LandingPageViewModel).SkeletonCommand.Execute(null);
        }

       
    }
}