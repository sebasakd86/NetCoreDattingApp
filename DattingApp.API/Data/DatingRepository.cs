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
        public async Task<Like> GetLike(int userId, int recipientId)
        {
            return await _context.Likes.FirstOrDefaultAsync(
                u => u.LikerId == userId && u.LikeeId == recipientId);
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
            //Console.WriteLine($"Likers: {usrParams.Likers} | Likees: {usrParams.Likees}");
            if (usrParams.Likers)
            {
                var userLikers = await GetUserLikes(usrParams.UserId, true);
                users = users.Where(u => userLikers.Contains(u.ID));
            }
            if (usrParams.Likees)
            {
                var userLikees = await GetUserLikes(usrParams.UserId, false);
                users = users.Where(u => userLikees.Contains(u.ID));
            }
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
        private async Task<IEnumerable<int>> GetUserLikes(int userId, bool likers)
        {
            User u = await _context.Users
                        .Include(x => x.Likers)
                        .Include(x => x.Likees)
                        .FirstOrDefaultAsync(u => u.ID == userId);
            if (likers)
            {
                return u.Likers.Where(u => u.LikeeId == userId).Select(i => i.LikerId);
            }
            else
            {
                return u.Likees.Where(u => u.LikerId == userId).Select(i => i.LikeeId);
            }
        }

        public async Task<bool> SaveAll()
        {
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<Message> GetMessage(int msgId)
        {
            return await _context.Messages.FirstOrDefaultAsync(m => m.Id == msgId);
        }

        public async Task<PagedList<Message>> GetMessagesForUser(MessageParams msgParams)
        {
            var msgs = _context.Messages
                .Include(u => u.Sender)
                .ThenInclude(p => p.Photos)
                .Include(u => u.Recipient)
                .ThenInclude(p => p.Photos)
                .AsQueryable();

            if (msgParams.MessageContainer == "Inbox")
                msgs = msgs.Where(u => u.RecipientId == msgParams.UserId);
            else if (msgParams.MessageContainer == "Outbox")
                msgs = msgs.Where(u => u.SenderId == msgParams.UserId);
            else
                msgs = msgs.Where(u => u.RecipientId == msgParams.UserId && !u.IsRead);

            msgs = msgs.OrderByDescending(d => d.MessageSent);

            return await PagedList<Message>.CreateAsync
                (msgs, msgParams.PageNumber, msgParams.PageSize);
            //This should be an abstraction or composite class since we're using them in quite a few classes.
            //Same with the fn that check the user ID EVERYWHERE!
        }

        public Task<IEnumerable<Message>> GetMessageThread(int senderId, int receiverId)
        {
            throw new NotImplementedException();
        }
    }
}