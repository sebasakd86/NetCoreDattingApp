using System;
using System.Collections.Generic;
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
    //[Authorize]
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

        [HttpGet]
        public async Task<IActionResult> GetMessagesForUser(
            int userId,
            [FromQuery] MessageParams msgParams)
        {
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();

            msgParams.UserId = userId;

            var msgsFromRepo = await _repo.GetMessagesForUser(msgParams);

            var msgs = _mapper.Map<IEnumerable<MessageToReturnDTO>>(msgsFromRepo);

            Response.AddPagination(
                msgsFromRepo.CurrentPage,
                msgsFromRepo.PageSize,
                msgsFromRepo.TotalCount,
                msgsFromRepo.TotalPages);

            return Ok(msgs);
        }
        [HttpGet("thread/{receiverId}")]
        public async Task<IActionResult> GetMessageThread(int userId, int receiverId)
        {
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();

            IEnumerable<Message> msgsFromRepo = await _repo.GetMessageThread(userId, receiverId);

            IEnumerable<MessageToReturnDTO> msgThread = _mapper.Map<IEnumerable<MessageToReturnDTO>>(msgsFromRepo);

            return Ok(msgThread);
        }

        [HttpPost]
        public async Task<IActionResult> CreateMessage(int userId, MessageForCreationDTO msgDTO)
        {
            //So autommaper maps automatically the objects inside MessageToReturnDTO
            User sender = await _repo.GetUser(userId);

            if (sender.Id != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();

            msgDTO.SenderId = userId;

            User recipient = await _repo.GetUser(msgDTO.RecipientId);

            if (recipient == null)
                return BadRequest("Could not find user");

            Message m = _mapper.Map<Message>(msgDTO);

            _repo.Add(m);

            if (await _repo.SaveAll())
            {
                var msgReturn = _mapper.Map<MessageToReturnDTO>(m);
                return CreatedAtRoute("GetMessage", new { userId, msgId = m.Id }, msgReturn);
                //return CreatedAtRoute("GetMessage", new { senderId, id = m.Id}, m);
            }

            throw new System.Exception("Error while creating message");
        }
        [HttpPost("{messageId}")]
        public async Task<IActionResult> DeleteMessage(int messageId, int userId)
        {
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();
            var msgFromRepo = await _repo.GetMessage(messageId);

            if (msgFromRepo.SenderId == userId)
                msgFromRepo.SenderDeleted = true;
            if (msgFromRepo.RecipientId == userId)
                msgFromRepo.RecipientDeleted = true;

            if (msgFromRepo.SenderDeleted && msgFromRepo.RecipientDeleted)
                _repo.Delete(msgFromRepo);

            if (await _repo.SaveAll())
                return NoContent();

            throw new Exception("Error deleting the message");
        }
        [HttpPost("{messageId}/read")]
        public async Task<IActionResult> MarkAsRead(int messageId, int userId)
        {
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();
            var msg = await _repo.GetMessage(messageId);
            if (msg.RecipientId != userId)
                return Unauthorized();
            msg.IsRead = true;
            msg.DateRead = DateTime.Now;

            if(await _repo.SaveAll())
                return NoContent();
            throw new Exception("Error while marking the message as read");
        }
    }
}