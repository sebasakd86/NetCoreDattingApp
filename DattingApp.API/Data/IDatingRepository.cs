using System.Collections.Generic;
using System.Threading.Tasks;
using DattingApp.API.Model;

namespace DattingApp.API.Data
{
    public interface IDatingRepository
    {
         void Add<T>(T entity) where T: class;
         void Delete<T>(T entity) where T: class;
         Task<bool> SaveAll(); // 0 or more changes
         Task<IEnumerable<User>> GetUsers();
         Task<User> GetUser(int id);
    }
}