using System;
using System.IO;
using System.Reflection;
using Android;
using Android.Content.PM;
using Android.OS;
using AndroidX.Core.App;
using EternityRadioApp.Services;
using NLog;
using NLog.Config;
using Xamarin.Forms;
using static AndroidX.RecyclerView.Widget.RecyclerView;

namespace EternityRadioApp.Droid
{
    public class LogService : ILogService
    {
        public void Initialize(Assembly assembly, string assemblyName, string path, object activity)
        {
            
            string resourcePrefix;

            try
            {
     
                /*
                ActivityCompat.RequestPermissions((Android.App.Activity)activity, new String[] { Manifest.Permission.WriteExternalStorage }, 1234);*/
                if (Device.RuntimePlatform == Device.iOS)
                    resourcePrefix = "EternityRadioApp.iOS";
                else if (Device.RuntimePlatform == Device.Android)
                    resourcePrefix = "EternityRadioApp.Droid";
                else
                    throw new Exception("Could not initialize Logger: Unknown Platform");

                //var location = $"{assemblyName}.NLog.config";

                string location = $"{resourcePrefix}.NLog.config";
                Stream stream = assembly.GetManifestResourceStream(location);
                if (stream == null)
                    throw new Exception($"The resource '{location}' was not loaded properly.");

                LogManager.Configuration = new XmlLoggingConfiguration(System.Xml.XmlReader.Create(stream), null);
                LogManager.Configuration.Variables["logdir"] = path;


            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}