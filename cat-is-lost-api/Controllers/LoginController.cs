using cat_is_lost_api.Models;
using cat_is_lost_api.Services;
using Microsoft.AspNetCore.Mvc;

namespace cat_is_lost_api.Controllers
{
    [ApiController]
    [Route("api/login")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public class LoginController : ControllerBase
    {
        private readonly IUserService _userService;

        public LoginController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost]
        public async Task<ActionResult<string>> Login([FromForm] Login login)
        {
           var token = await _userService.AuthenticateUser(login);
           if (token == null) { return Unauthorized(); }
           return Ok(token);
        }
    }
}
