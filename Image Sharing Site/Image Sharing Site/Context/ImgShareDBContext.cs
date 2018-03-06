using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MongoDB.Driver;
using MongoDB.Bson;
using MongoDB.Driver.Core;
using Image_Sharing_Site.Models;
using System.Web.Script.Serialization;

namespace Image_Sharing_Site.Context
{
    public class ImgShareDBContext : IDisposable
    {
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
        public bool IsSSLEnabled { get; set; }
        private IMongoDatabase _database { get; set; }

        public ImgShareDBContext()
        {

        }

        public ImgShareDBContext(string ConnectionString)
        {
            this.ConnectionString = ConnectionString;
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        ~ImgShareDBContext()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(false);
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            GC.SuppressFinalize(this); // kaze Garbage Collector - u da je objekat rucno "pokupljen"
        }
        #endregion

        public void Connect()
        {
            try
            {
                MongoClientSettings settings = MongoClientSettings.FromUrl(new MongoUrl(ConnectionString));

                if (IsSSLEnabled)
                    settings.SslSettings = new SslSettings() { EnabledSslProtocols = System.Security.Authentication.SslProtocols.Tls12 };

                MongoClient client = new MongoClient(settings);
                _database = client.GetDatabase(DatabaseName);
            }

            catch (Exception ex)
            {
                throw new Exception("Cannot connect to the driver. ", ex);
            }
        }

        public string[] GetImages()
        {            
            var collection = _database.GetCollection<Image>("uploads");

            List<string> products = new List<string>();
            

            foreach (Image img in collection.Find(x => x.Visibility == true).ToList().Take(10).ToList())
            {
                ImageForShow slika = new ImageForShow() { Slika = Convert.ToBase64String(img.ImgInBytes), Name = img.Name };
                products.Add(slika.ToJson());               
                
            }

            return products.ToArray();
        }
        
       
        internal User GetUserByUsername(string username, string password)
        {
            var collection = _database.GetCollection<User>("users");
            var result = collection.Find(x => x.Username == username).ToList();

            if (result != null && result.Count != 0)
            {
                User u = result.First();

                if (u.Password == password)
                    return u;

                return null;
            }

            return null;
        }

        internal void UpdateUserUploads(Image uploadedImage, string Username)
        {
            var collection = _database.GetCollection<Image>("uploads");
            collection.InsertOne(uploadedImage);

            if (!Username.Equals(String.Empty))
            {
                var collectionUser = _database.GetCollection<User>("users");
                User user = collectionUser.Find(x => x.Username == Username).ToList().First();

                if (user.Uploads == null)
                {
                    user.Uploads = new List<ImageForList>();
                }

                user.Uploads.Add(new ImageForList(uploadedImage._id,uploadedImage.Name,uploadedImage.Visibility));
                var filter = Builders<User>.Filter.Eq("username", Username);
                var update = Builders<User>.Update.Set("uploads", user.Uploads);

                collectionUser.UpdateOne(filter, update);
            }
        }

        public void InsertUser(User u)
        {
            var collection = _database.GetCollection<User>("users");
            collection.InsertOne(u);
        }

        internal User GetUserById(ObjectId id)
        {
            var collection = _database.GetCollection<User>("users");
            try
            {
                return collection.Find(x => x._id == id).ToList().First();
            }
            catch (Exception e)
            {
                return null;
            }
        }

        internal void RemoveImage(string fileId, string userId)
        {
            var tmp = ObjectId.Parse(fileId);

            var builder = Builders<Image>.Filter;
            var filter = builder.Eq(x => x._id, tmp);
            var collection = _database.GetCollection<Image>("uploads");            
            collection.DeleteOneAsync(filter);           

            var collectionUser = _database.GetCollection<User>("users");
            User user = collectionUser.Find(x => x._id == ObjectId.Parse(userId)).ToList().First();

            
            user.RemoveImage(tmp);
            
            var filter1 = Builders<User>.Filter.Eq("_id", ObjectId.Parse(userId));
            var update = Builders<User>.Update.Set("uploads", user.Uploads);

            collectionUser.UpdateOne(filter1, update);
        }

        internal Image SearchUploadedImages(string searchID)
        {
            if (_database != null)
            {
                var collection = _database.GetCollection<Image>("uploads");
                var filter = Builders<Image>.Filter.Eq(x => x._id, ObjectId.Parse(searchID));
                var res = collection.Find(filter).ToList().First();

                return res;
            }
            else
            {
                return new Image();
            }
        }

        internal void RemoveAccount(string id)
        {
            if (_database != null)
            {
                var collection = _database.GetCollection<User>("users");
                var filter = Builders<User>.Filter.Eq(x => x._id, ObjectId.Parse(id));
                collection.FindOneAndDeleteAsync(filter);
            }

            else
            {
                return;
            }
        }
    }
}