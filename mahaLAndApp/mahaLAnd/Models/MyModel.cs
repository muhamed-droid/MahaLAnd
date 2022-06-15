using System;
using System.Collections.Generic;

namespace mahaLAnd.Models
{
    public class MyModel
    {
        public User LoggedUser { get; set; }
        public User User { get; set; }
        public Profile LoggedProfile { get; set; }
        public Profile Profile { get; set; }
        public List<Post> Posts { get; set; }
        public List<User> Followers { get; set; }
        public List<User> Following { get; set; }
        public Post Post { get; set; }
        public Statistics Statistics { get; set; }
        public string SearchedUser { get; set; }
        public List<Tuple<User, Profile>> PotentionalUsers { get; set; }
        public List<Tuple<User, Profile>> Likes { get; set; }
        public List<Tuple<User, Profile, string>> Comments { get; set; }
        public List<Tuple<Notification, User, Profile>> Notifications { get; set; }
        public List<Tuple<User, Profile>> Follow { get; set; }
        public List<Tuple<User, Profile, Post, int>> Feed { get; set; }
        public int NumberOfLikes { get; set; } = 0;

        public MyModel()
        {
            LoggedUser = new User();
            User = new User();
            LoggedProfile = new Profile();
            Profile = new Profile();
            Posts = new List<Post>();
            Post = new Post();
            Followers = new List<User>();
            Following = new List<User>();
            Statistics = new Statistics();
            PotentionalUsers = new List<Tuple<User, Profile>>();
            Likes = new List<Tuple<User, Profile>>();
            Comments = new List<Tuple<User, Profile, string>>();
            Notifications = new List<Tuple<Notification, User, Profile>>();
            Follow = new List<Tuple<User, Profile>>();
            Feed = new List<Tuple<User, Profile, Post, int>>();
        }
    }
}
