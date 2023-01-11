using EternityRadioApp.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace EternityRadioApp.Model
{
    [Serializable]
    public class Category : IEquatable<Category>
    {

        [XmlIgnore]
        public int Count
        {
            get { return Series.Count; }
        }

        [XmlIgnore]
        public string Items
        {
            get { return $"{Count} Series"; }
        }

        public string Name { get; set; }
        public string Author { get; set; }
        public string Image { get; set; }
        public string Summary { get; set; }
        public int Sequence { get; set; }
     

        public List<Series> Series { get; set; } = new List<Series>();
        public Category()
        {

        }

        public Category(XmlNode node)
        {
  
            
            try
            {
                Debug.WriteLine("Loading new category...");
                string name = node.Attributes["Name"].Value;
                Debug.WriteLine($"Processing category: {name}");
          
                string image = node.Attributes["CoverImage"].Value;
                string author = node.Attributes["Author"].Value;
                string summary = node.Attributes["Summary"].Value;
                string sequence = node.Attributes["Sequence"].Value;





                if (!int.TryParse(sequence, out int seq))
                    Debug.WriteLine($"Warning: Invalid sequence for category '{name.ToUpper()}'");

                Name = name;
                Author = author;
                Summary = summary;
                Image = image;
                Sequence = seq;
                LoadCategorySeries(node);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
           
        }

        private Series AddSeries(XmlNode node)
        {
            Series series = new Series(node, Name);
            try
            {
                if (!Series.Contains(series))
                {
                    Series.Add(series);
                    return series;
                }
                else
                {
                    Debug.WriteLine($"Warning: Xml content issue. Duplicate series '{series.Name}' has been ignored. Please remove duplicate series from Xml database.");
                    return null;
                }
            }
            catch(Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return null;
            }
            
        }

        private void LoadCategorySeries(XmlNode xmlCategory)
        {
            foreach (XmlNode node in xmlCategory.SelectNodes(".//series"))
                AddSeries(node);
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
                    Debug.WriteLine("Failed Parsing Category Image");
                    Debug.WriteLine(ex.Message);
                    return null;
                }
            }
        }


        public bool Equals(Category other)
        {
            return Name.ToUpper() == other.Name.ToUpper();
        }
    }
}
