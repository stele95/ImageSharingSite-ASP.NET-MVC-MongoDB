using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Image_Sharing_Site.Models
{
    [BsonDiscriminator("users")]
    public class User
    {
        public ObjectId _id { get; set; }
        [BsonElement("firstname")]
        public string FirstName { get; set; }
        [BsonElement("lastname")]
        public string LastName { get; set; }
        [BsonElement("username")]
        public string Username { get; set; }
        [BsonElement("email")]
        public string Email { get; set; }
        [BsonElement("password")]
        public string Password { get; set; }
        [BsonElement("uploads")]
        public List<ImageForList> Uploads { get; set; }

        public User(string firstname, string lastname, string email, string username, string password)
        {
            _id = ObjectId.GenerateNewId();
            FirstName = firstname;
            LastName = lastname;
            Email = email;
            Username = username;
            Password = password;
            Uploads = new List<ImageForList>();
        }

        public User()
        {
        }

        internal void RemoveImage(ObjectId tmp)
        {
            foreach(ImageForList img in Uploads)
            {
                if (img._id == tmp)
                {
                    Uploads.Remove(img);
                    break;
                }
            }
        }
    }
}