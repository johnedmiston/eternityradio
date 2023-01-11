using EternityRadioApp.Model;
using EternityRadioApp.ViewModel;
using MediaManager;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EternityRadioApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class StreamPage : ContentPage
    {
        public StreamPage()
        {
            InitializeComponent();
        }

        public StreamViewModel Context
        {
            get { return BindingContext as StreamViewModel; }
        }

        protected override void OnAppearing()
        {

            CrossMediaManager.Current.Stop();
            Repository.Current.UnHookMedia();
            Context.SelectedStream = null;
          
            base.OnAppearing();


        }

        private void MenuContent_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            if (e.CurrentSelection.Count == 0)
                return;

            var media = e.CurrentSelection[0] as Media;
            


            var viewModel = new StreamPlayerViewModel(media);
            var playerPage = new StreamPlayerPage { BindingContext = viewModel };
           
            Navigation.PushAsync(playerPage, true);

        }
    }
}