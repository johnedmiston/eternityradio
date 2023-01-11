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
    public class StreamViewModel : BaseViewModel
    {
        public StreamViewModel()
        {
            StreamList = Repository.Current.Streams;

        }

        public ICommand BackCommand => new Command(Back);


        public async void Back()
        {
            var mainpage = Application.Current.MainPage as AppShell;
            await mainpage.Navigation.PopAsync();
        }


        private Media selectedStream;
        public Media SelectedStream
        {
            get { return selectedStream; }
            set
            {
                selectedStream = value;
                OnPropertyChanged();
            }

        }

        private List<Media> streamList;
        public List<Media> StreamList
        {
            get
            {
                return streamList;
            }
            set
            {
                streamList = value;
                OnPropertyChanged();
            }
        }



    }
}

