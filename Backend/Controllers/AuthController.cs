using System.Runtime.CompilerServices;
using Ewidencjomat.Dtos.UserDtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ewidencjomat.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : Controller
{
    private readonly IAuthRepository _authRepository;
    
    public AuthController(IAuthRepository authRepository)
    {
        _authRepository = authRepository;
    }
    
    [HttpPost]
    [Route(nameof(Register))]
    public async Task<ActionResult<ServiceResponse<GetUserDto>>> Register(RegisterUserDto registerUserDto)
    {
            var response = await _authRepository.Register(registerUserDto);
            return StatusCode(response.StatusCode, response);
    }
    
    [HttpPost]
    [Route(nameof(Login))]
    public async Task<ActionResult<ServiceResponse<GetUserDto>>> Login(LoginUserDto loginUserDto)
    {
        var response = await _authRepository.Login(loginUserDto);
        return StatusCode(response.StatusCode, response);
    }
}