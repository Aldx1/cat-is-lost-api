using cat_is_lost_api.Models;
using cat_is_lost_api.Services;
using Microsoft.AspNetCore.Mvc;

namespace cat_is_lost_api.Controllers
{
    [ApiController]
    [Route("api/users")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost]
        public async Task<ActionResult<string>> AddUser([FromForm] User user)
        {
            var addUser = await _userService.AddUser(user);
            if (addUser.Item1) return Ok(addUser.Item2);
            return BadRequest();
        }
    }
}
