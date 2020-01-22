using System;
using System.Collections.Generic;
using System.Linq;
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
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IDatingRepository _repo;
        private readonly IMapper _mapper;
        public UsersController(IDatingRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetUsers([FromQuery] UserParams usrParams)
        {
            int currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            User usrFromRepo = await _repo.GetUser(currentUserId);

            usrParams.UserId = currentUserId;

            if(string.IsNullOrEmpty(usrParams.Gender))
            {
                usrParams.Gender = usrFromRepo.Gender == "male" ? "female" : "male";
            }

            PagedList<User> users = await _repo.GetUsers(usrParams);

            var retUsers = _mapper.Map<IEnumerable<UserForListDTO>>(users);

            Response.AddPagination(
                users.CurrentPage, 
                users.PageSize, 
                users.TotalCount, 
                users.TotalPages
            );

            return Ok(retUsers);
        }
        [HttpGet("{id}", Name="GetUser")]
        public async Task<IActionResult> GetUser(int id)
        {
            User u = await _repo.GetUser(id);
          
            int currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            
            var retUser = _mapper.Map<UserForDetailedDTO>(u);
            if(id != currentUserId)
                retUser.Photos = retUser.Photos.Where(p => p.Status != "Pending").ToList();

            return Ok(retUser);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, UserForUpdateDTO usr)
        {
            if(id != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();

            var usrFromRepo = await _repo.GetUser(id);
            
            _mapper.Map(usr, usrFromRepo);

            if(await _repo.SaveAll())
                return NoContent();
            throw new System.Exception($"Updating user {id} failed on save");
        }
        [HttpPost("{id}/like/{recipientId}")]
        public async Task<IActionResult> LikeUser(int id, int recipientId)
        {
            if(id != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();

            var like = await _repo.GetLike(id, recipientId);

            if(like != null)
                return BadRequest("You've already liked this user");
            
            if(await _repo.GetUser(recipientId) == null)
                return NotFound();
            
            like = new Like
            {
                LikerId = id,
                LikeeId = recipientId
            };

            _repo.Add<Like>(like);

            if(await _repo.SaveAll())
                return Ok();
                
            return BadRequest("Couldnt like user :(");
        }

    }
}