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
using MediaManager.Library;
using NLog;

namespace EternityRadioApp.ViewModel
{

  

    public class DownloadPlayerViewModel : BaseViewModel
    {

        private readonly NLog.ILogger Logger = NLog.LogManager.GetCurrentClassLogger();


        public bool PreviousVisible
        {
            get {
                Logger.Trace("PreviousVisible Getter");
                return SelectedMedia.Sequence > 1; }
        }

        public bool NextVisible
        {
            get {
                Logger.Trace("NextVisible Getter");
                return SelectedMedia.Sequence < MediaList.Count; }
        }

        public string NextImage
        {
            get {
                Logger.Trace("NextImage Getter");
                return NextVisible ? "next.png" : "next_disabled.png";  }
        }

        public string PreviousImage
        {
            get {
                Logger.Trace("PreviousImage Getter");
                return PreviousVisible ? "previous.png" : "previous_disabled.png"; }
        }
        private bool _ready;
        public bool Ready
        {
            get
            {
                Logger.Trace("Ready Getter");
                return _ready;
            }
            set
            {
                Logger.Trace("Ready Setter");
                _ready = value;
                OnPropertyChanged();
            }
        }

        private string _status = "LOADING...";
        public string Status
        {
            get
            {
                Logger.Trace("Status Getter");
                return _status;
            }
            set
            {
                Logger.Trace("Status Setter");
                _status = value;
                OnPropertyChanged();
            }
        }

        public DownloadPlayerViewModel(Media media)
        {
            Logger.Trace("DownloadPlayerViewModel.DownloadPlayerViewModel()");
            try
            {
                MediaList = Repository.Current.GetMedia(media.Category, media.Series);
                SelectedMedia = media;

                Maximum = CrossMediaManager.Current.Duration.TotalSeconds;
                OnPropertyChanged("Maximum");
                CheckState(CrossMediaManager.Current.State);

                PlayMusic(media);

                Repository.Current.DownloadPlayerViewModel = this;
            } 
            catch (Exception ex)
            {
                Logger.Error("Error creating 'DownloadPlayerViewModel' instance");
                Logger.Error(ex.Message);
                Logger.Error(ex.StackTrace);
            }


        }


        private void Current_PositionChanged(object sender, MediaManager.Playback.PositionChangedEventArgs e)
        {
            Logger.Trace("DownloadPlayerViewModel.Current_PositionChanged()");
            Position = e.Position;

        }

        private void Current_MediaItemChanged(object sender, MediaManager.Media.MediaItemEventArgs e)
        {
            Logger.Trace("DownloadPlayerViewModel.MediaItemChanged()");
            Position = CrossMediaManager.Current.Position;
            Duration = e.MediaItem.Duration;
            Maximum = Duration.TotalSeconds;
            

            OnPropertyChanged("TotalSeconds");


        }






        #region Properties

        private Media selectedMedia;
        public Media SelectedMedia
        {
            get {
                Logger.Trace("SelectedMedia Getter");
                return selectedMedia; }
            set {
                Logger.Trace("SelectedMedia Setter");
                selectedMedia = value;
                OnPropertyChanged();
                OnPropertyChanged("PreviousVisible");
                OnPropertyChanged("NextVisible");
                OnPropertyChanged("NextImage");
                OnPropertyChanged("PreviousImage");
            }
        }

    private List<Media> mediaList;
        public List<Media> MediaList
        {
            get 
            {
                Logger.Trace("MediaList Getter");
                return mediaList;
            }
            set
            {
                Logger.Trace("MediaList Setter");
                mediaList = value;
                OnPropertyChanged();
            }
        }


        #endregion




        public ICommand ChangeCommand => new Command(ChangeMedia);
        
       

