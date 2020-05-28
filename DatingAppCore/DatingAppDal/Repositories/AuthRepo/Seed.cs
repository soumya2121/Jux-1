using DatingAppDal.Context;
using DatingAppDal.Model;
using System.IO;
using Newtonsoft.Json;
using System.Collections.Generic;
using DatingAppDal.BusinessLogic;

namespace DatingAppDal.Repositories.AuthRepo
{
    public class Seed
    {
        private DatingAppDbContext _context ;
        public Seed(DatingAppDbContext Context)
        {
            _context=Context;
        }
        public void seed()
        {
           _context.RemoveRange(_context.Users);
           _context.SaveChanges();
           var UsersFromFile= File.ReadAllText("Data/UserSeed.json");
           var Users= JsonConvert.DeserializeObject<List<User>>(UsersFromFile);
           foreach(var User in Users)
           {
              byte[] passwordhash;
              byte[] passswordsalt;
              AuthLogic.CreatePasswordHash("password",out passwordhash,out passswordsalt);
              User.PasswordHash=passwordhash;
              User.PasswordSalt=passswordsalt;
              User.Username= User.Username.ToLower();
              _context.Users.Add(User);
           }
           _context.SaveChanges();
        }
    }
}