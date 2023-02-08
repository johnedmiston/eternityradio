using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.OS;
using MediaManager;
using Xamarin.Forms;
using EternityRadioApp.Droid.Services;
using System.Threading.Tasks;
using static Android.Provider.SyncStateContract;
using System.Reflection;
using Xamarin.Forms.Platform.Android;
using System.IO;
using AndroidX.Core.App;
using Android;
using AndroidX.Core.Content;
using Android.Content.Res;
using System.Diagnostics;
using Android.Gms.Common.Logging;

namespace EternityRadioApp.Droid
{
    [Activity(Label = "EternityRadioApp", Icon = "@mipmap/ic_launcher", Theme = "@style/MainTheme", MainLauncher = false, ScreenOrientation = ScreenOrientation.Portrait, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {


        public static string ErrorPath = "";
        public override Resources Resources
        {
            get
            {
                Resources resource = base.Resources;
                Configuration configuration = new Configuration();
                configuration.SetToDefaults();
                if (Build.VERSION.SdkInt >= BuildVersionCodes.NMr1)
                {
                    return CreateConfigurationContext(configuration).Resources;
                }
                else
                {

                    resource.UpdateConfiguration(configuration, resource.DisplayMetrics);
                    return resource;
                }
            }
        }


        protected override void OnCreate(Bundle savedInstanceState)
        {
        
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            this.SetStatusBarColor(Color.FromHex("#fd7b38").ToAndroid());

            base.OnCreate(savedInstanceState);



            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            InitializeNLog();
            CrossMediaManager.Current.Init();


            CrossMediaManager.Current.StateChanged += (sender, e) =>
            {
                if (e.State == MediaManager.Player.MediaPlayerState.Stopped)
                {
                    var notificationManager = GetSystemService(NotificationService) as NotificationManager;
                    notificationManager.CancelAll();

                }
            };


            DependencyService.Register<IMediaManagerService, MediaManagerService_Android>();
            LoadApplication(new App());


        }

       


        private void InitializeNLog()
        {


            Assembly assembly = GetType().Assembly;
            string assemblyName = assembly.GetName().Name;

            var path = GetExternalFilesDir(Android.OS.Environment.DirectoryDocuments).AbsolutePath;

            bool isReadonly = Android.OS.Environment.MediaMountedReadOnly.Equals(Android.OS.Environment.ExternalStorageState);
            bool isWriteable = Android.OS.Environment.MediaMounted.Equals(Android.OS.Environment.ExternalStorageState);


            /*
            if (ContextCompat.CheckSelfPermission(this, Manifest.Permission.WriteExternalStorage) == (int)Permission.Granted)
            {
                // We have permission, go ahead and use the camera.
            }
            else
            {
                ActivityCompat.RequestPermissions(this, new String[] { Manifest.Permission.WriteExternalStorage }, 1234);

            }*/

            new LogService().Initialize(assembly, assemblyName, path, this);
        }


        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }



    }
}