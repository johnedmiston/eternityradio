using EternityRadioApp.Model;
using EternityRadioApp.ViewModel;
using NLog;
using NLog.LayoutRenderers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Xml;
using System.Xml.Serialization;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration;

namespace EternityRadioApp
{
    public class Repository : INotifyPropertyChanged
    {

        private const string RESOURCE_MEDIA_CONTENT = "eternitycontent.xml";
        private const string RESOURCE_HISTORY_CONTENT = "history.xml";
        private const string RESOURCE_SETTINGS = "settings.xml";
        private readonly NLog.ILogger Logger = NLog.LogManager.GetCurrentClassLogger();


        public bool IsOffline { get; set; }
        public static Repository Current;

        public static object thisLock = new object();

        public BaseViewModel LastModel;

        public ContentPage LastPage;

        public static string AppPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);

        public string Db = Path.Combine(AppPath, "db.xml");
        public string SettingsFile = Path.Combine(AppPath, "settings.xml");
        public string VersionFile = Path.Combine(AppPath, "version.xml");
        public string History = Path.Combine(AppPath, RESOURCE_HISTORY_CONTENT);
        public static string DownloadPath = string.Empty;
        public List<History> Histories = new List<History>();
        public List<Setting> Settings = new List<Setting>();

        public string XmlVersion { get; set; } = "";

        public string DataDate { get; set; } = "";
        public async static Task<Repository> CreateRepository()
        {
            
            await Task.Run(() =>
            {
                Current = new Repository();

                Current.LoadDefaultData();

                Current.FetchData();

                Current.LoadModel();

                Current.Sync();

                Current.LoadHistory();
            });

            return Current;
        }

        private Stream LoadResource(string resource)
        {
            return Assembly.GetManifestResourceStream(Assembly.GetName().Name + "." + resource);
        }

   
        private void LoadDefaultData()
        {
            Logger.Debug("Repository.LoadDefaultData()");


           
            AllContent = new XmlDocument();
            RecentContent = new XmlDocument();

            try
            {

                Settings.Add(new Setting("WarnDataUsage", true));


                if (File.Exists(SettingsFile))
                {
                    var serializer = new XmlSerializer(Settings.GetType());
                    using (var reader = new StreamReader(SettingsFile))
                        Settings = (List<Setting>)serializer.Deserialize(reader);
                }
                else
                {
                    SaveSettings();
                }



                using (var stream = LoadResource(RESOURCE_MEDIA_CONTENT))
                    AllContent.Load(stream);

           
            } 
            catch (XmlException ex)
            {
                Logger.Error("Failed to log default xml data files!");
                Logger.Error(ex.Message);
                Logger.Debug(ex.StackTrace);
            }

            Logger.Debug("Default XML content files loaded.");
        }

        private void FetchData()
        {
            Logger.Debug("Respository.FetchData()");
            try
            {

                var webPage = new HttpClient().GetStringAsync(new Uri("https://eternityradio.org/eternitycontent.xml"));
                var xml = webPage.Result;

                AllContent.LoadXml(xml);
                Logger.Debug("Updated content database retrieved from server.");
            }
            catch (Exception ex)
            {
             
                IsOffline = true;
                Logger.Warn("Could not retrieve updated content database from server.");
                Logger.Warn(ex.Message);
                Logger.Warn(ex.StackTrace);
                Logger.Warn("Application is using local content database");
            }
        }

       
        public Repository()
        {
           Assembly = IntrospectionExtensions.GetTypeInfo(typeof(Repository)).Assembly;
        }

        

        private Assembly Assembly;

