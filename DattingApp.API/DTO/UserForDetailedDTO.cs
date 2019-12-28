using System.Collections.Generic;
using DattingApp.API.Model;

namespace DattingApp.API.DTO
{
    public class UserForDetailedDTO
    {                
        public int ID { get; set; }
        public string UserName { get; set; }
        public string Gender { get; set; }
        public int Age { get; set; }
        public string KnownAs { get; set; }
        public string Interests { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string PhotoUrl { get; set; }
        public ICollection<PhotosForDetailDTO> Photos {get;set;}
        public string Introduction { get; set; }
        public string LookingFor { get; set; }
    }
}