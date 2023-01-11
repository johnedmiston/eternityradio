using EternityRadioApp.Model;
using HtmlAgilityPack;
using MediaManager;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Xml;
using Xamarin.Forms;

namespace EternityRadioApp.ViewModel
{
    public class DownloadViewModel : BaseViewModel
    {

        private int count;
        public int Count
        {
            get { return count; }
            set { count = value;
                OnPropertyChanged();

            }
        }
      

        private string category;
        public string Category
        {
            get { return category; }
            set { category = value;
                OnPropertyChanged();
            }
        }

  

        /// <summary>
        /// Constructor
        /// </summary>
        public DownloadViewModel(Category category)
        {

            Category = category.Name;
            Series = category.Series.ToList();
            Count = Series.Count;

            /*
            if (Series.Count() == 1)
            {
                var viewModel = new DownloadSeriesViewModel(Series[0]);
                var seriesPage = new DownloadSeriesPage() { BindingContext = viewModel };

                var mainPage = Application.Current.MainPage as NavigationPage;
                mainPage.Navigation.PushAsync(seriesPage, true);
            }*/

        }

        private Media selectedSeries;
        public Media SelectedSeries
        {
            get { return selectedSeries; }
            set
            {
                selectedSeries = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Returns a list of media items associated with this series
        /// </summary>
        /// 
        private List<Series> series;
        public List<Series> Series
        {
            get { return series; }
            set
            {
                series = value;
                OnPropertyChanged();


            }

        }




        public ICommand BackCommand => new Command(Back);


        public async void Back()
        {
            var mainPage = Application.Current.MainPage as AppShell;
            await mainPage.Navigation.PopAsync();
        }


    }
}

