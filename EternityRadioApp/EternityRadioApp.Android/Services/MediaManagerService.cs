using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using EternityRadioApp.Droid.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Forms;
using EternityRadioApp.Droid;
using MediaManager;
using Android.Graphics;
using Xamarin.Forms.Platform.Android;
using MediaManager.Library;
using System.Threading.Tasks;
using Android.Support.V4.Media;
using Xamarin.Essentials;

[assembly: Dependency(typeof(MediaManagerService_Android))]
namespace EternityRadioApp.Droid.Services
{
    public class MediaManagerService_Android :IMediaManagerService
    {


       public void SetMediaImage(IMediaManager mediaManager, ImageSource source, MediaItem mediaItem)
       {

            var handler = new StreamImagesourceHandler();



            var task =  Task.Run<Bitmap>(async () => await handler.LoadImageAsync(source, Android.App.Application.Context));

            var bitmap = task.Result;

            if (DeviceInfo.Version.Major >= 11)
            {
                CrossMediaManager.Current.Queue.Current.DisplayTitle = mediaItem.Title;
                CrossMediaManager.Current.Queue.Current.DisplaySubtitle = mediaItem.Artist;
                CrossMediaManager.Current.Queue.Current.DisplayDescription = mediaItem.Artist;

                // Add extra key to bundle to map the artist field to the subtitle of the notification.
                Bundle bundle = new Bundle();
                bundle.PutString(MediaMetadataCompat.MetadataKeyArtist, mediaItem.Artist);

                CrossMediaManager.Current.Queue.Current.Extras = bundle;
            }


            CrossMediaManager.Current.Queue.Current.IsMetadataExtracted = true;
            //var webUrlImage = "https://eternityradio.org/images/eternity%20radio%201s.png?crc=310015723";
            // CrossMediaManager.Current.Queue.Current.DisplayImageUri = webUrlImage;

            
            CrossMediaManager.Current.Queue.Current.DisplayImage = bitmap;
            CrossMediaManager.Current.Queue.Current.AlbumImage = bitmap;
            CrossMediaManager.Current.Queue.Current.Image = bitmap;
            CrossMediaManager.Current.Notification.UpdateNotification();
            CrossMediaManager.Current.Extractor.UpdateMediaItem(CrossMediaManager.Current.Queue.Current);

            

        }
    }
}