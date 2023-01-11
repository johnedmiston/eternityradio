using EternityRadioApp.ViewModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Xml.Serialization;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace EternityRadioApp.Model
{
    public class AppMenu
    {
        public string Name { get; set; }

        public string Image { get; set; }

        public int Sequence { get; set; }

        public string Summary { get; set; }

    

        [XmlIgnore]
        public ImageSource ImageSource
        {
            get
            {
                try
                {
                    if (Image.ToLower().StartsWith("http"))
                        return ImageSource.FromUri(new Uri(Image));
                    else if (Image.StartsWith("EternityRadioApp"))
                        return ImageSource.FromResource(Image, typeof(Media).GetTypeInfo().Assembly);
                    else
                        return null;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("Failed Parsing Media Image");
                    Debug.WriteLine(ex.Message);
                    return null;
                }
            }
        }
        public ICommand ChooseCommand => new Command(Choose);
        public async void Choose()
        {
  
            var nav = Application.Current.MainPage.Navigation;
            ContentPage page;
            switch (Name) 
            {
                case "Downloads":
                    page = new DownloadCategoryPage() { BindingContext = new DownloadCategoryViewModel() };
                    await nav.PushAsync(page);
                    break;
                case "Radio Streams":
                    page = new StreamPage() { BindingContext = new StreamViewModel() };
                    await nav.PushAsync(page, true);
                    break;
                case "EternityRadio.org":
                    await OpenBrowser(new Uri("https://eternityradio.org"));
                    break;
                
            } 
          
        }

        public async Task OpenBrowser(Uri uri)
        {
            try
            {
                await Browser.OpenAsync(uri, BrowserLaunchMode.SystemPreferred);
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", ex.Message, "Ok");
            }

        }



    }
}
