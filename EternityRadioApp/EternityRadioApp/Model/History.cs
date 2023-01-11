using EternityRadioApp.ViewModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using System.Windows.Input;
using System.Xml.Serialization;
using Xamarin.Forms;

namespace EternityRadioApp.Model
{
    public class History : IEquatable<History>
    {
        public History()
        {

        }
        public History(Media media)
        {
            Name = media.Name;
            Series = media.Series;
            Category = media.Category;
            Author = media.Author;
            Image = media.Image;
            
          
        }

      
        public string Name { get; set; }

        public string Image { get; set; }

        public string Series { get; set; }

        public string Category { get; set; }

        public string Author { get; set; }

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

        public ICommand RecentCommand => new Command(Recent);
        public async void Recent()
        {

            var nav = Application.Current.MainPage.Navigation;

            var media = Repository.Current.GetMediaFromHistory(Category, Series, Name);

            if (media != null) {
                var playerPage = new DownloadPlayerPage { BindingContext = new DownloadPlayerViewModel(media) };
                await nav.PushAsync(playerPage, true);
            } 
            else
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Message is no longer available.", "Ok");

            }
        }


        public bool Equals(History other)
        {
            return Name.ToUpper() == other.Name.ToUpper() &&
                Series.ToUpper() == other.Series.ToUpper() &&
                Category.ToUpper() == other.Category.ToUpper();
        }
    }
}
