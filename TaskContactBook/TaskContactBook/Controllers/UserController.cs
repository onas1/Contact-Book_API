using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using TaskContactBook.Services;

namespace TaskContactBook.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public class UserController : ControllerBase
    {
        private readonly Cloudinary _Cloudinary;
        private readonly UserManager<AppUser> _userManager;

        public UserController(IConfiguration config, UserManager<AppUser> userManager)
        {
            Account account = new Account
            {
                Cloud = config.GetSection("CloudinarySettings:CloudName").Value,
                ApiKey = config.GetSection("CloudinarySettings:ApiKey").Value,
                ApiSecret = config.GetSection("CloudinarySettings:ApiSecret").Value,
            };
            _Cloudinary = new Cloudinary(account);
            _userManager = userManager;
        }

        [HttpPatch("Photo/{Id}")]
        [Authorize(Roles="Admin, Regular")]
        public async Task<IActionResult> AddPhoto(string Id, [FromForm] PhotoDto model)
        {
            //if login userid does not match with id passed, return unauthorized.
            var user = await _userManager.GetUserAsync(User);
            
            if (Id != user.Id)
                return Unauthorized();

            var file = model.PhotoFile;

            if (file.Length <= 0)
                return BadRequest("Invalid file size");
            var imageUploadResult = new ImageUploadResult();
            using (var fs = file.OpenReadStream())
            {
                var imageUploadParams = new ImageUploadParams()
                {
                    File = new FileDescription(file.FileName, fs),
                    Transformation = new Transformation()
                    .Height(300).Width(300)
                    .Gravity("face").Crop("fill")
                };

                imageUploadResult =_Cloudinary.Upload(imageUploadParams);
            }
            var publicId = imageUploadResult.PublicId;
            var ImgUrl = imageUploadResult.Url.ToString();
            user.Image = ImgUrl;
            _userManager.UpdateAsync(user).Wait();

            return Ok(new { publicId, ImgUrl });
        }

        [HttpGet]
        [Route("get-all")]
        [Authorize(Roles = "Admin, Regular")]
        public IActionResult GetAll([FromQuery] UserParameter userParameter)
        {
            List<AppUserDto> usersDto = new List<AppUserDto>();
            var users = _userManager.Users.ToList();
            foreach (var item in users)
            {
                usersDto.Add(new AppUserDto
                {
                    FirstName = item.FirstName,
                    LastName = item.LastName,
                    ImgUrl = item.Image,
                    PhoneNumber = item.PhoneNumber,
                    Email = item.Email,
                    Street = item.Street,
                    City = item.City,
                    State = item.State,
                    Country = item.Country,
                    FacebookLink = item.FacebookLink,
                    LinkedinLink = item.LinkedinLink,
                    TwitterLink = item.TwitterLink
                });
            }
            var ReturnUsers = usersDto.OrderBy(o => o.FirstName)
                .Skip(userParameter.PageNumber - 1)
                .Take(userParameter.PageSize)
                .ToList();
            return Ok(ReturnUsers);
        }

        [HttpGet]
        [Route("Id")]
        [Authorize(Roles = "Admin, Regular")]
        public IActionResult GetUser(string ID)
        {
            var ReturnUser = _userManager.FindByIdAsync(ID).Result;
            if (ReturnUser == null)
                return NotFound();
            return Ok(ReturnUser);
        }

        [HttpGet]
        [Route("Email")]
        [Authorize(Roles = "Admin, Regular")]
        public IActionResult GetUserByEmail(string Email)
        {
            var ReturnUser = _userManager.FindByEmailAsync(Email).Result;
            if (ReturnUser == null)
                return NotFound();
            return Ok(ReturnUser);
        }

        [HttpDelete]
        [Route("Delete")]
        [Authorize(Roles = "Admin")]
        public IActionResult Delete(string ID)
        {
            var ReturnUser = _userManager.FindByIdAsync(ID).Result;
            if (ReturnUser == null)
                return NotFound();
            _userManager.DeleteAsync(ReturnUser);
            var message = $"{ReturnUser.FirstName} {ReturnUser.LastName} has been deleted";
            return Ok(message);
        }


        [HttpPost]
        [Route("Add-New")]
        [Authorize(Roles = "Admin")]
        public IActionResult AddNew(CreateUserDto model)
        {

            var user = _userManager.FindByEmailAsync(model.Email);
            if (user.Result ==null)
            {
                var NewUser = new AppUser
                {
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    PhoneNumber = model.PhoneNumber,
                    Email = model.Email,
                    City = model.City,
                    State = model.State,
                    Country = model.Country,
                    FacebookLink = model.FacebookLink,
                    TwitterLink = model.TwitterLink,
                    LinkedinLink = model.LinkedinLink,
                    Image = model.ImgUrl,
                    UserName = model.Email
                    
                };

                IdentityResult result = _userManager.CreateAsync(NewUser, model.Password).Result;
                if (result.Succeeded)
                {
                    _userManager.AddToRoleAsync(NewUser, "Regular").Wait();
                }

            }
            var message = $"{model.FirstName} {model.LastName} has successfuly been created";
            return Ok(message);
        }
        [HttpPut]
        [Route("Update")]
        [Authorize(Roles = "Admin")]
        public IActionResult UpdateUser(CreateUserDto model)
        {
            var user = _userManager.FindByEmailAsync(model.Email).Result;
            if (user!= null)
            {
                user = new AppUser
                {
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    PhoneNumber = model.PhoneNumber,
                    Email = model.Email,
                    City = model.City,
                    State = model.State,
                    Country = model.Country,
                    FacebookLink = model.FacebookLink,
                    TwitterLink = model.TwitterLink,
                    LinkedinLink = model.LinkedinLink,
                    Image = model.ImgUrl,
                    UserName = model.Email
                };
                _userManager.UpdateAsync(user);
                return Ok(user);
            }
            
            return BadRequest();
        }

        [HttpGet]
        [Route("Search")]
        [Authorize(Roles = "Admin")]
        public IActionResult Search([FromQuery] UserParameter userParameter)
        {
            List<AppUserDto> usersDto = new List<AppUserDto>();
            var users = _userManager.Users.ToList();
            foreach (var item in users)
            {
                usersDto.Add(new AppUserDto
                {
                    FirstName = item.FirstName,
                    LastName = item.LastName,
                    ImgUrl = item.Image,
                    PhoneNumber = item.PhoneNumber,
                    Email = item.Email,
                    Street = item.Street,
                    City = item.City,
                    State = item.State,
                    Country = item.Country,
                    FacebookLink = item.FacebookLink,
                    LinkedinLink = item.LinkedinLink,
                    TwitterLink = item.TwitterLink
                });
            }
            var ReturnUsers = usersDto
                .Where(o => o.FirstName.Contains(userParameter.QuerySearch))
                .OrderBy(o => o.FirstName)
                .Skip(userParameter.PageNumber - 1)
                .Take(userParameter.PageSize)
                .ToList();
            return Ok(ReturnUsers);
        }


    }

}
