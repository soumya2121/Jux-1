using System.Threading.Tasks;
using DatingAppDal.Model;

namespace DatingAppDal.Repositories.AuthRepo
{
    public interface IAuthRepository
    {
         
         Task<User> RegisterUser(User user,string password);
         Task<User> LoginUser(string Username, string Password);
         Task<bool> UserExist(string Username);
    }
}