namespace Ewidencjomat.Dtos.UserDtos;

public class RegisterUserDto
{
    public string Email { get; set; } = String.Empty;
    public string Name { get; set; } = String.Empty;
    public string Surname { get; set; } = String.Empty;
    public UserTypes Role { get; set; } = UserTypes.Normal;
    public string Password { get; set; } = String.Empty;
}