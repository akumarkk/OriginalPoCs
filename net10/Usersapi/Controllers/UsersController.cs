using Microsoft.AspNetCore.Mvc;

namespace Usersapi;

[ApiController]
[Route("api/[controller]")]
public class UsersController: ControllerBase
{
    UsersService _usersService;
    public UsersController(UsersService usersService)
    {
        _usersService = usersService;
    }

    [HttpGet("users")]
    public ActionResult<List<User>> GetUsers()
    {
        return Ok(_usersService.GetUsers());
    }


    [HttpPost("users")]
    public ActionResult<User> PostUser([FromBody] User user )
    {
        _usersService.AddUser(user);
        return Ok(user);
    }
}