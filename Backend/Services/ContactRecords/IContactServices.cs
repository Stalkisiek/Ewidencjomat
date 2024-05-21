﻿using Ewidencjomat.Dtos.ContactRecordDtos;

namespace Ewidencjomat.Services.ContactRecords;

public interface IContactServices
{
    public Task<ServiceResponse<List<GetContactRecordDto>>> GetAllContactsAsync();
    public Task<ServiceResponse<GetContactRecordDto>> GetContactByIdAsync(int id);
    public Task<ServiceResponse<GetContactRecordDto>> AddContactAsync(AddContactRecordDto newContact);
}