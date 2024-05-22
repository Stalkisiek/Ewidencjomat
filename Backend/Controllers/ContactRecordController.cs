using Ewidencjomat.Dtos.ContactRecordDtos;
using Ewidencjomat.Services.ContactRecords;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ewidencjomat.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ContactRecordController : Controller
{
    private readonly IContactServices _contactServices;
    
    public ContactRecordController(IContactServices contactServices)
    {
        _contactServices = contactServices;
    }

    [HttpGet]
    public async Task<IActionResult> GetContactRecords(int? Id)
    {
        if(Id.HasValue)
        {
            var response = await _contactServices.GetContactByIdAsync(Id.Value);
            return StatusCode(response.StatusCode, response);
        }
        else
        {
            var response = await _contactServices.GetAllContactsAsync();
            return StatusCode(response.StatusCode, response);
        }
    }
    
    [HttpPost]
    public async Task<IActionResult> AddContactRecord(AddContactRecordDto newContact)
    {
        var response = await _contactServices.AddContactAsync(newContact);
        return StatusCode(response.StatusCode, response);
    }
    
    [HttpDelete]
    public async Task<IActionResult> DeleteContactRecord([FromQuery]int id)
    {
        var response = await _contactServices.DeleteContactAsync(id);
        return StatusCode(response.StatusCode, response);
    }
    
    [HttpPut]
    public async Task<IActionResult> UpdateContactRecord(UpdateContactRecordDto updatedContact)
    {
        var response = await _contactServices.UpdateContactAsync(updatedContact);
        return StatusCode(response.StatusCode, response);
    }
    
    [HttpPut("ownership")]
    public async Task<IActionResult> UpdateContactOwnership(ChangeOwnershipContactDto changeOwnershipContactDto)
    {
        var response = await _contactServices.UpdateContactOwnershipAsync(changeOwnershipContactDto);
        return StatusCode(response.StatusCode, response);
    }
}