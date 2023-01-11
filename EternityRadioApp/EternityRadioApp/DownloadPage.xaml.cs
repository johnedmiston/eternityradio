using EternityRadioApp.Model;
using EternityRadioApp.ViewModel;
using MediaManager;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EternityRadioApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DownloadPage : ContentPage
    {
        public DownloadPage()
        {
            InitializeComponent();
        }
        public DownloadViewModel Context
        {
            get { return BindingContext as DownloadViewModel; }
        }
        protected override void OnAppearing()
        {

            CrossMediaManager.Current.Stop();

            Title = Context.Category;
            Context.SelectedSeries = null;

            base.OnAppearing();
         

        }

        private void SeriesContent_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            if (e.CurrentSelection.Count == 0)
                return;

            var series = e.CurrentSelection[0] as Series;

            if (series != null)
            {
                //var viewModel = new DownloadSeriesViewModel(series);
                //var seriesPage = new DownloadSeriesPage() { BindingContext = viewModel };

                var viewModel = new MessageViewModel(series);
                var msgPage = new MessagePage() { BindingContext=viewModel};
                Navigation.PushAsync(msgPage, true);

            }
        }
        

        private void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            Navigation.PopAsync();
        }

        private void TapGestureRecognizer_Tapped_1(object sender, EventArgs e)
        {
            Navigation.PopAsync();
        }
    }
}