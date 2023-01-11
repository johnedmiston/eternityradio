using EternityRadioApp.Model;
using EternityRadioApp.ViewModel;
using MediaManager;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EternityRadioApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MessagePage : ContentPage
    {
        private readonly NLog.ILogger Logger = NLog.LogManager.GetCurrentClassLogger();


        public MessageViewModel Context
        {
            get
            {
                return BindingContext as MessageViewModel;
            }
        }

        public MessagePage()
        {
            InitializeComponent();
        }

        

        public void LoadMore()
        {

            var additional_messages = Context.Series.Messages.OrderBy(x => x.Sequence).Skip(Context.ShownItems).Take(MessageViewModel.LAZY_LOAD_THRESHOLD).ToList();
            Context.ShownItems += additional_messages.Count;

       
         
            foreach (var message in additional_messages)
            {
                try
                {


                    Frame frame = new Frame().LoadFromXaml("<Frame Margin=\"10,10\" Visual=\"Material\"></Frame>");
                    Logger.Debug("Loaded Frame xaml");
                    StackLayout layout = new StackLayout();

                    var name = new Label().LoadFromXaml("<Label HorizontalTextAlignment=\"Start\" TextColor=\"#222\"  FontSize=\"25\" />");
                    Logger.Debug("Loaded Name Label xaml");
                    name.SetBinding(Label.TextProperty, "Name");
                    name.BindingContext = message;
                    layout.Children.Add(name);

                    var summary = new Label().LoadFromXaml("<Label FontSize=\"18\" LineHeight=\"1.2\" />");
                    Logger.Debug("Loaded Submit Label xaml");
                    summary.SetBinding(Label.TextProperty, "Summary");
                    summary.BindingContext = message;
                    layout.Children.Add(summary);

                    var author = new Label().LoadFromXaml("<Label Margin=\"10,10\"  FontAttributes=\"Bold\" FontSize=\"16\" />");
                    Logger.Debug("Loaded Author Label xaml");
                    author.SetBinding(Label.TextProperty, "Author");
                    author.BindingContext = message;
                    layout.Children.Add(author);

                    var download = new Button().LoadFromXaml("<Button class=\"btn\"  Visual=\"Default\" FontSize=\"20\" Text=\"Download\" BackgroundColor=\"#f1f1f1\" TextColor=\"#1d1d1d\"></Button>");
                    Logger.Debug("Loaded Download Button xaml");
                    FontImageSource fontImage = new FontImageSource();
                    fontImage.FontFamily = "FontAwesome";
                    fontImage.Glyph = IconFont.Download;
                    fontImage.Color = Color.FromHex("#1d1d1d");
                    fontImage.Size = 20;

                    download.ImageSource = fontImage;

                    download.SetBinding(Button.CommandProperty, "DownloadCommand");
                    download.SetBinding(Button.IsVisibleProperty, "NotDownloaded");
                    download.BindingContext = message;
                    layout.Children.Add(download);

                    var delete = new Button().LoadFromXaml("<Button class=\"btn\"  Visual=\"Default\" FontSize=\"20\" Text=\"Delete\" BackgroundColor=\"#f1f1f1\" TextColor=\"#1d1d1d\"></Button>");
                    Logger.Debug("Loaded Delete Button xaml");
                    var fontImageDelete = new FontImageSource();
                    fontImageDelete.FontFamily = "FontAwesome";
                    fontImageDelete.Glyph = IconFont.Trash;
                    fontImageDelete.Color = Color.FromHex("#1d1d1d");
                    fontImageDelete.Size = 20;

                    delete.ImageSource = fontImageDelete;
                    delete.SetBinding(Button.CommandProperty, "DeleteCommand");


                    delete.SetBinding(Button.IsVisibleProperty, "CanDelete");
                    delete.BindingContext = message;
                    layout.Children.Add(delete);


                    var stacklayout2 = new StackLayout().LoadFromXaml("<StackLayout Orientation=\"Horizontal\" HorizontalOptions=\"Center\" />");
                    Logger.Debug("Loaded Stack Layout xaml");
                    stacklayout2.SetBinding(StackLayout.IsVisibleProperty, "IsDownloading");
                    stacklayout2.BindingContext = message;

                    var downloadimg = new Image().LoadFromXaml("<Image Source=\"loader\" IsAnimationPlaying=\"True\" WidthRequest=\"36\" HorizontalOptions=\"Start\" />");
                    Logger.Debug("Loaded Download Image xaml");
                    downloadimg.SetBinding(Image.IsVisibleProperty, "IsDownloading");
                    downloadimg.BindingContext = message;

                    stacklayout2.Children.Add(downloadimg);
                    var downloading_label = new Label().LoadFromXaml("<Label VerticalOptions=\"Center\" Text=\"Downloading...\"></Label>");
                    Logger.Debug("Loaded Download Label xaml");
                    downloading_label.SetBinding(Label.IsVisibleProperty, "IsDownloading");
                    downloading_label.BindingContext = message;
                    stacklayout2.Children.Add(downloading_label);
                    layout.Children.Add(stacklayout2);

                    var listen = new Button().LoadFromXaml("<Button class=\"btn\" Text=\"Listen\" FontAttributes=\"Bold\" FontSize=\"20\" TextColor=\"#FF9900\" Visual=\"Default\" />");
                    Logger.Debug("Loaded Listen Button xaml");
                    listen.SetBinding(Button.CommandProperty, "ListenCommand");
                    listen.BindingContext = message;
                    layout.Children.Add(listen);
                    frame.Content = layout;
                    MediaStack.Children.Add(frame);
                }
                catch(Exception ex)
                {
                    Logger.Error("Error during dynamic content load!");
                    Logger.Error(ex.Message);
                    Logger.Error(ex.StackTrace);
                }
            }
   
         

        }

        protected override void OnAppearing()
        {
            Logger.Trace("MessagePage.OnAppearing()");
            try
            {
                base.OnAppearing();
                CrossMediaManager.Current.Stop();
                Repository.Current.UnHookMedia();
                Title = Context.Name;
                Context.SelectedMedia = null;
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message);
                Logger.Error(ex.StackTrace);
            }
            
            
            //(BindingContext as MessageViewModel).SkeletonCommand.Execute(null);
        }

        private void Button_Clicked(object sender, EventArgs e)
        {
            LoadMore();
        }
    }
}