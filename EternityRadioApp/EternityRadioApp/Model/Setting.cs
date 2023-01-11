using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Schema;

namespace EternityRadioApp.Model
{
    public class Setting
    {
        public Setting()
        {

        }

        public Setting(string name, object value)
        {
            Name = name;
            Value = value; 
        }
        public string Name { get; set; }

        public object Value { get; set; }

      
    }
}
