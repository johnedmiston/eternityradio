using MediaManager;
using EternityRadioApp.Model;
using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;
using System.Linq;
using MediaManager.Player;
using System.Collections.Generic;
using System.Diagnostics;

namespace EternityRadioApp.ViewModel
{


    public class DownloadCategoryViewModel : BaseViewModel
    {


        public ICommand BackCommand => new Command(Back);


        public async void Back()
        {
            var mainpage = Application.Current.MainPage as AppShell;  
            await mainpage.Navigation.PopAsync();
        }

        public DownloadCategoryViewModel()
        {
           Categories = Repository.Current.Categories.ToList();
        }

        #region Properties

        private Category selectedCategory;
        public Category SelectedCategory
        {
            get { return selectedCategory; }
            set { selectedCategory = value;
                OnPropertyChanged(); }

        }

    private List<Category> categories;
     public List<Category> Categories
        {
            get 
            {
                return categories;
            }
            set
            {
                categories = value;
                OnPropertyChanged();
            }
        }

    
    #endregion

      


        
       

       
       

       
        


        
    }
}
