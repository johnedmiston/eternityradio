using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Xml;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace EternityRadioApp.Model
{
    [Serializable]
    public class PdfDocument : IEquatable<PdfDocument>
    {
        private readonly NLog.ILogger Logger = NLog.LogManager.GetCurrentClassLogger();

        public string Name { get; set; }

        public string Url { get; set; }

        public string Category { get; set; }

        public string Series { get; set; }

        public string FilePath { get; set; }

        public int Sequence { get; set; }

        public PdfDocument()
        {

        }

        public PdfDocument(XmlNode node, string category, string series)
        {
            Logger.Trace("PdfDocument.PdfDocument");
            Name = node.Attributes["Name"].Value;
            Url = node.Attributes["Url"].Value;
            Category = category;
            Series = series;
            FilePath = Path.Combine(Repository.DownloadPath, Path.GetFileName(Url));
            Sequence = int.Parse(node.Attributes["Sequence"].Value);

        }
        public ICommand DownloadCommand => new Command(Download);
        public async void Download()
        {
            Logger.Trace("PdfDocument.Download()");
            var filePath = await this.DownloadPdfFileAsync();
            //var nav = Application.Current.MainPage.Navigation;
            if (filePath != null)
            {
                await Launcher.OpenAsync(new OpenFileRequest
                {
                    File = new ReadOnlyFile(filePath)
                });
            }
            // await OpenBrowser(new Uri("file://" + filePath));
                //await OpenBrowser(new Uri(Url));
                //await nav.PushAsync(new PdfJsPage(filePath, Name));

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

        public async Task<string> DownloadPdfFileAsync()
        {
            Logger.Trace("PdfDocument.DownloadPdfFileAsync()");

            if (File.Exists(FilePath))
                return FilePath;

            try
            {
                var httpClient = new HttpClient();
                var pdfBytes = await httpClient.GetByteArrayAsync(Url);

                File.WriteAllBytes(FilePath, pdfBytes);

                return FilePath;
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message);
                Logger.Error(ex.StackTrace);
                await Application.Current.MainPage.DisplayAlert("Error", ex.Message, "Ok");
            }

            return null;
        }


        public bool Equals(PdfDocument other)
        {
            return Name.ToUpper() == other.Name.ToUpper()
                && Url.ToUpper() == Url.ToUpper();
        }
    }
}
