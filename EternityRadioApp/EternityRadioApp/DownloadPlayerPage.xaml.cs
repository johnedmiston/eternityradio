using EternityRadioApp.ViewModel;
using MediaManager;
using NLog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EternityRadioApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DownloadPlayerPage : ContentPage
    {
        private readonly NLog.ILogger Logger = NLog.LogManager.GetCurrentClassLogger();

        public DownloadPlayerPage()
        {
            Logger.Trace("DownloadPlayerPage.DownloadPlayerPage()");

            InitializeComponent();


            slider.ValueChanged -= Slider_ValueChanged;
            slider.ValueChanged += Slider_ValueChanged;
            
        }

        private void Slider_ValueChanged(object sender, ValueChangedEventArgs e)
        {
            Logger.Trace("DownloadPlayerPage.Slider_ValueChanged");

            double value = e.NewValue - e.OldValue; 
            if (Math.Abs(value) > 2)
                CrossMediaManager.Current.SeekTo(TimeSpan.FromSeconds(e.NewValue));
        }

        public DownloadPlayerViewModel Context
        {
            get { return BindingContext as DownloadPlayerViewModel; }
        }

        protected override void OnAppearing()
        {
           
            Logger.Trace("DownloadPlayerPage.OnAppearing()");
            try
            {
                // Added 1ms to ensure the android slider updates the ui


                Context.HookUpEvents();
            }
            catch(Exception ex)
            {
                Logger.Error(ex.Message);
                Logger.Error(ex.StackTrace);
            }
            base.OnAppearing();

        }
        protected override void OnDisappearing()
        {
            Logger.Trace("DownloadPlayerPage.OnDisappearing()");

            base.OnDisappearing();
        }

       




        private void TapGestureRecognizer_Tapped_2(object sender, EventArgs e)
        {
            Logger.Trace("DownloadPlayerPage.TapGestureRecognizer_Tapped_2()");

            Navigation.PopAsync();
               
            

        }
    
   
    }
}