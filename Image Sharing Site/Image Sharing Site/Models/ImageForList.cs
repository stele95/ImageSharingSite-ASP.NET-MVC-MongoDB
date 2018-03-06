using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Image_Sharing_Site.Models
{
    public class ImageForList
    {
        public ObjectId _id { get; set; }
        public string Name { get; set; }
        public bool Visibility { get; set; }

        public ImageForList() { }
        public ImageForList(ObjectId id, string name, bool v)
        {
            _id = id;
            Name = name;
            Visibility = v;
        }
    }
}