        internal void Save()
        {
           Logger.Debug("Repository.Save()");
           try
            {
                lock (thisLock)
                {
                    //List<Category> tempCat = new List<Category>();
                    var serializer = new XmlSerializer(typeof(List<Category>));
                    using (var writer = new StreamWriter(Db, false))
                        serializer.Serialize(writer, Categories);

                   
                        
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Failed to save respository data!");
                Logger.Error(ex.Message);
                Logger.Error(ex.StackTrace);
            }
        }

        public List<Series> Series { get; set; } = new List<Series>();

        public List<Category> Categories { get; set; } = new List<Category>();

        public List<Media> Streams { get; set; } = new List<Media>();

        public Media RecentMedia { get; set; }

        public bool Navigated { get; set;  } 

        public void SaveSettings()
        {
            
            var serializer = new XmlSerializer(Settings.GetType());
            using (var writer = new StreamWriter(SettingsFile, false))
                serializer.Serialize(writer, Settings);

        }

        public void SaveXmlVersion()
        {
            List<string> datas = new List<string>();
            datas.Add(Repository.Current.XmlVersion);
            datas.Add(Repository.Current.DataDate);

            var serializer = new XmlSerializer(typeof(List<string>));
            using (var writer = new StreamWriter(VersionFile, false))
                serializer.Serialize(writer, datas);

        }

        public void Setting(string name, object value)
        {
            var setting = Settings.FirstOrDefault(x => x.Name.ToUpper() == name.ToUpper());

            if (setting != null)
            {
                setting.Value = value;
                SaveSettings();
            }
        }

        public XmlDocument AllContent;

        private XmlDocument RecentContent;

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string name = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        private void Sync()
        {
            Logger.Trace("Repository.Sync");
            List<Category> local_categories = new List<Category>();
         
            if (File.Exists(Db))
            {
                try
                {
                    var serializer = new XmlSerializer(typeof(List<Category>));
                    using (var reader = new StreamReader(Db))
                        local_categories = (List<Category>)serializer.Deserialize(reader);

                }
                catch (Exception ex)
                {
                    Debug.WriteLine("Failed to load local database!");
                    Debug.WriteLine(ex.Message);
                    File.Delete(Db);
                }
            }

            

            if (!IsOffline)
            {

                // First look for changes online
                foreach (Category online_cat in Categories)
                {
                    var local_cat = local_categories.FirstOrDefault(x => x.Sequence == online_cat.Sequence);

                    if (local_cat == null)
                        local_categories.Add(online_cat);
                    else
                    {
                        local_cat.Image = online_cat.Image;
                        local_cat.Author = online_cat.Author;
                        local_cat.Summary = online_cat.Summary;
                        local_cat.Sequence = online_cat.Sequence;

                        foreach (var online_series in online_cat.Series)
                        {
                            var local_ser = local_cat.Series.FirstOrDefault(x => x.Sequence == online_series.Sequence);

                            if (local_ser == null)
                                local_cat.Series.Add(online_series);
                            else
                            {
                                local_ser.Image = online_series.Image;
                                local_ser.Author = online_series.Author;
                                local_ser.Summary = online_series.Summary;
                                local_ser.Sequence = online_series.Sequence;

                                foreach (var online_msg in online_series.Messages)
                                {
                                    var local_msg = local_ser.Messages.FirstOrDefault(x => x.Sequence == online_msg.Sequence);


                                    if (local_msg == null)
                                        local_ser.Messages.Add(online_msg);
                                    else
                                    {
                                        local_msg.Image = online_msg.Image;
                                        local_msg.Author = online_msg.Author;
                                        local_msg.Summary = online_msg.Summary;
                                        local_msg.Url = online_msg.Url;
                                        local_msg.LocalFile = online_msg.LocalFile;
                                    }
                                }

                                foreach (var online_doc in online_series.Documents)
                                {
                                    var local_doc = local_ser.Documents.FirstOrDefault(x => x.Sequence == online_doc.Sequence);

                                    if (local_doc == null)
                                        local_ser.Documents.Add(online_doc);
                                    else
                                    {
                                        local_doc.Url = online_doc.Url;
                                        local_doc.FilePath = online_doc.FilePath;
                                    }

                                }

                            }

                        }
                    }

                }

                // Second remove any local objects that were removed from online
                for (int c = 0; c < local_categories.Count(); c++)
                {
                    var online_cat = Categories.FirstOrDefault(x => x.Name.ToUpper() == local_categories[c].Name.ToUpper());

                    if (online_cat == null)
                        local_categories.RemoveAt(c);
                    else
                    {
                        for (int s = 0; s < local_categories[c].Series.Count; s++)
                        {
                            var online_ser = online_cat.Series.FirstOrDefault(x => x.Name.ToUpper() == local_categories[c].Series[s].Name.ToUpper());

                            if (online_ser == null)
                                local_categories[c].Series.RemoveAt(s);
                            else
                            {
                                for (int m = 0; m < local_categories[c].Series[s].Messages.Count; m++)
                                {
                                    var online_msg = online_ser.Messages.FirstOrDefault(x => x.Name.ToUpper() == local_categories[c].Series[s].Messages[m].Name.ToUpper());
                                    if (online_msg == null)
                                        local_categories[c].Series[s].Messages.RemoveAt(m);

                                }

                                for (int d = 0; d < local_categories[c].Series[s].Documents.Count; d++)
                                {
                                    var online_doc = online_ser.Documents.FirstOrDefault(x => x.Name.ToUpper() == local_categories[c].Series[s].Documents[d].Name.ToUpper());
                                    if (online_doc == null)
                                        local_categories[c].Series[s].Documents.RemoveAt(d);
                                }
                            }

                        }
                    }
                }


             




                Categories = local_categories.OrderBy(x => x.Sequence).ToList();
                SaveXmlVersion();
            } 
            else
            {
                try
                {
                    if (local_categories != null)
                        if (local_categories.Count > 0)
                            Categories = local_categories.OrderBy(x=> x.Sequence).ToList();



                    if (File.Exists(VersionFile))
                    {
                        var serializer = new XmlSerializer(typeof(List<string>));
                        using (var reader = new StreamReader(VersionFile))
                        {
                            var datas = (List<string>)serializer.Deserialize(reader);
                            XmlVersion = datas[0];
                            DataDate = datas[1];
                        }

                    }
                  

                }
                catch(Exception ex)
                {
                    Repository.Current.XmlVersion = "Offline Error";
                    Repository.Current.DataDate = "";
                }
            }


       
            // Now check the respository to validate downloaded files exist
            // and repair inconsistencies
            foreach (Category cat in Categories)
                foreach (Series ser in cat.Series)
                {
                    ser.Category = cat.Name;
                    foreach (Media med in ser.Messages)
                    {
                        try
                        {

                            med.Category = cat.Name;
                            med.Series = ser.Name;
                            if (string.IsNullOrWhiteSpace(med.LocalFile))
                            {
                                med.DownloadStatus = DownloadStatus.NONE;
                            }
                            else
                            {

                                try
                                {
                                    //var db = Repository.Current.Db;

                                    Uri uri = new Uri(med.Url);
                                    med.LocalFile = Path.Combine(Repository.DownloadPath, Path.GetFileName(uri.LocalPath));

                                }
                                catch (Exception ex)
                                {
                                    Debug.WriteLine($"Invalid url specified for media {med.Name}");
                                    Debug.WriteLine(ex.Message);
                                    med.LocalFile = string.Empty;
                                }

                                if (med.DownloadStatus == DownloadStatus.ERROR
                                    || med.DownloadStatus == DownloadStatus.DOWNLOADING)
                                    med.DownloadStatus = DownloadStatus.NONE;



                                if (!File.Exists(med.LocalFile))
                                    med.DownloadStatus = DownloadStatus.NONE;
                                else
                                    med.DownloadStatus = DownloadStatus.DOWNLOADED;
                            }
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine("Issue with media during synchronization.");
                            Debug.WriteLine(ex.Message);
                            med.DownloadStatus = DownloadStatus.NONE;
                        }
                    }

                    // Kludge to fix missing document series and catalog tags
                    foreach (PdfDocument doc in ser.Documents)
                    {
                        doc.Series = ser.Name;
                        doc.Category = cat.Name;
                    }

                }
            
            
            
            
            Save();


        }
        private void LoadHistory()
        {
            Logger.Trace("Repository.LoadHistory()");
            if (File.Exists(History))
            {
                try
                {
                    var serializer = new XmlSerializer(typeof(List<History>));
                    using (var reader = new StreamReader(History))
                        Histories = (List<History>)serializer.Deserialize(reader);

                }
                catch (Exception ex)
                {
                    Debug.WriteLine("Failed to load local history content!");
                    Debug.WriteLine(ex.Message);
                    File.Delete(History);
                }
            }
            
        }

        private void LoadModel()
        {
            Logger.Trace("Repository.LoadModel()");

            try
            {

                XmlNode update = AllContent.SelectSingleNode("//update");

                if (update != null) 
                {
                    Current.XmlVersion = update.Attributes["version"].Value.ToString();
                    Current.DataDate = update.Attributes["date"].Value.ToString();           
                }
                else
                {
                    Current.XmlVersion = "0";
                    Current.DataDate = "";
                }

                foreach (XmlNode node in AllContent.SelectNodes("//content//download//category"))
                {

                    Category category = new Category(node);
                    if (!Categories.Contains(category))
                        Categories.Add(category);
                }
                Logger.Debug("Downloadable content model loaded.");

                foreach (XmlNode node in AllContent.SelectNodes("//content//streams//media"))
                {
                    Media media = new Media(node, "Streams", string.Empty);
                    if (!Streams.Contains(media))
                        Streams.Add(media);

                }

                Categories = Categories.OrderBy(x => x.Sequence).ToList();
                Logger.Debug("Streamable content model loaded.");
            }
            catch(Exception ex)
            {
                Logger.Error("Fatal error loading content model.");
                Logger.Error(ex.Message);
                Logger.Error(ex.StackTrace);
                Process.GetCurrentProcess().Kill();

            }
        }

        internal void UnHookMedia()
        {
            Logger.Trace("Repository.UnHookMedia()");
            try
            {
           
                if (DownloadPlayerViewModel != null)
                    DownloadPlayerViewModel.UnHookEvents();

                if (StreamPlayerViewModel != null)
                    StreamPlayerViewModel.UnHookEvents();

            } catch (Exception ex)
            {
                Logger.Error(ex.Message);
                Logger.Error(ex.StackTrace);

            }

        }

        internal List<Media> GetMedia(string category, string series)
        {
           Logger.Trace("Repository.GetMedia()");
           var cat =  Categories.FirstOrDefault(x => x.Name.ToUpper() == category.ToUpper());

            if (cat != null)
            {
                var ser = cat.Series.FirstOrDefault(x => x.Name.ToUpper() == series.ToUpper());
                if (ser != null)
                    return ser.Messages.OrderBy(x=>x.Sequence).ToList();
            }
            Logger.Debug("Could not found requested media");
            return new List<Media>();

           
        }

        internal void AddHistory(Media media)
        {
            Logger.Trace("Repository.AddHistory()");
            try
            {
                History history = new History(media);
                Histories.Remove(history);
                Histories.Insert(0, history);

                if (Histories.Count > 10)
                    Histories.RemoveAt(10);

                var serializer = new XmlSerializer(typeof(List<History>));
                using (var writer = new StreamWriter(History, false))
                    serializer.Serialize(writer, Histories);

                Logger.Debug($"Added history for media: {media.Name}");

            }
            catch (Exception ex)
            {
                Logger.Error($"Failed to save history for media: {media.Name}");
                Logger.Error(ex.Message);
                Logger.Error(ex.StackTrace);
            }
        }

        internal PdfJsPage PdfJsPage { get; set; }

        internal T Setting<T>(string name)
        {
            return (T)Settings.FirstOrDefault(x => x.Name.ToUpper() == name.ToUpper()).Value;
        }

        internal Media GetMediaFromHistory(string category, string series, string name)
        {
            Logger.Trace("Repository.GetMediaFromHistory()");
            Logger.Debug($"GetMediaFromHistory Parameters: [category]{category},[series]{series},[name]{name}");
            var cat = Categories.FirstOrDefault(x => x.Name.ToUpper() == category.ToUpper());

            if (cat != null)
            {
                var ser = cat.Series.FirstOrDefault(x => x.Name.ToUpper() == series.ToUpper());
                if (ser != null)
                    return ser.Messages.FirstOrDefault(x => x.Name.ToUpper() == name.ToUpper());

            }
            Logger.Debug("Could not found requested media");
            return null;
        }

        public bool Locked { get; set; }
        public DownloadPlayerViewModel DownloadPlayerViewModel { get; set; }

        public StreamPlayerViewModel StreamPlayerViewModel { get; set; }
      
    }
}
