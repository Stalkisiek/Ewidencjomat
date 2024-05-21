namespace Ewidencjomat.Dtos.ContactRecordDtos;

public class AddContactRecordDto
{
    public string Name { get; set; } = string.Empty;
    public string Surname { get; set; } = string.Empty;
    public string Gender { get; set; } = string.Empty;
    public string? City { get; set; } = string.Empty;
    public string ReadyToMove { get; set; } = string.Empty;
    public string FirstContactDate { get; set; } = string.Empty;
    public string LastContactDate { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; } = string.Empty;
    public string Source { get; set; } = string.Empty;
    public int Age { get; set; }
    public string Occupation { get; set; } = string.Empty;
    public string AdditionalInformation { get; set; } = string.Empty;
}