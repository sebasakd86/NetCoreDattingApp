using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using DattingApp.API.Data;
using DattingApp.API.DTO;
using DattingApp.API.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DattingApp.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AdminController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly UserManager<User> _userManager;
        private readonly IMapper _mapper;
        private readonly PhotosController _photoController;
        public AdminController(DataContext context, UserManager<User> userManager, IMapper mapper, PhotosController photoController)
        {
            _photoController = photoController;
            _mapper = mapper;
            _userManager = userManager;
            _context = context;
        }

        [Authorize(Policy = "RequireAdminRole")]
        [HttpGet("usersWithRoles")]
        public async Task<IActionResult> GetUserWithRoles()
        {
            var usrList = await _context.Users
                .OrderBy(x => x.UserName)
                .Select(u => new
                {
                    Id = u.Id,
                    UserName = u.UserName,
                    Roles = ((from userROle in u.UserRoles
                              join role in _context.Roles
                              on userROle.RoleId equals role.Id
                              select role.Name)).ToList()
                }).ToListAsync();

            return Ok(usrList);
        }
        [Authorize(Policy = "ModeratePhotoRole")]
        [HttpGet("photosForModeration")]
        public async Task<IActionResult> GetPhotosForModeration()
        {
            IEnumerable<Photo> photos = await _context.Photos.Where(p => p.Status == "Pending").ToListAsync();
            IEnumerable<PhotoForReturnDTO> pReturn = _mapper.Map<IEnumerable<PhotoForReturnDTO>>(photos);
            return Ok(pReturn);
        }
        [Authorize(Policy = "ModeratePhotoRole")]
        [HttpPost("approvePhoto/{photoId}")]
        public async Task<IActionResult> ApprovePhoto(int photoId)
        {
            //Photo p = await _repo.GetPhoto(photoId);
            Photo p = await _context.Photos.FindAsync(photoId);
            if (p == null)
                return BadRequest();
            //Update the photo
            p.Status = "Approved";
            await _context.SaveChangesAsync();
            return NoContent();
        }
        [Authorize(Policy = "ModeratePhotoRole")]
        [HttpDelete("banPhoto/{photoId}")]
        public async Task<IActionResult> BanPhoto(int photoId)
        {
            //Photo p = await _repo.GetPhoto(photoId);
            Photo p = await _context.Photos.FindAsync(photoId);
            if (p == null)
                return BadRequest();
            //Update the photo            
            _context.Photos.Remove(p);
            await _context.SaveChangesAsync();
            return NoContent();            
        }
        [Authorize(Policy = "RequireAdminRole")]
        [HttpPost("editRoles/{userName}")]
        public async Task<IActionResult> EditRoles(string userName, RoleEditDTO roleEditDto)
        {
            var usr = await _userManager.FindByNameAsync(userName);
            var usrRoles = await _userManager.GetRolesAsync(usr);
            string[] selRoles = roleEditDto.RoleNames;
            //selRoles == selRoles ?? new string[]{}; //could be null? or just empty?
            var result = await _userManager.AddToRolesAsync(usr, selRoles.Except(usrRoles));
            if (!result.Succeeded)
                return BadRequest("Failer to add roles");

            result = await _userManager.RemoveFromRolesAsync(usr, usrRoles.Except(selRoles));
            if (!result.Succeeded)
                return BadRequest("Failer to remove roles");

            return Ok(await _userManager.GetRolesAsync(usr));
        }
    }
}