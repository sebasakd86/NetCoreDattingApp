using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using DattingApp.API.Data;
using DattingApp.API.DTO;
using DattingApp.API.Helpers;
using DattingApp.API.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace DattingApp.API.Controllers
{
    [Authorize]
    [Route("api/users/{userId}/photos")]
    public class PhotosController : ControllerBase
    {
        private readonly IDatingRepository _repo;
        private readonly IMapper _mapper;
        private readonly IOptions<CloudinarySettings> _cloudCfg;
        private Cloudinary _cloudinary;

        public PhotosController(IDatingRepository repo, IMapper mapper, IOptions<CloudinarySettings> cloudCfg)
        {
            _repo = repo;
            _mapper = mapper;
            _cloudCfg = cloudCfg;

            Account acc = new Account(
                _cloudCfg.Value.CloudName,
                _cloudCfg.Value.ApiKey,
                _cloudCfg.Value.ApiSecret);

            _cloudinary = new Cloudinary(acc);
        }

        [HttpPost]
        public async Task<IActionResult> AddPhotoForUser(
            int userId,
            [FromForm]PhotoForCreationDTO photoFroCreation)
        {
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();

            var usrFromRepo = await _repo.GetUser(userId);
            var file = photoFroCreation.File;
            var uploadResult = new ImageUploadResult();

            if (file.Length > 0)
            {
                using (var stream = file.OpenReadStream())
                {
                    var uploadParams = new ImageUploadParams()
                    {
                        File = new FileDescription(file.Name, stream),
                        Transformation = new Transformation()
                            .Width(500)
                            .Height(500)
                            .Crop("fill")
                            .Gravity("face")
                    };
                    uploadResult = _cloudinary.Upload(uploadParams);
                }
            }
            photoFroCreation.Url = uploadResult.Uri.ToString();
            photoFroCreation.PublicId = uploadResult.PublicId;

            var photo = _mapper.Map<Photo>(photoFroCreation);
            
            photo.IsMain = false;

            if (!usrFromRepo.Photos.Any(u => u.IsMain))
                photo.IsMain = true;

            usrFromRepo.Photos.Add(photo);

            if (await _repo.SaveAll())
            {
                //return Ok();
                PhotoForReturnDTO pReturn = _mapper.Map<PhotoForReturnDTO>(photo);
                return CreatedAtRoute("GetPhoto", new { userId = userId, id = photo.Id }, pReturn);
            }
            return BadRequest("Could not add the photo");
        }

        [HttpGet("photoId", Name = "GetPhoto")]
        public async Task<IActionResult> GetPhoto(int photoId)
        {
            var photoFromRepo = await _repo.GetPhoto(photoId);

            var photo = _mapper.Map<PhotoForReturnDTO>(photoFromRepo);

            return Ok(photo);
        }

        [HttpPost("{photoId}/setMain")]
        public async Task<IActionResult> SetMainPhoto(int userId, int photoId)
        {
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();

            var usrFromRepo = await _repo.GetUser(userId);

            if (!usrFromRepo.Photos.Any(p => p.Id == photoId))
                return Unauthorized();

            var photoFromRepo = await _repo.GetPhoto(photoId);

            if (photoFromRepo.IsMain)
                return BadRequest("This is already the main photo");
            // Why not?
            //var currentMainPhoto = await _repo.GetPhoto(usrFromRepo.Photos.Any(p => p.IsMain).Id);
            var currentMainPhoto = await _repo.GetMainPhotoForUser(userId);

            currentMainPhoto.IsMain = false;

            photoFromRepo.IsMain = true;

            if (await _repo.SaveAll())
            {
                return NoContent();
            }

            return BadRequest("Failed to set photo to main");
        }
        [HttpDelete("{photoId}")]
        public async Task<IActionResult> DeletePhoto(int userId, int photoId)
        {
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();

            var usrFromRepo = await _repo.GetUser(userId);

            if (!usrFromRepo.Photos.Any(p => p.Id == photoId))
                return Unauthorized();

            var photoFromRepo = await _repo.GetPhoto(photoId);

            if (photoFromRepo.IsMain)
                return BadRequest("Can't delete the main photo");

            if (photoFromRepo.PublicId != null)
            {

                var delParams = new DeletionParams(photoFromRepo.PublicId);

                var result = _cloudinary.Destroy(delParams);

                if (result.Result == "ok")
                {
                    _repo.Delete(photoFromRepo);
                }
            }
            else
                _repo.Delete(photoFromRepo);
                
            if (await _repo.SaveAll())
            {
                return Ok();
            }

            return BadRequest("Failed to delete the photo");
        }
    }
}