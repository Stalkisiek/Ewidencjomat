using HostingEnvironmentExtensions = Microsoft.Extensions.Hosting.HostingEnvironmentExtensions;

namespace Ewidencjomat.Dtos.UserDtos;

public class LoginUserDto
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}