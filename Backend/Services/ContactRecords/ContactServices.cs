using Ewidencjomat.Dtos.ContactRecordDtos;
using Microsoft.EntityFrameworkCore;

namespace Ewidencjomat.Services.ContactRecords;

public class ContactServices : IContactServices
{
    private readonly DataContext _context;
    private readonly IAuthRepository _authRepository;
    private readonly IMapper _mapper;

    public ContactServices(DataContext context, IAuthRepository authRepository, IMapper mapper)
    {
        _context = context;
        _authRepository = authRepository;
        _mapper = mapper;
    }
    public async Task<ServiceResponse<List<GetContactRecordDto>>> GetAllContactsAsync()
    {
        var response = new ServiceResponse<List<GetContactRecordDto>>();
        try
        {
            int userId = _authRepository.GetCurrentUserId();
            User? user = _context.Users.FirstOrDefault(u => u.Id == userId);
            if (user == null)
            {
                response.StatusCode = 404;
                response.Success = false;
                response.Message = "User not found.";
                return response;
            }
            
            if(user.Role != UserTypes.Admin && user.Role != UserTypes.Server)
            {
                response.StatusCode = 403;
                response.Success = false;
                response.Message = "You are not authorized to view this content.";
                return response;
            }

            response.Data = await _context.ContactRecords.Select(c => _mapper.Map<GetContactRecordDto>(c)).ToListAsync();
            response.StatusCode = 200;
            response.Success = true;
            return response;
        }
        catch (Exception e)
        {
            if(response.StatusCode == 200)
                response.StatusCode = 500;
            response.Success = false;
            response.Message = e.Message;
            return response;
        }
    }

    public async Task<ServiceResponse<GetContactRecordDto>> GetContactByIdAsync(int id)
    {
        throw new NotImplementedException();
    }

    public async Task<ServiceResponse<GetContactRecordDto>> AddContactAsync(AddContactRecordDto newContact)
    {
        var response = new ServiceResponse<GetContactRecordDto>();
        try
        {
            ContactRecord contact = new ContactRecord
            {
                Name = newContact.Name,
                Surname = newContact.Surname,
                Gender = newContact.Gender,
                City = newContact.City,
                ReadyToMove = newContact.ReadyToMove,
                FirstContactDate = newContact.FirstContactDate,
                LastContactDate = newContact.LastContactDate,
                PhoneNumber = newContact.PhoneNumber,
                Source = newContact.Source,
                Age = newContact.Age,
                Occupation = newContact.Occupation,
                AdditionalInformation = newContact.AdditionalInformation
            };
            var userId = _authRepository.GetCurrentUserId();
            contact.User = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId); 
            await _context.ContactRecords.AddAsync(contact);
            await _context.SaveChangesAsync();
            response.Data = _mapper.Map<GetContactRecordDto>(contact);
            response.StatusCode = 201;
            response.Success = true;
            return response;
        }
        catch (Exception e)
        {
            if(response.StatusCode == 200)
                response.StatusCode = 500;
            response.Success = false;
            response.Message = e.Message;
            return response;
        }
        
    }
}