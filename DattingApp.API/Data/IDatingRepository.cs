using System.Collections.Generic;
using System.Threading.Tasks;
using DattingApp.API.Helpers;
using DattingApp.API.Model;

namespace DattingApp.API.Data
{
    public interface IDatingRepository
    {
        void Add<T>(T entity) where T : class;
        void Delete<T>(T entity) where T : class;
        Task<bool> SaveAll(); // 0 or more changes
        Task<PagedList<User>> GetUsers(UserParams usrParams);
        Task<User> GetUser(int id);
        Task<Photo> GetPhoto(int photoId);
        Task<Photo> GetMainPhotoForUser(int userId);
        Task<Like> GetLike(int userId, int recipientId);
    }
}