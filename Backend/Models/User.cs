using System.ComponentModel.DataAnnotations.Schema;

namespace Ewidencjomat.Models;

public class User
{
      public int Id { get; set; }
      public string? Name { get; set; } = String.Empty;
      public string? Surname { get; set; } = String.Empty;
      public string Email { get; set; } = String.Empty;
      public UserTypes Role { get; set; } = UserTypes.Normal;
      
      public byte[] PasswordHash {get;set;} = Array.Empty<byte>();
      public byte[] PasswordSalt {get;set;} = Array.Empty<byte>();    
}