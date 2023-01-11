using EternityRadioApp.Model;
using EternityRadioApp.ViewModel;
using MediaManager;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EternityRadioApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DownloadSeriesPage : ContentPage
    {
        public DownloadSeriesPage()
        {
            InitializeComponent();

           
        }

        public DownloadSeriesViewModel Context
        {
            get { return BindingContext as DownloadSeriesViewModel;  }
        }


        protected override void OnAppearing()
        {
            CrossMediaManager.Current.Stop();
            Repository.Current.UnHookMedia();
            Title = Context.Name;
            Context.SelectedMedia = null;
            DocumentContent.SelectedItem = null;
            base.OnAppearing();


        }
        private void SeriesDownloadContent_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.CurrentSelection.Count == 0)
                return;

            var media = e.CurrentSelection[0] as Media;

            if (media.IsDownloading)
                return;

            var viewModel = new DownloadPlayerViewModel(media);         
            var playerPage = new DownloadPlayerPage() { BindingContext = viewModel };

            Navigation.PushAsync(playerPage, true);
        }

        private void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            
            Navigation.PopAsync();
        }

        private async void DocumentContent_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.CurrentSelection.Count > 0)
            {
                var pdf = e.CurrentSelection[0] as PdfDocument;
                if (pdf != null)
                {
                    var filePath = await pdf.DownloadPdfFileAsync();

                    if (filePath != null)
                        await Navigation.PushAsync(new PdfJsPage(filePath, pdf.Name));
                }
            }
        }
    }
}