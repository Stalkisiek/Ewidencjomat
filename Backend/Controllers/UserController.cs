using Ewidencjomat.Services.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ewidencjomat.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class UserController : Controller
{
    private readonly IUserServices _userServices;

    public UserController(IUserServices userServices)
    {
        _userServices = userServices;
    }
    
    [HttpGet]
    public async Task<IActionResult> GetUsers([FromQuery] int? id)
    {
        if (id.HasValue)
        {
            var response = await _userServices.GetById(id.Value);
            return StatusCode(response.StatusCode, response);
        }
        else
        {
            var response = await _userServices.GetAll();
            return StatusCode(response.StatusCode, response);
        }
    }
    
    [HttpGet]
    [Route("Current")]
    public async Task<IActionResult> GetCurrentUser()
    {
        var response = await _userServices.GetYourself();
        return StatusCode(response.StatusCode, response);
    }
    
    [HttpDelete]
    public async Task<IActionResult> DeleteUser([FromQuery] int id)
    {
        var response = await _userServices.DeleteUserById(id);
        return StatusCode(response.StatusCode, response);
    }
}