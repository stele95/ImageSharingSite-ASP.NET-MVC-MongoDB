using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Web;
using System.Web.Mvc;
using MongoDB.Bson;
using Image_Sharing_Site.Context;

namespace Image_Sharing_Site.Controllers
{
    public class UploadController : Controller
    {
        // GET: Image
        public ActionResult Index()
        {
            return View();
        }

        public void UploadImage(HttpPostedFileBase file, string visability)
        {
            if (file == null)
            {
                Response.Redirect("/Upload");

            }
            else
            {
                var fileName = "";
                var uploadedFile = Request.Files[0];
                fileName = Path.GetFileName(uploadedFile.FileName);


                using (var DBContext = new ImgShareDBContext())
                {
                    DBContext.ConnectionString = "mongodb://localhost:27017";
                    DBContext.DatabaseName = "imgshare";
                    DBContext.IsSSLEnabled = true;

                    DBContext.Connect();

                    Models.Image forUpload = new Models.Image()
                    {
                        _id = ObjectId.GenerateNewId(),
                        Name = Request.Files[0].FileName
                        
                    };

                    if (Request.Cookies.Count > 0)
                        if (visability.Equals("private"))
                            forUpload.Visibility = false;
                        else
                            forUpload.Visibility = true;
                    else
                        forUpload.Visibility = true;

                    
                    using (var binaryReader = new BinaryReader(Request.Files[0].InputStream))
                    {
                        forUpload.ImgInBytes = binaryReader.ReadBytes(uploadedFile.ContentLength);
                    }

                    if (Request.Cookies.Count > 0)
                        DBContext.UpdateUserUploads(forUpload, Request.Cookies[0]["username"]);
                    else
                        DBContext.UpdateUserUploads(forUpload, String.Empty);

                    DBContext.Dispose(); //release resources used here
                }

                Response.Redirect("/");
            }
        }


        [AsyncTimeout(150)]
        [HandleError(ExceptionType = typeof(TimeoutException), View = "TimeoutError")]
        public void Remove(string _id, string userId)
        {
            using (var dbContext = new ImgShareDBContext())
            {
                dbContext.ConnectionString = "mongodb://localhost:27017";
                dbContext.IsSSLEnabled = true;
                dbContext.DatabaseName = "imgshare";

                dbContext.Connect();

                dbContext.RemoveImage(_id, userId);

                dbContext.Dispose(); //release the resources used here
            }

            Response.Redirect("/User/Profile");
        }

       
    }
}