using MongoDB.Bson;
using MongoDB.Bson.Serialization.IdGenerators;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Image_Sharing_Site.Context;

namespace Image_Sharing_Site.Controllers
{
    public class UserController : Controller
    {
        // GET: User
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Login(string username, string password)
        {
            using (var dbContext = new ImgShareDBContext())
            {
                dbContext.ConnectionString = "mongodb://localhost:27017/";
                dbContext.DatabaseName = "imgshare";
                dbContext.IsSSLEnabled = true;


                dbContext.Connect();

                Models.User u = dbContext.GetUserByUsername(username, password);

                if (u != null)
                {
                    HttpCookie cookie = new HttpCookie("upload_services");
                    cookie["username"] = u.Username;
                    cookie["firstname"] = u.FirstName;
                    cookie["lastname"] = u.LastName;
                    cookie["id"] = u._id.ToString();
                    cookie.Expires = DateTime.Now.AddDays(1d);
                    Response.Cookies.Add(cookie);

                    dbContext.Dispose();                    
                    return Json(new { isok = true, message = "Successfully logged in!"}); 
                }

                dbContext.Dispose();
               
            }
            return Json(new { isok = false, message = "Incorrect Username/Password!" });
        }

        public void Register(string firstname, string lastname, string email, string password, string username)
        {
            Models.User user = new Models.User(firstname, lastname, email, username, password);

            using (var dbContext = new ImgShareDBContext())
            {
                dbContext.ConnectionString = "mongodb://localhost:27017";
                dbContext.DatabaseName = "imgshare";
                dbContext.IsSSLEnabled = true;

                dbContext.Connect();
                
                dbContext.InsertUser(user);

                dbContext.Dispose();
            }

            Login(username, password);

            Response.Redirect("/");
        }

        public new ActionResult Profile()
        {
            Models.User user = new Models.User();
            using (var context = new ImgShareDBContext())
            {
                context.ConnectionString = "mongodb://localhost:27017";
                context.IsSSLEnabled = true;
                context.DatabaseName = "imgshare";

                context.Connect();

                HttpCookie cookie = Request.Cookies.Get("upload_services");



                var id = ObjectId.Parse(cookie["id"]);
                user = context.GetUserById(id);

                context.Dispose();
            }

            if (user != null)
                return View(user);
            else
                return Content("Profil ne postoji");
        }

        public void DeleteAccount(string userid)
        {
            using (var dbContext = new ImgShareDBContext())
            {
                dbContext.ConnectionString = "mongodb://localhost:27017";
                dbContext.IsSSLEnabled = true;
                dbContext.DatabaseName = "imgshare";

                dbContext.Connect();
               
                dbContext.RemoveAccount(userid);

                dbContext.Dispose();
                                
            }
            Logout();

        }

        public void Logout()
        {
            Session.Abandon();
            Response.Cookies["upload_services"].Expires = DateTime.Now.AddDays(-1d);

            Response.Redirect("/");
        }
    }
}
