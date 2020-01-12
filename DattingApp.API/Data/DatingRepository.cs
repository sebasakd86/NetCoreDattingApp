using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DattingApp.API.Helpers;
using DattingApp.API.Model;
using Microsoft.EntityFrameworkCore;

namespace DattingApp.API.Data
{
    public class DatingRepository : IDatingRepository
    {
        private readonly DataContext _context;
        public DatingRepository(DataContext context)
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

        public async Task<Photo> GetMainPhotoForUser(int userId)
        {
            return await _context.Photos.Where(u => u.UserId == userId).FirstOrDefaultAsync(p => p.IsMain);
        }

        public async Task<Photo> GetPhoto(int photoId)
        {
            var p = await _context.Photos.FirstOrDefaultAsync(p => p.Id == photoId);
            return p;
        }

        public async Task<User> GetUser(int id)
        {
            var u = await _context.Users.Include(p => p.Photos).FirstOrDefaultAsync(u => u.ID == id);
            return u;
        }
        public async Task<PagedList<User>> GetUsers(UserParams usrParams)
        {
            //do i need to put every user and photo in memory? cant filter here?
            var users = _context.Users
                .Include(p => p.Photos)
                .OrderByDescending(u => u.LastActive)
                .AsQueryable();

            users = users.Where(u => u.ID != usrParams.UserId);

            users = users.Where(u => u.Gender == usrParams.Gender);

            if (usrParams.MinAge != 18 || usrParams.MaxAge != 99)
            {
                var minDob = DateTime.Now.AddYears(usrParams.MaxAge * -1 - 1);
                var maxDob = DateTime.Now.AddYears(usrParams.MinAge * -1 - 1);
                users = users.Where(u => u.DateOfBirth >= minDob && u.DateOfBirth <= maxDob);
            }
            if (!string.IsNullOrEmpty(usrParams.OrderBy))
            {
                if (usrParams.OrderBy == "created")
                {
                    users = users.OrderByDescending(u => u.Created);
                }
            }
            return await PagedList<User>.CreateAsync(users, usrParams.PageNumber, usrParams.PageSize);
            //return await  _context.Users.Include(p => p.Photos).ToListAsync();
        }
        public async Task<bool> SaveAll()
        {
            return await _context.SaveChangesAsync() > 0;
        }
    }
}