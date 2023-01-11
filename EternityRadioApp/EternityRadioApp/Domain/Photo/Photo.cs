using System;
using System.Collections.Generic;
using System.Text;

namespace EternityRadioApp.Domain.Models
{
    public class Photo
    {
        public string ImageSrc { get;set;}
        public int Id { get;set; }

        public override string ToString()
        {
            return ImageSrc;
        }
    }
}
