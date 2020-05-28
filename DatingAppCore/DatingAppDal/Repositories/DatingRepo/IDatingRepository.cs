using System.Collections.Generic;
using System.Threading.Tasks;
using DatingAppDal.Model;
using DatingAppWebApi.Helpers;

namespace DatingAppDal.Repositories.DatingRepo
{
    public interface IDatingRepository
    {
        void Add<T>(T entity) where T:class; 
        void Delete<T>(T entity) where T:class; 
        Task<bool> SaveAll();
        Task<PagedLists<User>> GetUsers(UserParams userParams);
        Task<User> GetUser(int Id);
        Task<Photo> GetPhoto(int id);
        Task<Photo> GetUserMainPhoto(int id);
        Task<Like> GetLike(int userId, int recipientId);
    }
}