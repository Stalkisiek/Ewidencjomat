using Ewidencjomat.Dtos.UserDtos;

namespace Ewidencjomat.Data;

public interface IAuthRepository
{
    Task<ServiceResponse<GetUserDto>> Register(RegisterUserDto registerUserDto);
    Task<ServiceResponse<string>> Login(LoginUserDto loginUserDto); // returns JWT
    Task<bool> UserExists(string email);
    int GetCurrentUserId();
    bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt);
    void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt);

}