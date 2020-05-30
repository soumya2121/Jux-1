using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DatingAppDal.Context;
using DatingAppDal.Helpers;
using DatingAppDal.Model;
using DatingAppWebApi.Helpers;
using Microsoft.EntityFrameworkCore;

namespace DatingAppDal.Repositories.DatingRepo
{
  public class DatingRepository : IDatingRepository
  {
    private readonly DatingAppDbContext _context;
    public DatingRepository(DatingAppDbContext context)
    {
      _context = context;
    }
    public void Add<T>(T entity) where T : class
    {
      _context.Add(entity);
    }

    public void Delete<T>(T entity) where T : class
    {
      _context.Remove(entity);
    }

    public async Task<User> GetUser(int Id)
    {
      var user = await _context.Users.Include(p => p.Photos).FirstOrDefaultAsync(x => x.Id == Id);
      return user;
    }

    public async Task<PagedLists<User>> GetUsers(UserParams userParams)
    {

      var users = _context.Users.Include(p => p.Photos).OrderByDescending(x => x.LastActive).Where(x => x.Gender == userParams.Gender);
      users = users.Where(x => x.Id != userParams.UserId);

      if (userParams.Likers)
      {
        var userLikers = await GetUserLikes(userParams.UserId, userParams.Likers);
        users = users.Where(u => userLikers.Contains(u.Id));
      }
      if (userParams.Likees)
      {
        var userLikers = await GetUserLikes(userParams.UserId, userParams.Likers);
        users = users.Where(u => userLikers.Contains(u.Id));
      }
      if (userParams.MinAge != 18 || userParams.MaxAge != 99)
      {
        users = users.Where(x => x.DateOfBirth.CalculateAge() >= userParams.MinAge && x.DateOfBirth.CalculateAge() <= userParams.MaxAge);
      }
      if (!string.IsNullOrEmpty(userParams.OrderBy))
      {
        switch (userParams.OrderBy)
        {
          case "created":
            users = users.OrderByDescending(u => u.Created);
            break;
          default:
            users = users.OrderByDescending(u => u.LastActive);
            break;
        }
      }
      return await PagedLists<User>.CreateAsync(users, userParams.PageNumber, userParams.pageSize);
    }

    public async Task<bool> SaveAll()
    {
      return await _context.SaveChangesAsync() > 0;
    }
    public Task<Photo> GetPhoto(int id)
    {
      var photo = _context.Photos.FirstOrDefaultAsync(p => p.Id == id);

      return photo;
    }

    public Task<Photo> GetUserMainPhoto(int id)
    {
      var photo = _context.Photos.Where(x => x.UserId == id).FirstOrDefaultAsync(p => p.IsMain == true);
      return photo;
    }

    public async Task<IEnumerable<int>> GetUserLikes(int id, bool liker)
    {
      var user = await _context.Users.Include(x => x.Liker).Include(x => x.Likee).FirstOrDefaultAsync(x => x.Id == id);
      if (liker)
      {
        return user.Liker.Where(x => x.LikeeId == id).Select(y => y.LikerId);
      }
      else
      {
        return user.Likee.Where(x => x.LikerId == id).Select(y => y.LikeeId);
      }
    }

    public Task<Like> GetLike(int userId, int recipientId)
    {
      var user = _context.Likes.FirstOrDefaultAsync(x => x.LikerId == userId && x.LikeeId == recipientId);
      return user;
    }
  }
}
