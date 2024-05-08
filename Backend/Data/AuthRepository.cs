using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Ewidencjomat.Dtos.UserDtos;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace Ewidencjomat.Data;

public class AuthRepository : IAuthRepository
{
    private readonly DataContext _context;
    private readonly IConfiguration _configuration;
    private readonly IMapper _mapper;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AuthRepository(DataContext context, IConfiguration configuration, IMapper mapper, IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _configuration = configuration;
        _mapper = mapper;
        _httpContextAccessor = httpContextAccessor;
    }
    
    public async Task<ServiceResponse<GetUserDto>> Register(RegisterUserDto registerUser)
    {
        var response = new ServiceResponse<GetUserDto>();
        try
        {
            var user = new User
            {
                Email = registerUser.Email.ToLower(),
                Name = registerUser.Name.ToLower(),
                Surname = registerUser.Surname.ToLower(),
                Role = registerUser.Role
            };
            
            var password = registerUser.Password;
            if (await UserExists(user.Email))
            {
                response.StatusCode = 409;
                throw new Exception("User already exists!");
            }
            
            if(user.Role == UserTypes.Admin || user.Role == UserTypes.Server)
            {
                response.Success = false;
                response.StatusCode = 403;
                response.Message = "You are not allowed to create this type of user";
                return response;
            }
            
            user.Name = user.Name.First().ToString().ToUpper() + user.Name.Substring(1);
            user.Surname = user.Surname.First().ToString().ToUpper() + user.Surname.Substring(1);
            
            CreatePasswordHash(password, out byte[] passwordHash, out byte[] passwordSalt);

            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;
            
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            // response.Data = _mapper.Map<GetUserDto>(user);
            response.Data = _mapper.Map<GetUserDto>(user);
            response.StatusCode = 201;
            response.Message = "User created";
            return response;
        }
        catch (Exception e)
        {
            response.Message = e.Message;
            if(response.StatusCode == 200)
                response.StatusCode = 500;
            response.Success = false;
            return response;
        }
    }

    public async Task<ServiceResponse<string>> Login(LoginUserDto loginUserDto)
    {
        var response = new ServiceResponse<string>();
        try
        {
            string email = loginUserDto.Email.ToLower();
            string password = loginUserDto.Password;
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower());
            // Check if user exists
            if (user is null)
            {
                response.StatusCode = 404;
                throw new Exception("User doesnt exist");
            }
            
            // Check if password is correct
            CreatePasswordHash(password, out byte[] passwordHash, out byte[] passwordSalt);
            if (!VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt))
            {
                response.StatusCode = 401;
                throw new Exception("Wrong password");
            }
            
            // If all is good
            response.Data = CreateToken(user);
            response.Message = "You are logged in";
            return response;
        }
        catch (Exception e)
        {
            response.Message = e.Message;
            response.Success = false;
            if(response.StatusCode == 200)
                response.StatusCode = 500;
            return response;
        }
    }


    public async Task<bool> UserExists(string email)
    {
        if (await _context.Users.AnyAsync(x => x.Email.ToLower() == email.ToLower()))
        {
            return true;
        }
        return false;
    }

    public int GetCurrentUserId()
    { 
        return int.Parse(_httpContextAccessor.HttpContext!.User.FindFirstValue(ClaimTypes.NameIdentifier) ?? throw new InvalidOperationException());
    }
    
    public void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
    {
        using var hmac = new System.Security.Cryptography.HMACSHA512();
        passwordSalt = hmac.Key;
        passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
    }

    public bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
    {
        using var hmac = new System.Security.Cryptography.HMACSHA512(passwordSalt);
        var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
        return computedHash.SequenceEqual(passwordHash);
    }
    private string CreateToken(User user)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Email)
        };
        var appSettingsToken = _configuration.GetSection("AppSettings:Token").Value;
        if (appSettingsToken is null)
        {
            throw new Exception("Token is null");
        }

        SymmetricSecurityKey key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(appSettingsToken));
        SigningCredentials credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.Now.AddDays(120), // how much token will exists
            SigningCredentials = credentials
        };

        JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
        SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);

        return tokenHandler.WriteToken(token);
    }

}