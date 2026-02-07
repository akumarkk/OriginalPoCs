using Microsoft.AspNetCore.Mvc;

namespace Usersapi;

[ApiController]
[Route("api/[controller]")]
public class UsersController: ControllerBase
{
    IUsersService _usersService;
    public UsersController(IUsersService usersService)
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
        return CreatedAtAction(nameof(PostUser), user);
    }
}