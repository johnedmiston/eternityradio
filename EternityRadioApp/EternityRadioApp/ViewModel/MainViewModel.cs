using EternityRadioApp.Model;
using HtmlAgilityPack;
using MediaManager;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Windows.Input;
using System.Xml;
using Xamarin.Forms;

namespace EternityRadioApp.ViewModel
{
    public class MainViewModel : BaseViewModel
    {

  

        public MainViewModel()
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
                     Image = "EternityRadioApp.Images.1024.png",
                 }
            };

        }


       

        private Menu selectedMenu;
        public Menu SelectedMenu
        {
            get { return selectedMenu; }
            set
            {
                selectedMenu = value;
                OnPropertyChanged();
            }

        }

        private List<AppMenu> menuList;
        public List<AppMenu> MenuList
        {
            get
            {
                return menuList;
            }
            set
            {
                menuList = value;
                OnPropertyChanged();
            }
        }



    }
}

