using Ewidencjomat.Dtos.UserDtos;
using Ewidencjomat.Models;
namespace Ewidencjomat.Services.Users;

public interface IUserServices
{
    Task<ServiceResponse<List<GetUserDto>>> GetAll();
    Task<ServiceResponse<GetUserDto>> GetById(int id);
    Task<ServiceResponse<GetUserDto>> GetYourself();
    Task<ServiceResponse<string>> DeleteUserById(int id);

    //Updates
    Task<ServiceResponse<GetUserDto>> UpdateUserEmail(UpdateUserEmailDto updateUserEmailDto);
    Task<ServiceResponse<GetUserDto>> UpdateUserName(UpdateUserNameDto updateUserNameDto);
    Task<ServiceResponse<GetUserDto>> UpdateUserSurname(UpdateUserSurnameDto updateUserSurnameDto);
    Task<ServiceResponse<GetUserDto>> UpdateUserPassword(UpdateUserPasswordDto updateUserPasswordDto);
}