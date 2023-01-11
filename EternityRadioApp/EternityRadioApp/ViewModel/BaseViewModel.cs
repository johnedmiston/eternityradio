using EternityRadioApp.Model;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace EternityRadioApp.ViewModel
{
    public class BaseViewModel : INotifyPropertyChanged
    {
        public bool Navigated { get; set; }

       

      

    




       


        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string name = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