        /// <summary>
        /// Changes the current media element based on the button pressed
        /// </summary>
        /// <param name="obj">A character indicating which direction to move</param>
        private void ChangeMedia(object obj)
        {


            Logger.Trace("DownloadPlayerViewModel.ChangeMedia()");
            Logger.Debug($"ChangeMedia Parameter[obj]:{obj}");
            if ((string)obj == "P")
                PreviousMusic();
            else if ((string)obj == "N")
                NextMusic();

           

        }
        private TimeSpan duration;
        public TimeSpan Duration
        {
            get {
                Logger.Trace("Duration Getter");
                return duration; }
            set
            {
                Logger.Trace("Duration Setter");
                duration = value;
                OnPropertyChanged();
            }
        }

        private TimeSpan position;
        public TimeSpan Position
        {
            get {
                Logger.Trace("Position Getter");
                return position; }
            set
            {
                Logger.Trace("Position Setter");
                position = value;
                OnPropertyChanged();
              
            }
        }

        public double TotalSeconds
        {
            get {
                Logger.Trace("TotalSeconds Getter");
                return Position != null ? Position.TotalSeconds : 0.0d; }
            set
            {
                Logger.Trace("TotalSeconds Setter");
                CrossMediaManager.Current.SeekTo(TimeSpan.FromSeconds(value));
                OnPropertyChanged();

            }
        }

        public void HookUpEvents()
        {
            
            Logger.Trace("DownloadPlayerViewModel.HookUpEvents() Enter");
          
            CrossMediaManager.Current.MediaItemChanged += Current_MediaItemChanged;
            CrossMediaManager.Current.StateChanged += Current_StateChanged;
            CrossMediaManager.Current.PositionChanged += Current_PositionChanged;

            Logger.Trace("DownloadPlayerViewModel.HookUpEvents() Exit");

        }

        public void UnHookEvents()
        {
            Logger.Trace("DownloadPlayerViewModel.UnHookEvents");
            CrossMediaManager.Current.MediaItemChanged -= Current_MediaItemChanged;
            CrossMediaManager.Current.StateChanged -= Current_StateChanged;
            CrossMediaManager.Current.PositionChanged -= Current_PositionChanged;
        }


        double maximum = 100f;
        public double Maximum
        {
            get {
                Logger.Trace("Maximum Getter");
                return maximum; }
            set
            {

                Logger.Trace("Maximum Setter");
                if (value > 0)
                {

                    maximum = value;
                    OnPropertyChanged();
                }
            }
        }

        private MediaPlayerState MediaPlayerState;

        private void CheckState(MediaPlayerState state)
        {
            Logger.Trace("DownloadPlayerViewModel.CheckState()");
            MediaPlayerState = state;
            switch (MediaPlayerState)
            {
                case MediaPlayerState.Playing:
                    Status = "PLAYING NOW";
                    IsPlaying = true;
                    Ready = true;
                    break;
                case MediaPlayerState.Buffering:
                    Status = "BUFFERING...";
                    IsPlaying = false;
                    Ready = false;
                    break;
                case MediaPlayerState.Loading:
                    Status = "LOADING...";
                    IsPlaying = false;
                    Ready = false;
                    break;
                case MediaPlayerState.Paused:
                    Status = "PAUSED";
                    IsPlaying = false;
                    Ready = true;
                    break;
                case MediaPlayerState.Stopped:
                    Status = "STOPPED";
                    IsPlaying = false;
                    Ready = true;
                    break;
                case MediaPlayerState.Failed:
                    Status = "PLAYBACK FAILED";
                    IsPlaying = false;
                    Ready = true;
                    break;
            }
        }

        private void Current_StateChanged(object sender, MediaManager.Playback.StateChangedEventArgs e)
        {
            Logger.Trace("DownloadPlayerViewModel.Current_StateChanged()");
          
            Duration = CrossMediaManager.Current.Duration;
            Maximum = Duration.TotalSeconds;
            Position = CrossMediaManager.Current.Position;
            
            CheckState(e.State); 


          
        }


        private bool isPlaying;
        public bool IsPlaying
        {
            get {
                Logger.Trace("IsPlaying Getter");
                return isPlaying; }
            set
            {
                Logger.Trace("IsPlaying Setter");
                isPlaying = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(PlayIcon));
            }
        }


