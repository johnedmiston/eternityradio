using EternityRadioApp.Model;
using HtmlAgilityPack;
using MediaManager;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Xml;
using Xamarin.Forms;

namespace EternityRadioApp.ViewModel
{
    public class DownloadSeriesViewModel : BaseViewModel
    {

        public DownloadSeriesViewModel()
        {

        }


        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="series">The series of media</param>
        public DownloadSeriesViewModel(Series series)
        {


            Name = series.Name;
            Author = series.Author;
            ImageSource = series.ImageSource;
            Messages = series.Messages.ToList();
            Documents = series.Documents;

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
            set { imageSource = value;
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
           var media =  Messages.FirstOrDefault(x => x.Url == url);
           var downloadedFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal));
 

           await media.DownloadFileAsync(url, downloadedFilePath);
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
            get { return messages;  }
            set
            {
                messages = value;
                OnPropertyChanged();
            }
            
        }

        public string Name { get; set; }

        public string Author { get; set; }

      
    }
}

