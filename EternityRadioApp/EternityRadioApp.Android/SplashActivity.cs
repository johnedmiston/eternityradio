using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Util;
using AndroidX.Core.App;
using AndroidX.Core.Content;
using EternityRadioApp.Droid;

namespace EternityRadioApp.Droid
{
    [Activity(Theme = "@style/MyTheme.Splash", MainLauncher = true, NoHistory = true)]
    public class SplashActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        static readonly string TAG = "X:" + typeof(SplashActivity).Name;
        static bool LOADED = false;
        public override void OnCreate(Bundle savedInstanceState, PersistableBundle persistentState)
        {
              base.OnCreate(savedInstanceState, persistentState);  
            
             Log.Debug(TAG, "SplashActivity.OnCreate");





        }

        // Launches the startup task
        protected override void OnResume()
        {
            base.OnResume();

            if (!LOADED)
            {
                Task startupWork = new Task(() =>
                {

                    SimulateStartup();
                    LOADED = true;
                });
                startupWork.Start();
            }
            else
            {

                StartActivity(new Intent(Application.Context, typeof(MainActivity)));
            }
            
           
        }

       

        // Prevent the back button from canceling the startup process
        public override void OnBackPressed() 
        {
        
        }

        // Simulates background work that happens behind the splash screen
        async void SimulateStartup()
        {
            Log.Debug(TAG, "Loading repository data");
           // ActivityCompat.RequestPermissions((Android.App.Activity)this, new String[] { Manifest.Permission.WriteExternalStorage }, 1234);



            if (ContextCompat.CheckSelfPermission(this, Manifest.Permission.WriteExternalStorage) == (int)Permission.Granted)
            {
                // We have permission, go ahead and use the camera.
            }
            else
            {
                ActivityCompat.RequestPermissions(this, new String[] { Manifest.Permission.WriteExternalStorage }, 1234);

                // Camera permission is not granted. If necessary display rationale & request.
            }

            Repository.DownloadPath = GetExternalFilesDir(Android.OS.Environment.DirectoryMusic).AbsolutePath;
           // Repository.DownloadPath = Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryDownloads).AbsolutePath;
            await Repository.CreateRepository();
            Log.Debug(TAG, "repository data load finished. Starting MainActivity.");
            StartActivity(new Intent(Application.Context, typeof(MainActivity)));
           
        }
    }
}