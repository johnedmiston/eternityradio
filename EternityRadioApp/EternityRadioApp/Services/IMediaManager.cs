using MediaManager;
using MediaManager.Library;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace EternityRadioApp

{
    public interface IMediaManagerService
    {
       

       void SetMediaImage(IMediaManager mediaManager, ImageSource source, MediaItem mediaItem);
    }
}
