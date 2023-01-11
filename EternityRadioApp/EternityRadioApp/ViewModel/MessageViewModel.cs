
using EternityRadioApp.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Xml.Linq;
using Xamarin.CommunityToolkit.UI.Views;
using Xamarin.Forms;

namespace EternityRadioApp.ViewModel
{
    public class MessageViewModel : NewBaseViewModel
    {
        private readonly NLog.ILogger Logger = NLog.LogManager.GetCurrentClassLogger();

        private LayoutState _mainState;
        public const int LAZY_LOAD_THRESHOLD = 10;

        
       public string Items
        {
            get
            {
                return $"{TotalMessages} message(s)";
            }
        }
        private int _totalMessages = 0;
        public int TotalMessages
        {
            get { return _totalMessages; }
            set { _totalMessages = value;
                OnPropertyChanged();
                OnPropertyChanged("HasMoreMessages");
            }
        }

        private int _shownItems = 0;
        public int ShownItems
        {
            get { return _shownItems; }
            set
            {
                _shownItems = value;
                OnPropertyChanged();
                OnPropertyChanged("HasMoreMessages");
            }
        }

        
        public bool HasMoreMessages
        {
            get { return _totalMessages - _shownItems > 0; }
        }

        public LayoutState MainState
        {
            get => _mainState;
            set => SetProperty(ref _mainState, value);
        }

        

        public MessageViewModel()
        {

            Logger.Trace("MessageViewModel.MessageViewModel()");
            try
            {
                var series = Repository.Current.Categories.FirstOrDefault().Series.FirstOrDefault();

                if (series != null)
                    LoadModel(series);
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message);
                Logger.Error(ex.StackTrace);
                throw;
            }
            //UpdateCurrentImage();
        }

        private void LoadModel(Series series)
        {
            Logger.Trace("MessageViewModel.LoadModel()");

            Name = series.Name;
            Author = series.Author;
            ImageSource = series.ImageSource;
            int msgCount = series.Messages.Count();
            if (msgCount <= LAZY_LOAD_THRESHOLD)
            {
                
                Messages = series.Messages.ToList();
                
                ShownItems = Messages.Count;
            } 
            else
            {
                Messages = series.Messages.Take(LAZY_LOAD_THRESHOLD).ToList();
                ShownItems = Messages.Count;

            }
            TotalMessages = msgCount;
            Documents = series.Documents;
            Summary = series.Summary;

        }


        public Series Series;
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="series">The series of media</param>
        public MessageViewModel(Series series)
        {
            try
            {
                Series = series;
                Logger.Trace("MessageViewModel.MessageViewModel()");
                Logger.Debug($"MessageViewModel Parameter: [series]{series}");
                LoadModel(series);

            }
            catch(Exception ex)
            {
                Logger.Error("Could not create MessageViewModel instance");
                Logger.Error(ex.Message);
                Logger.Error(ex.StackTrace);
                throw;
            }
            

        }

        private List<PdfDocument> documents;
        public List<PdfDocument> Documents
        {
            get { return documents; }
            set
            {
                documents = value;
                OnPropertyChanged();
            }
        }


        private ImageSource imageSource;
        public ImageSource ImageSource
        {
            get { return imageSource; }
            set
            {
                imageSource = value;
                OnPropertyChanged();
            }
        }

       

        public ICommand DownloadCommand
        {
            get
            {
                return new Command<string>((url) => Download(url));
            }
        }
        public async void Download(string url)
        {
            Logger.Trace("MessageViewModel.Download()");
            Logger.Debug($"MessageViewModel Parameter: [url]url");
            try
            {
                var media = Messages.FirstOrDefault(x => x.Url == url);
                var downloadedFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal));


                await media.DownloadFileAsync(url, downloadedFilePath);

            } catch(Exception ex)
            {
                Logger.Error(ex.Message);
                Logger.Error(ex.StackTrace);
                throw;
            }
        }

        private Media selectedMedia;
        public Media SelectedMedia
        {
            get { return selectedMedia; }
            set
            {
                selectedMedia = value;
                OnPropertyChanged();
            }
        }


        /// <summary>
        /// Returns a list of media items associated with this series
        /// </summary>
        /// 
        private List<Media> messages;
        public List<Media> Messages
        {
            get { return messages; }
            set
            {
                messages = value;
                OnPropertyChanged();
            }

        }

        public string Name { get; set; }

        public string Author { get; set; }


    
        public string Summary { get; set; }




        public ICommand RefreshCommand => new Command(async () => await OnRefreshAsync());

        private async Task OnRefreshAsync()
        {
            Logger.Trace("MessageViewModel.OnRefreshAsync()");
            try
            {
                MainState = LayoutState.Loading;
                IsBusy = true;
                await Task.Delay(3000);
                IsBusy = false;
                MainState = LayoutState.None;
            }
            catch(Exception ex)
            {
                Logger.Error(ex.Message);
                Logger.Error(ex.StackTrace);
            }
        }


    }
}
