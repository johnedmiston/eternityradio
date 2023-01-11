
using EternityRadioApp.Model;
using NLog.Fluent;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Xml.Linq;
using Xamarin.CommunityToolkit.UI.Views;
using Xamarin.Forms;

namespace EternityRadioApp.ViewModel
{
    public class LandingPageViewModel : NewBaseViewModel
    {
        private LayoutState _mainState;

        public string XmlVersion { get; set; }

        public LayoutState MainState
        {
            get => _mainState;
            set => SetProperty(ref _mainState, value);
        }

        public LandingPageViewModel()
        {
            LoadModel();
        }

        private void LoadModel()
        {
            MenuList = new List<AppMenu>()
            {
                new AppMenu()
                {

                    Summary = "Listen to in-depth bible teaching for training pastors in low-bandwidth locations around the world.",
                    Name = "Radio Streams",
                    Image = "EternityRadioApp.Images.EnglishRadio.jpg",
                },
                 new AppMenu()
                {

                    Summary = "Listen to in-depth bible teaching for training pastors in low-bandwidth locations around the world.",
                    Name = "Downloads",
                    Image = "EternityRadioApp.Images.Downloads.jpg",

                },
                 new AppMenu()
                 {
                     Summary = "Eternity Radio provides in-depth bible teaching in audio formats that are suitable for training pastors in low-bandwidth locations around the world. We also broadcast via shortwave radio across Africa and Latin America.",
                     Name = "EternityRadio.org",
                     Image = "EternityRadioApp.Images.er.png",
                 }
            };

            RecentContent = Repository.Current.Histories;
            XmlVersion = "Xml Ver: " + Repository.Current.XmlVersion + "       Data Date: " + Repository.Current.DataDate;
            

        }

        /// <summary>


        private List<History> recentContent;
        public List<History> RecentContent
        {
            get { return recentContent; }
            set { recentContent = value;
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

       
       

        


        /// <summary>
        /// Returns a list of media items associated with this series
        /// </summary>
        /// 
        private List<AppMenu> menuList;
        public  List<AppMenu> MenuList
        {
            get { return menuList; }
            set
            {
                menuList = value;
                OnPropertyChanged();
            }

        }

        public string Feature { get; set; }

        public string Author { get; set; }


    
        public string Summary { get; set; }


      
       

        public ICommand RefreshCommand => new Command(async () => await OnRefreshAsync());

        private async Task OnRefreshAsync()
        {
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
                Console.Write(ex.Message);
            }
        }


    }
}
