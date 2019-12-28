using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using DattingApp.API.Data;
using DattingApp.API.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DattingApp.API.Controllers
{
    [Authorize]
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
        public async Task<IActionResult> GetUsers()
        {
            var users = await _repo.GetUsers();

            var retUsers = _mapper.Map<IEnumerable<UserForListDTO>>(users);

            return Ok(retUsers);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser(int id)
        {
            var u = await _repo.GetUser(id);

            var retUser = _mapper.Map<UserForDetailedDTO>(u);

            return Ok(retUser);
        }
    }
}