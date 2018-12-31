using System.Collections.Generic;
using System.Linq;
using SortMImage.Models;

namespace SortMImage.Services.DatabaseServices
{
    public class UserDatabaseService
    {
        #region Get

        public IList<User> GetUsers()
        {
            using (var db = new SortMImageContext())
            {
                return db.Users.ToList();
            }
        }

        public User GetUserByUserName(string username)
        {
            using (var db = new SortMImageContext())
            {
                return db.Users.FirstOrDefault(usr => usr.Username == username);
            }
        }

        #endregion

        #region Save

        public void SaveUser(User user)
        {
            using (var db = new SortMImageContext())
            {
                db.Users.Add(user);
                db.SaveChanges();
            }
        }

        #endregion

        #region Delete

        public void DeleteUser(User user)
        {
            using (var db = new SortMImageContext())
            {
                db.Users.Remove(user);
                db.SaveChanges();
            }
        }

        #endregion

        #region Verify

        public bool IsLoggedIn(string username, string passowrd)
        {
            foreach (User user in GetUsers())
            {
                if (user.Name == username && user.Password == passowrd)
                {
                    return true;
                }
            }

            return false;
        }

        #endregion

    }
}
