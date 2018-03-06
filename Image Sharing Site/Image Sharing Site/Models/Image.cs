using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MongoDB.Driver;
using MongoDB.Bson;
using System.Drawing;
using System.IO;

namespace Image_Sharing_Site.Models
{
    public class Image
    {
        public ObjectId _id { get; set; }
        public string Name { get; set; }
        public bool Visibility { get; set; }        
        public byte[] ImgInBytes { get; set; }        

        public byte[] imageToByteArray(System.Drawing.Image imageIn)
        {
            MemoryStream ms = new MemoryStream();
            imageIn.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
            return ms.ToArray();
        }

        /*public System.Drawing.Image byteArrayToImage(byte[] byteArrayIn)
        {
            MemoryStream ms = new MemoryStream(byteArrayIn);
            System.Drawing.Image returnImage = System.Drawing.Image.FromStream(ms);
            return returnImage;
        }*/

        public System.Drawing.Image byteArrayToImage(byte[] byteArrayIn)
        {

            System.Drawing.ImageConverter converter = new System.Drawing.ImageConverter();
            System.Drawing.Image img = (System.Drawing.Image)converter.ConvertFrom(byteArrayIn);

            return img;
        }
    }


}