using Ewidencjomat.Dtos.UserDtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using User = Ewidencjomat.Models.User;

namespace Ewidencjomat.Services.Users;

public class UserServices : IUserServices
{
    private readonly DataContext _context;
    private readonly IConfiguration _configuration;
    private readonly IMapper _mapper;
    private readonly IAuthRepository _auth;

    public UserServices(DataContext context, IConfiguration configuration, IMapper mapper, IAuthRepository auth)
    {
        _context = context;
        _configuration = configuration;
        _mapper = mapper;
        _auth = auth;
    }
    
    // only Admins & Server get all
    public async Task<ServiceResponse<List<GetUserDto>>> GetAll()
    {
        var response = new ServiceResponse<List<GetUserDto>>();
        try
        {
            // only Admins & Server get all
            var currentUser = await _context.Users.FirstAsync(u => u.Id == _auth.GetCurrentUserId());
            if(currentUser.Role != UserTypes.Admin && currentUser.Role != UserTypes.Server)
            {
                response.Success = false;
                response.StatusCode = 403;
                response.Message = "You are not allowed to get all users";
                return response;
            }
            
            response.Data = await _context.Users.Select(u => _mapper.Map<GetUserDto>(u)).ToListAsync();
            response.Success = true;
            response.StatusCode = 200;
            return response;
        }
        catch (Exception e)
        {
            response.Success = false;
            if(response.StatusCode == 200)
                response.StatusCode = 500;
            response.Message = e.Message;
            return response;
        }
    }

    public async Task<ServiceResponse<GetUserDto>> GetById(int id)
    {
        var response = new ServiceResponse<GetUserDto>();
        try
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
            var currentUser = await _context.Users.FirstAsync(u => u.Id == _auth.GetCurrentUserId());
            if(currentUser.Role != UserTypes.Admin && currentUser.Role != UserTypes.Server && currentUser.Id != id)
            {
                response.Success = false;
                response.StatusCode = 403;
                response.Message = "You are not allowed to get this user";
                return response;
            }
            if(user == null)
            {
                response.Success = false;
                response.StatusCode = 404;
                response.Message = "User not found";
                return response;
            }
            response.Data = _mapper.Map<GetUserDto>(user);
            response.Success = true;
            response.StatusCode = 200;
            return response;
        }
        catch (Exception e)
        {
            response.Success = false;
            if(response.StatusCode == 200)
                response.StatusCode = 500;
            response.Message = e.Message;
            return response;
        }
    }

    public async Task<ServiceResponse<GetUserDto>> GetYourself()
    {
        var response = new ServiceResponse<GetUserDto>();
        try
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == _auth.GetCurrentUserId());
            if(user == null)
            {
                response.Success = false;
                response.StatusCode = 404;
                response.Message = "User not found";
                return response;
            }
            response.Data = _mapper.Map<GetUserDto>(user);
            response.Success = true;
            response.StatusCode = 200;
            return response;
        }
        catch (Exception e)
        {
            response.Success = false;
            if(response.StatusCode == 200)
                response.StatusCode = 500;
            response.Message = e.Message;
            return response;
        }
    }
    
    public async Task<ServiceResponse<string>> DeleteUserById(int id)
    {
        var response = new ServiceResponse<string>();
        try
        {
            var currentUser = await _context.Users.FirstAsync(u => u.Id == _auth.GetCurrentUserId());
            if(currentUser.Role != UserTypes.Admin && currentUser.Role != UserTypes.Server)
            {
                response.Success = false;
                response.StatusCode = 403;
                response.Message = "You are not allowed to delete users";
                return response;
            }
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
            if(user == null)
            {
                response.Success = false;
                response.StatusCode = 404;
                response.Message = "User not found";
                return response;
            }

            if (user.Role == UserTypes.Server)
            {
                response.Success = false;
                response.StatusCode = 403;
                response.Message = "You are not allowed to delete servers";
                return response;
            }
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            response.Success = true;
            response.StatusCode = 200;
            return response;
        }
        catch (Exception e)
        {
            response.Success = false;
            if(response.StatusCode == 200)
                response.StatusCode = 500;
            response.Message = e.Message;
            return response;
        }
    }

    public async Task<ServiceResponse<GetUserDto>> UpdateEmail()
    {
        throw new NotImplementedException();
    }

    public async Task<ServiceResponse<GetUserDto>> UpdateName()
    {
        throw new NotImplementedException();
    }

    public async Task<ServiceResponse<GetUserDto>> UpdateSurname()
    {
        throw new NotImplementedException();
    }

    public async Task<ServiceResponse<GetUserDto>> UpdatePassword()
    {
        throw new NotImplementedException();
    }
}