using System.Threading.Tasks;
using DatingAppDal.Context;
using DatingAppDal.Model;
using DatingAppDal.BusinessLogic;
using Microsoft.EntityFrameworkCore;

namespace DatingAppDal.Repositories.AuthRepo
{
    public class AuthRepository : IAuthRepository
    {
        private DatingAppDbContext _context;
        public AuthRepository(DatingAppDbContext context)
        {
            _context = context;
        }
        public async Task<User> LoginUser(string Username, string Password)
        {
           var User= await _context.Users.Include(p=>p.Photos).FirstOrDefaultAsync(x=>x.Username==Username);
           if(User==null)
           return null;
           if(!AuthLogic.VerifyPasswordHash(Password,User.PasswordHash,User.PasswordSalt))
           return null;
           return User;
        }

        public async Task<User> RegisterUser(User user, string password)
        {
            byte[] passwordhash,passwordsalt;

            AuthLogic.CreatePasswordHash(password, out passwordhash, out passwordsalt);
            user.PasswordHash =  passwordhash;
            user.PasswordSalt = passwordsalt;
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<bool> UserExist(string Username)
        {
            var user= await _context.Users.FirstOrDefaultAsync(x=>x.Username==Username);
            if(user!=null)
            return false;
            return true;
        }
    }
}
