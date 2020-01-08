using System;

namespace DattingApp.API.Model
{
    public class Photo
    {
        public string Url { get; set; }
        public int Id { get; set; }
        public string Description { get; set; }
        public DateTime DateAdded { get; set; }
        public bool IsMain { get; set; }
        // So EF makes a cascade delete
        public User User { get; set; }
        public int UserId { get; set; }

        public string PublicId {get;set;}
    }
}