namespace Ewidencjomat.Dtos.ContactRecordDtos;

public class UpdateContactRecordDto
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Surname { get; set; }
    public string? Gender { get; set; }
    public string? City { get; set; }
    public string? ReadyToMove { get; set; }
    public string? FirstContactDate { get; set; }
    public string? LastContactDate { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Source { get; set; }
    public int? Age { get; set; }
    public string? Occupation { get; set; }
    public string? AdditionalInformation { get; set; }
}