using Image_Sharing_Site.Context;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace Image_Sharing_Site.Controllers
{
    
    
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

       
        public string GetImages()
        {
            using (var dbContext = new ImgShareDBContext())
            {
                dbContext.ConnectionString = "mongodb://localhost:27017/";
                dbContext.DatabaseName = "imgshare";
                dbContext.IsSSLEnabled = true;

                dbContext.Connect();

                var products = dbContext.GetImages();                               

                dbContext.Dispose();

                JavaScriptSerializer objSerializer = new JavaScriptSerializer();
                return objSerializer.Serialize(products);
            }

        }
    }
}