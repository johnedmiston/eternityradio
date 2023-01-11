using EternityRadioApp.Model;
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
    public partial class DownloadCategoryPage : ContentPage
    {
        public DownloadCategoryPage()
        {
            InitializeComponent();
        }

        private DownloadCategoryViewModel Context
        {
            get { return BindingContext as DownloadCategoryViewModel; }
        }
        protected override void OnAppearing()
        {
            CrossMediaManager.Current.Stop();
            Context.SelectedCategory = null;   
            base.OnAppearing();
        }

        private void CategoryContent_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            if (e.CurrentSelection.Count == 0)
                return;

            var category = e.CurrentSelection[0] as Category;

            if (category.Count > 1)
            {
                var viewModel = new DownloadViewModel(category);
                var downloadPage = new DownloadPage { BindingContext = viewModel };

                Navigation.PushAsync(downloadPage, true);
            }
            else
            {
                //var viewModel = new DownloadSeriesViewModel(category.Series[0]);
                // var seriesPage = new DownloadSeriesPage { BindingContext = viewModel };
                var viewModel = new MessageViewModel(category.Series[0]);
                var seriesPage = new MessagePage() { BindingContext = viewModel };
                Navigation.PushAsync(seriesPage, true);
            }
        }


        private void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            Navigation.PopAsync();
        }
    }
}