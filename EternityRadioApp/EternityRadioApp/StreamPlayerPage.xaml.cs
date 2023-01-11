using EternityRadioApp.ViewModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EternityRadioApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class StreamPlayerPage : ContentPage
    {
        public StreamPlayerPage()
        {
            InitializeComponent();
        }

        public StreamPlayerViewModel Context
        {
            get { return BindingContext as StreamPlayerViewModel; }
        }

        private void TapGestureRecognizer_Tapped(object sender, System.EventArgs e)
        {
            if (Context.Ready)
                Navigation.PopAsync();

            
           
        }

       

        protected override void OnAppearing()
        {
            Context.HookUpEvents();
            base.OnAppearing();
        }

        protected override void OnDisappearing()
        {     
            base.OnDisappearing();
        }

    }
}