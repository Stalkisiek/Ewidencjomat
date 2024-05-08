using Ewidencjomat.Dtos.UserDtos;

namespace Ewidencjomat;

public class AutoMapperProfile : Profile
{
    //usage _mapper.Map<{to what type}>({from what})
    //Lists
    //serviceResposne.Data = users.Select(c => _mapper.Map<GetUserDto>(c)).ToList();
    
    public AutoMapperProfile()
    {
        // CreateMap<{from}, {to}>()
        CreateMap<RegisterUserDto, User>();
        CreateMap<User, GetUserDto>();
    }
}