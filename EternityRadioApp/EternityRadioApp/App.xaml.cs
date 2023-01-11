using EternityRadioApp.Domain.Global;
using NLog;
using System;
using System.Reflection;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EternityRadioApp
{
    public partial class App : Application
    {
        public static double ScreenWidth
        {
            get
            {
                return Device.Info.ScaledScreenSize.Width;
            }
        }

        private readonly NLog.ILogger Logger = NLog.LogManager.GetCurrentClassLogger();

        public App()
        {
            InitializeComponent();
            

           // MainPage = new NavigationPage(new MainPage());

            DependencyService.Register<AppModel>();

            MainPage = new AppShell();

            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;



        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Logger.Trace("CurrentDomain_UnhandledException()");
            Logger.Error(((Exception)e.ExceptionObject).Message);
            Logger.Error(((Exception)e.ExceptionObject).Message);
         
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        { 
            
        }


        public static Color LookupColor(string key)
        {
            try
            {
                Application.Current.Resources.TryGetValue(key, out var newColor);
                return (Color)newColor;
            }
            catch
            {
                return Color.White;
            }
        }

        public static string AppTheme
        {
            get; set;
        }
    }
}
