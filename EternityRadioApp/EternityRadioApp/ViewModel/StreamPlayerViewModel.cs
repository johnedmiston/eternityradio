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

namespace EternityRadioApp.ViewModel
{


    public class StreamPlayerViewModel : BaseViewModel
    {
        private bool _ready;
        public bool Ready
        {
            get
            {
                return _ready;
            }
            set
            {
                _ready = value;
                OnPropertyChanged();
            }
        }

        private string _status = "LOADING...";
        public string Status
        {
            get
            {
                return _status;
            }
            set
            {
                _status = value;
                OnPropertyChanged();
            }
        }

        public StreamPlayerViewModel(Media media)
        {
            
            SelectedMedia = media;

            PlayMusic(media);

            Repository.Current.StreamPlayerViewModel = this;

            

        }


        private void Current_PositionChanged(object sender, MediaManager.Playback.PositionChangedEventArgs e)
        {
          Position = e.Position;
        }

        public void HookUpEvents()
        {
            UnHookEvents();
            CrossMediaManager.Current.StateChanged += Current_StateChanged;
            CrossMediaManager.Current.PositionChanged += Current_PositionChanged;
        }

        public void UnHookEvents()
        {
            CrossMediaManager.Current.StateChanged -= Current_StateChanged;
            CrossMediaManager.Current.PositionChanged -= Current_PositionChanged;
        }

        private bool IsCurrentPage
        {
            get
            {
                return Application.Current.MainPage is StreamPlayerPage || (Application.Current.MainPage is NavigationPage navPage && navPage.CurrentPage is StreamPlayerPage);

            }
        }
        





        #region Properties

        private Media selectedMedia;
        public Media SelectedMedia
        {
            get { return selectedMedia; }
            set
            {
                selectedMedia = value;
                OnPropertyChanged();
            }
        }

        private List<Media> mediaList;
        public List<Media> MediaList
        {
            get
            {
                return mediaList;
            }
            set
            {
                mediaList = value;
                OnPropertyChanged();
            }
        }


        #endregion







    
        private TimeSpan duration;
        public TimeSpan Duration
        {
            get { return duration; }
            set
            {
                duration = value;
                OnPropertyChanged();
            }
        }

        private TimeSpan position;
        public TimeSpan Position
        {
            get { return position; }
            set
            {
                position = value;
                OnPropertyChanged();
            }
        }

        public double TotalSeconds
        {
            get { return Position != null ? Position.TotalSeconds : 0.0d; }
            set
            {
                CrossMediaManager.Current.SeekTo(TimeSpan.FromSeconds(value));
                OnPropertyChanged();
            }
        }

        double maximum = 100f;
        public double Maximum
        {
            get { return maximum; }
            set
            {
                if (value > 0)
                {
                    maximum = value;
                    OnPropertyChanged();
                }
            }
        }

        private MediaPlayerState MediaPlayerState;

        private void Current_StateChanged(object sender, MediaManager.Playback.StateChangedEventArgs e)
        {
            Duration = CrossMediaManager.Current.Duration;
            Maximum = Duration.TotalSeconds;
            Position = CrossMediaManager.Current.Position;

            MediaPlayerState = e.State;

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

        public ICommand ShareCommand => new Command(ShareMedia);
        public void ShareMedia()
        {
            Share.RequestAsync(SelectedMedia.Url,SelectedMedia.Name);
        }

        private bool isPlaying;
        public bool IsPlaying
        {
            get { return isPlaying; }
            set
            {
                isPlaying = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(PlayIcon));
            }
        }

        internal async void PlayMusic(Media music)
        {

            var mediaInfo = CrossMediaManager.Current;


            if (!mediaInfo.IsBuffering())
            {
                var mediaItem = new MediaItem()
                {

                    MediaUri = music?.Url,
                    ImageUri = music?.Image,
                    Artist = music.Author,
                    Title = music.Name,
                    Image = music?.ImageSource,
                    DisplayTitle = music.Name,
                    DisplaySubtitle = music.Author,
                    DisplayDescription = music.Summary

                };
                // Fix for Android 12. See https://github.com/Baseflow/XamarinMediaManager/issues/876.
                mediaInfo.Speed = 1F;
                await mediaInfo.Play(mediaItem);
                DependencyService.Get<IMediaManagerService>().SetMediaImage(CrossMediaManager.Current, music.ImageSource, mediaItem);

                SelectedMedia = music;
                Repository.Current.RecentMedia = music;
            }
            else
                Debug.WriteLine($"Request is still Buffering: {mediaInfo.IsBuffering()}", "Application");

        }



        public string PlayIcon
        {
            get {
               
                return CrossMediaManager.Current.IsPlaying() ? "pause.png" : "play.png";  }
            set
            {
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Plays or pauses the active media
        /// </summary>
        public ICommand PlayCommand => new Command(Play);


        private void Play()
        {
            if (Ready)
            {
                // Fix for Android 12. See https://github.com/Baseflow/XamarinMediaManager/issues/876.
                CrossMediaManager.Current.Speed = 1F;
                CrossMediaManager.Current.PlayPause();
            }

        }
        /// <summary>
        /// Switches to the next media element
      



    }
}
