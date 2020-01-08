using System;

namespace DattingApp.API.DTO
{
    public class PhotoForReturnDTO
    {
        public string Url { get; set; }
        public int Id { get; set; }
        public string Description { get; set; }
        public DateTime DateAdded { get; set; }
        public bool IsMain { get; set; }  
        public string PublicId { get; set; }
    }
}