using System.Windows.Input;
using Xamarin.Forms;
using EternityRadioApp.Content;
using EternityRadioApp.ViewModel;

namespace EternityRadioApp.Scenarios
{
    public class IndexPageViewModel : NewBaseViewModel
    {
        private string selectedItem;

        public ICommand SelectCommand { get; set; }

        public IndexPageViewModel()
        {
            SelectCommand = new Command(Selected);
        }

        private async void Selected()
        {
            if (SelectedItem == null)
                return;

            string targetPage = "landing";
            switch (SelectedItem.ToLower())
            {
                case "product details":
                    targetPage = "message";
                    break;
                case "map":
                    targetPage = "map";
                    break;
                case "login":
                    targetPage = "login";
                    break;
                case "other login":
                    targetPage = "otherlogin";
                    break;
                case "to do list":
                    targetPage = "todo";
                    break;
                case "conversational":
                    targetPage = "conversation";
                    break;
                case "onboarding":
                    targetPage = "onboarding";
                    break;
                case "photo gallery":
                    targetPage = "photogallery";
                    break;
                default:
                    break;

            }

            SelectedItem = null;
            await Shell.Current.GoToAsync($"{targetPage}");
        }

        public string SelectedItem
        {
            get => selectedItem;
            set => SetProperty(ref selectedItem, value);
        }
    }

}
