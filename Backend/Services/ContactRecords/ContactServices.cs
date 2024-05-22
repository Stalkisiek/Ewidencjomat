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

            if (user.Role != UserTypes.Admin && user.Role != UserTypes.Server)
            {
                response.StatusCode = 403;
                response.Success = false;
                response.Message = "You are not authorized to view this content.";
                return response;
            }

            response.Data = await _context.ContactRecords.Select(c => _mapper.Map<GetContactRecordDto>(c))
                .ToListAsync();
            response.StatusCode = 200;
            response.Success = true;
            return response;
        }
        catch (Exception e)
        {
            if (response.StatusCode == 200)
                response.StatusCode = 500;
            response.Success = false;
            response.Message = e.Message;
            return response;
        }
    }

    public async Task<ServiceResponse<GetContactRecordDto>> GetContactByIdAsync(int id)
    {
        var response = new ServiceResponse<GetContactRecordDto>();
        try
        {
            User? user = await _context.Users.FirstOrDefaultAsync(u => u.Id == _authRepository.GetCurrentUserId());

            if (user != null && user.Role != UserTypes.Admin && user.Role != UserTypes.Server)
            {
                response.StatusCode = 403;
                response.Success = false;
                response.Message = "You are not authorized to view this content.";
                return response;
            }

            ContactRecord? contact = await _context.ContactRecords.FirstOrDefaultAsync(c => c!.Id == id);
            if (contact == null)
            {
                response.StatusCode = 404;
                response.Success = false;
                response.Message = "Contact not found.";
                return response;
            }

            response.Data = _mapper.Map<GetContactRecordDto>(contact);
            response.StatusCode = 200;
            response.Success = true;
            return response;
        }
        catch (Exception e)
        {
            if (response.StatusCode == 200)
                response.StatusCode = 500;
            response.Success = false;
            if (response.Message == String.Empty)
                response.Message = e.Message;
            return response;
        }
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
            if (response.StatusCode == 200)
                response.StatusCode = 500;
            response.Success = false;
            response.Message = e.Message;
            return response;
        }

    }

    public async Task<ServiceResponse<int>> DeleteContactAsync(int id)
    {
        var response = new ServiceResponse<int>();
        try
        {
#pragma warning disable CS8634 // The type cannot be used as type parameter in the generic type or method. Nullability of type argument doesn't match 'class' constraint.
            ContactRecord? contact = await _context.ContactRecords.Include(contactRecord => contactRecord!.User!)
#pragma warning restore CS8634 // The type cannot be used as type parameter in the generic type or method. Nullability of type argument doesn't match 'class' constraint.
                .FirstOrDefaultAsync(c => c!.Id == id);
            //only admin can delete contact
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == _authRepository.GetCurrentUserId());
            if (user!.Role != UserTypes.Admin && user.Role != UserTypes.Server)
            {
                response.StatusCode = 403;
                response.Success = false;
                response.Message = "You are not authorized to update this contact.";
                return response;
            }

            if (contact == null)
            {
                response.StatusCode = 404;
                response.Success = false;
                response.Message = "Contact not found.";
                return response;
            }

            _context.ContactRecords.Remove(contact);
            await _context.SaveChangesAsync();
            response.Data = id;
            response.StatusCode = 200;
            response.Success = true;
            return response;
        }
        catch (Exception e)
        {
            if (response.StatusCode == 200)
                response.StatusCode = 500;
            response.Success = false;
            if (response.Message == String.Empty)
                response.Message = e.Message;
            return response;
        }
    }

    public async Task<ServiceResponse<GetContactRecordDto>> UpdateContactAsync(UpdateContactRecordDto updatedContact)
    {
        var response = new ServiceResponse<GetContactRecordDto>();
        try
        {
            //only admin or owner can update contact
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == _authRepository.GetCurrentUserId());
            if (user!.Role != UserTypes.Admin && user.Role != UserTypes.Server && user.Id != updatedContact.Id)
            {
                response.StatusCode = 403;
                response.Success = false;
                response.Message = "You are not authorized to update this contact.";
                return response;
            }

            var contact = await _context.ContactRecords.FirstOrDefaultAsync(c => c!.Id == updatedContact.Id);
            if (contact == null)
            {
                response.StatusCode = 404;
                response.Success = false;
                response.Message = "Contact not found.";
                return response;
            }

            if (updatedContact.Name != null)
                contact.Name = updatedContact.Name;
            if (updatedContact.Surname != null)
                contact.Surname = updatedContact.Surname;
            if (updatedContact.FirstContactDate != null)
                contact.FirstContactDate = updatedContact.FirstContactDate;
            if (updatedContact.LastContactDate != null)
                contact.LastContactDate = updatedContact.LastContactDate;
            if (updatedContact.Gender != null)
                contact.Gender = updatedContact.Gender;
            if (updatedContact.City != null)
                contact.City = updatedContact.City;
            if (updatedContact.ReadyToMove != null)
                contact.ReadyToMove = updatedContact.ReadyToMove;
            if (updatedContact.PhoneNumber != null)
                contact.PhoneNumber = updatedContact.PhoneNumber;
            if (updatedContact.Source != null)
                contact.Source = updatedContact.Source;
            if (updatedContact.Age.HasValue)
                contact.Age = updatedContact.Age.Value;
            if (updatedContact.Occupation != null)
                contact.Occupation = updatedContact.Occupation;
            if (updatedContact.AdditionalInformation != null)
                contact.AdditionalInformation = updatedContact.AdditionalInformation;

            _context.ContactRecords.Update(contact);
            await _context.SaveChangesAsync();
            response.Data = _mapper.Map<GetContactRecordDto>(contact);
            response.StatusCode = 200;
            response.Success = true;
            return response;

        }
        catch (Exception e)
        {
            if (response.StatusCode == 200)
                response.StatusCode = 500;
            response.Success = false;
            if (response.Message == String.Empty)
                response.Message = e.Message;
            return response;
        }
    }

    public async Task<ServiceResponse<GetContactRecordDto>> UpdateContactOwnershipAsync(ChangeOwnershipContactDto changeOwnershipContactDto)
    {
        var response = new ServiceResponse<GetContactRecordDto>();
        try
        {
            var currentUser = await _context.Users.FirstOrDefaultAsync(u => u.Id == _authRepository.GetCurrentUserId());
#pragma warning disable CS8634 // The type cannot be used as type parameter in the generic type or method. Nullability of type argument doesn't match 'class' constraint.
            var contact = await _context.ContactRecords.Include(contactRecord => contactRecord!.User).FirstOrDefaultAsync(c => c!.Id == changeOwnershipContactDto.ContactId);
#pragma warning restore CS8634 // The type cannot be used as type parameter in the generic type or method. Nullability of type argument doesn't match 'class' constraint.
            
            if(contact == null)
            {
                response.StatusCode = 404;
                response.Success = false;
                response.Message = "Contact not found.";
                return response;
            }
            
            if(currentUser!.Role != UserTypes.Admin && currentUser.Role != UserTypes.Server && currentUser != contact.User)
            {
                response.StatusCode = 403;
                response.Success = false;
                response.Message = "You are not authorized to change ownership of this contact.";
                return response;
            }
            
            var newOwner = await _context.Users.FirstOrDefaultAsync(u => u.Id == changeOwnershipContactDto.NewOwnerId);
            if (newOwner == null)
            {
                response.StatusCode = 404;
                response.Success = false;
                response.Message = "New owner not found.";
                return response;
            }
            
            if(newOwner == contact.User)
            {
                response.StatusCode = 400;
                response.Success = false;
                response.Message = "New owner is the same as the current owner.";
                return response;
            }
            
            contact.User = newOwner;
            _context.ContactRecords.Update(contact);
            await _context.SaveChangesAsync();
            response.Data = _mapper.Map<GetContactRecordDto>(contact);
            response.StatusCode = 200;
            response.Success = true;
            return response;
        }
        catch (Exception e)
        {
            if (response.StatusCode == 200)
                response.StatusCode = 500;
            response.Success = false;
            if (response.Message == String.Empty)
                response.Message = e.Message;
            return response;
        }
    }
}
