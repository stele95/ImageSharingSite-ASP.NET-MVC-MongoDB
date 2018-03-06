using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Image_Sharing_Site.Models
{
    public class ImageForShow
    {
        
        public string Slika { get; set; }
        public string Name { get; set; }
        public ImageForShow() { }        
    }
}