        public ICommand ShareCommand => new Command(ShareMedia);
        public void ShareMedia()
        {
            Logger.Trace("DownloadPlayerViewModel.ShareMedia()");
            Share.RequestAsync(SelectedMedia.Url, SelectedMedia.Name);
        }

        internal async void PlayMusic(Media music)
        {


            Logger.Trace("DownloadPlayerViewModel.PlayMusic()");
            var mediaInfo = CrossMediaManager.Current;


            if (!mediaInfo.IsBuffering())
            {

                var mediaItem = new MediaItem()
                {

                    MediaUri = music.Downloaded ? music.LocalFile : music.Url,
                    ImageUri = music?.Image,
                    Artist = music.Author,
                    Title = music.Name,
                    Image = music?.ImageSource,
                    DisplayTitle = music.Name,
                    DisplaySubtitle = music.Series,
                    DisplayDescription = music.Summary

                };
                // Fix for Android 12. See https://github.com/Baseflow/XamarinMediaManager/issues/876.
                mediaInfo.Speed = 1F;
                await mediaInfo.Play(mediaItem);
                //  CrossMediaManager.Current.Queue.Current.IsMetadataExtracted = false;
                //  var webUrlImage = "https://eternityradio.org/images/eternity%20radio%201s.png?crc=310015723";
                // CrossMediaManager.Current.Queue.Current.DisplayImageUri = webUrlImage;
                //CrossMediaManager.Current.Queue.Current.AlbumImageUri = webUrlImage;
                // CrossMediaManager.Current.Queue.Current.ImageUri = webUrlImage;
                // CrossMediaManager.Current.Notification.UpdateNotification();
                // await CrossMediaManager.Current.Extractor.UpdateMediaItem(CrossMediaManager.Current.Queue.Current);
                DependencyService.Get<IMediaManagerService>().SetMediaImage(CrossMediaManager.Current, music.ImageSource, mediaItem);

                SelectedMedia = music;
                Repository.Current.RecentMedia = music;
                Repository.Current.AddHistory(music);
            }
            else
                Logger.Debug($"Request is still Buffering: {mediaInfo.IsBuffering()}");

        }

     
      
        public string PlayIcon
        {
            get {
                Logger.Trace("PlayIcon Getter");
                return CrossMediaManager.Current.IsPlaying() ? "pause.png" : "play.png"; }
            set
            {
                Logger.Trace("PlayIcon Setter");
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Plays or pauses the active media
        /// </summary>
        public ICommand PlayCommand => new Command(Play);


        private void Play()
        {


            Logger.Trace("DownloadPlayerViewModel.Play()");
            // Fix for Android 12. See https://github.com/Baseflow/XamarinMediaManager/issues/876.
            CrossMediaManager.Current.Speed = 1F;
            CrossMediaManager.Current.PlayPause();
                
        }
        /// <summary>
        /// Switches to the next media element
        /// </summary>
        internal void NextMusic()
        {

            Logger.Trace("DownloadPlayerViewModel.NextMusic()");


            var currentIndex = MediaList.IndexOf(SelectedMedia);



            if (currentIndex < MediaList.Count - 1)
            {

                Logger.Debug($"Playing Index:{ currentIndex + 1}");

             
                PlayMusic(MediaList[currentIndex + 1]);

            } else
            {
                Repository.Current.Locked = false;
            }
      
        }

        /// <summary>
        /// Switches to the previous media element
        /// </summary>
        internal void PreviousMusic()
        {

            Logger.Trace("DownloadPlayerViewModel.PreviousMusic()");

            var currentIndex = MediaList.IndexOf(SelectedMedia);

            if (currentIndex > 0)
            {
                Logger.Debug($"Playing Index:{currentIndex + 1}");
                PlayMusic(MediaList[currentIndex -1]);

            }
            else
            {
                Repository.Current.Locked = false;
            }

        }


        
    }
}
