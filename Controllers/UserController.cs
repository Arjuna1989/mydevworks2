using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using API.Data;
using API.Dtos;
using API.Entity;
using API.Extension;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _context;
        private readonly IMapper _mapper;
        private readonly IPhotoService _photoService;
        public UserController(IUserRepository context, IMapper mapper, IPhotoService photoService)
        {
            _photoService = photoService;
            _mapper = mapper;
            _context = context;

        }

        [HttpGet("{id}")]
        public async Task<ActionResult<UserDto>> GetUser(int Id)
        {

            var user = await _context.GetUserByIdAsync(Id);
            return Ok(user);

        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetUsers()
        {

            var users = await _context.GetUsersAsync();
            return Ok(users);

        }

        [HttpPut]
        public async Task<ActionResult> UpdateUser(UserUpdateDto userUpdateDto)
        {

            var userName = User.GetName();
            var user = await _context.GetUserByNameAsync(userName);

            if (user == null) return NotFound();
            _mapper.Map(userUpdateDto, user);

            if (await _context.SaveAll()) return NoContent();

            return BadRequest("An Error occured during the Update");

        }

        [HttpPost("add-photo")]
        public async Task<ActionResult<PhotoDto>> AddPhotoAsync(IFormFile file)
        {
            var user = await _context.GetUserByNameAsync(User.GetName());

            if (user == null) return NotFound();

            var result = await _photoService.AddPhotoAsync(file);

            if (result.Error != null) return BadRequest("Error occured ! while uploading the file");

            var photo = new Photos
            {
                PublicId = result.PublicId,
                Url = result.SecureUrl.AbsoluteUri,

            };

            if (user.Photos.Count == 0) photo.IsMain = true;

            user.Photos.Add(photo);

            if (await _context.SaveAll()) return CreatedAtAction(nameof(GetUser), new { id = user.Id }, _mapper.Map<PhotoDto>(photo));

            return BadRequest("Error occured during the photo upload");



        }


        [HttpDelete("delete-photo/{photoId}")]
        public async Task<ActionResult<PhotoDto>> DeletePhotoAsync(int photoId)
        {
            var user = await _context.GetUserByNameAsync(User.GetName());
            var photo = user.Photos.FirstOrDefault(x => x.Id == photoId);
            if (photo == null) return NotFound("There aee no Photos to delete");

            if (photo.IsMain) return BadRequest("Can not delete the main photo");

            var photoDeletionResult = await _photoService.DeletePhotoAsync(photo.PublicId);

            if (photoDeletionResult.Error != null) return BadRequest(photoDeletionResult.Error.Message);

            user.Photos.Remove(photo);

            if (await _context.SaveAll()) return Ok();

            return BadRequest("Problem deleting photo !");

        }

        [HttpPost("set-main-photo/{photoId}")]
        public async Task<ActionResult<PhotoDto>> UpdatePhotoAsync(int photoId)
        {
            var user = await _context.GetUserByNameAsync(User.GetName());
            if (user == null) return NotFound("User not found !");
            var photo = user.Photos.FirstOrDefault(x => x.Id == photoId);
            if (photo == null) return NotFound("There are no any Photo to Update");

            if (photo.IsMain) return BadRequest("this is already your main photo");

            var currentPMainPhoto = user.Photos.FirstOrDefault(x => x.IsMain == true);
            if (currentPMainPhoto != null) currentPMainPhoto.IsMain = false;
            photo.IsMain = true;


            if (await _context.SaveAll()) return NoContent();

            return BadRequest("Problem setting the main photo !");

        }
    }
}