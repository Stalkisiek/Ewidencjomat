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
    public async Task<IActionResult> GetAllContactRecords()
    {
        var response = await _contactServices.GetAllContactsAsync();
        return StatusCode(response.StatusCode, response);
    }
    
    [HttpPost]
    public async Task<IActionResult> AddContactRecord(AddContactRecordDto newContact)
    {
        var response = await _contactServices.AddContactAsync(newContact);
        return StatusCode(response.StatusCode, response);
    }
}