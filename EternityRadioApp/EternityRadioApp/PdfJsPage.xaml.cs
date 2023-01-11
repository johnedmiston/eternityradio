using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EternityRadioApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PdfJsPage : ContentPage
    {
        private readonly NLog.ILogger Logger = NLog.LogManager.GetCurrentClassLogger();

        public PdfJsPage(string url, string title)
        {
            Logger.Trace("PdfJsPage.PdfJsPage()");

            try
            {
                InitializeComponent();

                Title = title;
              // PdfJsWebView = new Controls.PdfJsWebView();
              //  PdfJsWebView.Navigated -= PdfJsWebView_Navigated;
              //  PdfJsWebView.Navigated += PdfJsWebView_Navigated;
              //  PdfJsWebView.Uri = url;
                Repository.Current.PdfJsPage = this;
            }
            catch(Exception ex)
            {
                Logger.Error("Failed to instantiate PdfJsPage");
                Logger.Error(ex.Message);
                Logger.Error(ex.StackTrace);

            }
            
      
        }
        
        protected override void OnDisappearing()
        {
            //PdfJsWebView = null;
            Logger.Trace("PdfJsPage.OnDisappearing()");
            base.OnDisappearing();
        }

        private void PdfJsWebView_Navigated(object sender, WebNavigatedEventArgs e)
        {
            Logger.Trace("PdfJsPage.PdfJsWebView_Navigated()");
               
        }

        
    }
}