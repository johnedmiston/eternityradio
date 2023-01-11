

using EternityRadioApp.ViewModel;
using NLog;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Xml;
using System.Xml.Serialization;
using Xamarin.Forms;
using Xamarin.Forms.Shapes;
using static Xamarin.Essentials.Permissions;
using Path = System.IO.Path;

namespace EternityRadioApp.Model
{
    public enum DownloadStatus : ushort
    {
        NONE = 0,
        DOWNLOADING = 1,
        DOWNLOADED = 2,
        ERROR = 3
    }


    [Serializable]
    public class Media : IEquatable<Media>, INotifyPropertyChanged
    {
        private readonly NLog.ILogger Logger = NLog.LogManager.GetCurrentClassLogger();

        public Media()
        {

        }

        public Media(XmlNode node, string category, string series)
        {
            try
            {
                Name = node.Attributes["Name"].Value;
                Debug.WriteLine($"Processing media: {Name}");
                Image = node.Attributes["CoverImage"].Value;
                Author = node.Attributes["Author"].Value;
                Summary = node.Attributes["Summary"].Value;

                if (category == null)
                    Debug.WriteLine("Category can't be null");
                Category = category;
                if (string.IsNullOrWhiteSpace(Summary))
                    Summary = $"An insightful and encouraging message from {Author}.";
              
                Url = node.Attributes["Url"].Value;


                try
                {
                    //var db = Repository.Current.Db;

                    Uri uri = new Uri(Url);
                    LocalFile = Path.Combine(Repository.DownloadPath, Path.GetFileName(uri.LocalPath));

                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Invalid url specified for media {Name}");
                    Debug.WriteLine(ex.Message);
                    LocalFile = string.Empty;
                }

                Series = series;
                string sequence = node.Attributes["Sequence"].Value;


                if (!int.TryParse(sequence, out int seq))
                    Debug.WriteLine($"Warning: Invalid sequence for series '{Name}'");

                Sequence = seq;
            } catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            
        }

        public string Name { get; set; }
        public string Author { get; set; }

        public string LocalFile { get; set; }
        public string Url { get; set; }
        public string Image { get; set; } = "Images/EnglishRadio.jpg"; //"https://cybermissions.org/images/cybermissionsfull_400.jpg?crc=3997375289";
        public string Series { get; set; }
        public string Category { get; set; }
        public int Sequence { get; set; }

        private DownloadStatus downloadStatus = DownloadStatus.NONE;
        public DownloadStatus DownloadStatus
        {
            get
            {
                return downloadStatus;
            }
            set
            {
                downloadStatus = value;
                Downloaded = downloadStatus == DownloadStatus.DOWNLOADED;

                OnPropertyChanged();
                OnPropertyChanged("TextColor");
                OnPropertyChanged("StatusText");
                OnPropertyChanged("IsDownloading");
                OnPropertyChanged("CanDelete");
                
            }
        }

        [XmlIgnore]
        public Color TextColor
        {
            get
            {
                switch (DownloadStatus)
                {
                    case DownloadStatus.DOWNLOADED:
                        return Color.Green;
                    case DownloadStatus.DOWNLOADING:
                        return Color.CornflowerBlue;
                    case DownloadStatus.ERROR:
                        return Color.Red;
                    default:
                        return Color.Black;

                }
            }


        }

        [XmlIgnore]
        public string StatusText
        {
            get
            {
                switch (DownloadStatus)
                {
                    case DownloadStatus.NONE:
                    default:
                        return string.Empty;

                    case DownloadStatus.DOWNLOADED:
                        return "DOWNLOADED";
                    case DownloadStatus.DOWNLOADING:
                        return "DOWNLOADING...";
                    case DownloadStatus.ERROR:
                        return "ERROR";
                }
            }

        }

        [XmlIgnore]
        public bool NotDownloaded
        {
            get { return !Downloaded; }
        }

        private bool downloaded = false;
        public bool Downloaded
        {
            get
            {
                if (IsDownloading)
                    return true;
                else
                    return downloaded;
                
            }
            set
            {
                downloaded = value;
                OnPropertyChanged();
                OnPropertyChanged("NotDownloaded");
            }
        }

        [XmlIgnore]
        public bool IsDownloading
        {
            get { return DownloadStatus == DownloadStatus.DOWNLOADING; }
        }

        [XmlIgnore]
        public bool Downloadable
        {
            get { return !string.IsNullOrWhiteSpace(LocalFile); }
           
        }

        [XmlIgnore]
      
        public bool CanDelete
        {

            get { return downloaded; }
          
        }
       

        public string Summary { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string name = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

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

        public bool Equals(Media other)
        {
            return Name.ToUpper() == other.Name.ToUpper() 
                && Author.ToUpper() == other.Author.ToUpper() 
                && Series.ToUpper() == other.Series.ToUpper()
                && Category.ToUpper() == other.Category.ToUpper();
        }

        public ICommand DownloadCommand => new Command(Download);
        public ICommand DeleteCommand => new Command(Delete);
        public ICommand ListenCommand => new Command(Listen);

        public async void Delete()
        {
            if (DownloadStatus != DownloadStatus.DOWNLOADED)
                return;

            bool confirm = await Application.Current.MainPage.DisplayAlert("Delete Message","Are you sure you want to delete this message from your phone?","Yes", "No");

            if (confirm)
            {
                DownloadStatus = DownloadStatus.NONE;
                var result = await DeleteFileAsync(LocalFile);
            }
        }


        public async void Listen()
        {
            Logger.Trace("Media.Listen()");
            if (IsDownloading)
                return;

            try
            {
                var viewModel = new DownloadPlayerViewModel(this);
                var playerPage = new DownloadPlayerPage() { BindingContext = viewModel };

                var nav = Application.Current.MainPage.Navigation;
                await nav.PushAsync(playerPage, true);
            } 
            catch(Exception ex)
            {
                Logger.Error(ex.Message);
                Logger.Error(ex.StackTrace);
            }
        }

      
        public async void Download()
        {

            if (DownloadStatus == DownloadStatus.DOWNLOADING)
                return;

            DownloadStatus = DownloadStatus.DOWNLOADING;
            bool result = await DownloadFileAsync(Url, LocalFile);

           
            DownloadStatus = result ? DownloadStatus.DOWNLOADED : DownloadStatus.ERROR;
 
            Repository.Current.Save();

           
        }

        public async Task<bool> DeleteFileAsync(string downloadedFilePath)
        {



            return await Task.Run(() =>
            {
                try
                {
                    File.Delete(downloadedFilePath);
                    return Task.FromResult(true);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("Failed to delete media");
                    Debug.WriteLine(ex.Message);
                    return Task.FromResult(false);
                }
            });
        }

        public async Task<bool> DownloadFileAsync(string fileUrl, string downloadedFilePath)
        {
            return await Task.Run(async () =>
            {

                try
                {

                    using (var client = new HttpClient())
                    {
                        var downloadStream = await client.GetStreamAsync(fileUrl);



                        using (var fileStream = File.Create(downloadedFilePath))
                            downloadStream.CopyTo(fileStream);
                        return true;
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                    return false;
                }
                
            });
            
        }

           
        
    }
}

