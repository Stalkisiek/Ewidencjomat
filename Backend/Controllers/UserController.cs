using Ewidencjomat.Dtos.UserDtos;
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
    
    [HttpPut]
    [Route("Email")]
    public async Task<IActionResult> UpdateEmail([FromBody] UpdateUserEmailDto updateUserEmailDto)
    { 
        var response = await _userServices.UpdateUserEmail(updateUserEmailDto);
        return StatusCode(response.StatusCode, response);
    }
    
    [HttpPut]
    [Route("Name")]
    public async Task<IActionResult> UpdateName([FromBody] UpdateUserNameDto updateUserNameDto)
    {
        var response = await _userServices.UpdateUserName(updateUserNameDto);
        return StatusCode(response.StatusCode, response);
    }
    
    [HttpPut]
    [Route("Surname")]
    public async Task<IActionResult> UpdateSurname([FromBody] UpdateUserSurnameDto updateUserSurnameDto)
    {
        var response = await _userServices.UpdateUserSurname(updateUserSurnameDto);
        return StatusCode(response.StatusCode, response);
    }
    
    [HttpPut]
    [Route("Password")]
    public async Task<IActionResult> UpdatePassword([FromBody] UpdateUserPasswordDto updateUserPasswordDto)
    {
        var response = await _userServices.UpdateUserPassword(updateUserPasswordDto);
        return StatusCode(response.StatusCode, response);
    }
}