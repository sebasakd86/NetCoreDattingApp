using System;

namespace DattingApp.API.DTO
{
    public class PhotosForDetailDTO
    {
        public string Url { get; set; }
        public int Id { get; set; }
        public string Description { get; set; }
        public DateTime DateAdded { get; set; }
        public bool IsMain { get; set; }        
    }
}