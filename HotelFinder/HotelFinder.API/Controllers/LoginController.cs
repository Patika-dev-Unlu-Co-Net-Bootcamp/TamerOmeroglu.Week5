using HotelFinder.API.Authorization;
using HotelFinder.API.Filters;
using HotelFinder.API.Services;
using HotelFinder.Entity;
using HotelFinder.Entity.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using BcryptNet = BCrypt.Net.BCrypt;


namespace HotelFinder.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    [CustomResultFilter]
    public class LoginController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        public LoginController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> Register(UserRegisterDto userPrm)
        {
            //register functionality  

            var user = new User
            {
                
                UserName = userPrm.Username,
                Email = userPrm.Email,
                FirstName = userPrm.FirstName,
                LastName = userPrm.LastName,
                Password = userPrm.Password,
                PasswordHash = BcryptNet.HashPassword("user"),
                Role = Role.User,
            };

            var result = await _userManager.CreateAsync(user, user.Password);


            if (result.Succeeded)
            {
                // User sign  
                // sign in   
                var signInResult = await _signInManager.PasswordSignInAsync(user, user.Password, false, false);

                if (signInResult.Succeeded)
                {
                    return Ok();
                }
            }

            return BadRequest();
        }


        [AllowAnonymous]
        [HttpPost("[action]")]
        public IActionResult Authenticate(AuthenticateRequest model)
        {
            var response = _userService.Authenticate(model);
            return Ok(response);
        }

        [Authorize(Role.Admin)]
        [HttpGet]
        public IActionResult GetAll()
        {
            var users = _userService.GetAll();
            return Ok(users);
        }

        [HttpGet("{id:int}")]
        public IActionResult GetById(int id)
        {
            // only admins can access other user records
            var currentUser = (User)HttpContext.Items["User"];
            if (id != currentUser.Id && currentUser.Role != Role.Admin)
                return Unauthorized(new { message = "Unauthorized" });

            var user = _userService.GetById(id);
            return Ok(user);
        }
    }
}
