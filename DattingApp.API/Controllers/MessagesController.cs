using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using DattingApp.API.Data;
using DattingApp.API.DTO;
using DattingApp.API.Helpers;
using DattingApp.API.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DattingApp.API.Controllers
{
    [ServiceFilter(typeof(LogUserActivity))]
    [Authorize]
    [Route("api/users/{userId}/[controller]")]
    [ApiController]
    public class MessagesController : ControllerBase
    {
        IDatingRepository _repo;
        IMapper _mapper;
        public MessagesController(IDatingRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        [HttpGet("{msgId}", Name = "GetMessage")]
        public async Task<IActionResult> GetMessage(int userId, int msgId)
        {
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();

            var msgFromRepo = await _repo.GetMessage(msgId);
            if (msgFromRepo == null)
                return NotFound();
            return Ok(msgFromRepo);
        }
        [HttpPost]
        public async Task<IActionResult> CreateMessage(int senderId, MessageForCreationDTO msgDTO)
        {
            if (senderId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();

            msgDTO.SenderId = senderId;

            User recipient = await _repo.GetUser(msgDTO.ReceipientId);
            if(recipient == null)
                return BadRequest("Could not find user");

            Message m = _mapper.Map<Message>(msgDTO);

            _repo.Add(m);

            if(await _repo.SaveAll())
            {                
                var msgReturn = _mapper.Map<MessageForCreationDTO>(m);
                return CreatedAtRoute("GetMessage", new { senderId, id = m.Id}, msgReturn);
                //return CreatedAtRoute("GetMessage", new { senderId, id = m.Id}, m);
            }

            throw new System.Exception("Error while creating message");
        }
    }
}