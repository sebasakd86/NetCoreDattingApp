using System;

namespace DattingApp.API.DTO
{
    public class MessageForCreationDTO
    {
        public MessageForCreationDTO()
        {
            MessageSent = DateTime.Now;
        }

        public int SenderId { get; set; }
        public int ReceipientId { get; set; }
        public DateTime MessageSent { get; set; }
        public string Content { get; set; }
        
    }
}