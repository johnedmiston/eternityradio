using EternityRadioApp.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using Xamarin.Essentials;
using Xamarin.Forms;
using static Xamarin.Essentials.Permissions;

namespace EternityRadioApp.Model
{

    public class Series : IEquatable<Series>
    {
        [XmlIgnore]
        public string Items
        {
            get { return $"{Count} Message(s)"; }
        }

        [XmlIgnore]
        public int Count
        {
            get { return Messages.Count; }
        }

        public string Name { get; set; }

        public string Image { get; set; }

        public string Category { get; set; }

        public string Author { get; set; }

        public string Summary { get; set; }

        public int Sequence { get; set; }


        public List<PdfDocument> Documents { get; set; } = new List<PdfDocument>();
        public List<Media> Messages { get; set; } = new List<Media>();

        public Series()
        {

        }
        public Media AddMessage(XmlNode node)
        {
            Media media = new Media(node, Category, Name);
            if (!Messages.Contains(media))
            {
                Messages.Add(media);
                return media;
            }
            else
            {
                Debug.WriteLine($"Warning: Xml content issue. Duplicate media '{media.Name}' has been ignored. Please remove duplicate message from Xml database.");
                return null;
            }

        }

        public PdfDocument AddDocument(XmlNode node)
        {
            PdfDocument document = new PdfDocument(node, Category, Name);
            if (!Documents.Contains(document))
            {
                Documents.Add(document);
                return document;
            } else
            {
                Debug.WriteLine($"Warning: Xml content issue. Duplicate document '{document.Name}' has been ignored. Please remove duplicate document from Xml database.");
                return null;
            }
        }

        private void LoadMessages(XmlNode xmlSeries)
        {
            Debug.WriteLine($"Loading {Name} messages...");
            foreach (XmlNode node in xmlSeries.SelectNodes(".//media"))
                AddMessage(node);
        }

        private void LoadDocuments(XmlNode xmlSeries)
        {
            foreach (XmlNode node in xmlSeries.SelectNodes(".//document"))
                AddDocument(node);
        
                    
        }


        public Series(XmlNode node, string category)
        { 
             try
            {
                Category = category;
                Debug.WriteLine("Loading new series..."); 
                string name = node.Attributes["Name"].Value;

                
                Debug.WriteLine($"Processing Series {name.ToUpper()}");
          
                string image = node.Attributes["CoverImage"].Value;

                string author = node.Attributes["Author"].Value;
                string summary = node.Attributes["Summary"].Value;
                string sequence = node.Attributes["Sequence"].Value;

                if (string.IsNullOrWhiteSpace(summary))
                    summary = $"An insightful and encouraging audio series focusing on the truth of God's word";

                Name = name;
                Author = author;
                Summary = summary;
                Image = image;

                if (!int.TryParse(sequence, out int seq))
                    Debug.WriteLine($"Warning: Invalid sequence for series '{name}'");

                Sequence = seq;

                LoadMessages(node);
                LoadDocuments(node);

            }catch(Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }

        }

        [XmlIgnore]
        public ImageSource ImageSource
        {
            get
            {
                try
                {
                    if (Image.ToLower().StartsWith("http"))
                        return ImageSource.FromUri(new Uri(Image));
                    else if (Image.StartsWith("EternityRadioApp"))
                        return ImageSource.FromResource(Image, typeof(Media).GetTypeInfo().Assembly);
                    else
                        return null;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("Failed Parsing Series Image");
                    Debug.WriteLine(ex.Message);
                    return null;
                }
            }
        }

     
        public bool Equals(Series other)
        {
            return other.Name.ToUpper() == Name.ToUpper() 
                && other.Author.ToUpper() == Author.ToUpper()
                && other.Category.ToUpper() == Category.ToUpper();
        }
    }
}
