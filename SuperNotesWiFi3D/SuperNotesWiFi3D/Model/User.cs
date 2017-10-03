using System.Collections.Generic;
using Firebase.Auth;
using SQLite;


namespace SuperNotesWiFi3D.Model
{
    public class UserData
    {
        public string email;
        public string name;
    }

    [Table("Users")]
    public class DBUser
    {
        [PrimaryKey, AutoIncrement, Column("_id")]
        public int ID { get; set; }
        public string UUID { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
    }


    public class User
    {
        public readonly string UUID; // from server
        public readonly int ID; // from local database

        public readonly string email;
        public readonly string password;
        public readonly string facebookToken;
        public FirebaseAuthLink authData;

        public UserData userData;

        public User(string UUID, int ID, string email, string password, string facebookToken, UserData userData) 
        {
            this.UUID = UUID;
            this.ID = ID;
            this.email = email;
            this.password = password;
            this.facebookToken = facebookToken;
            this.userData = userData;
        }
    }
}
