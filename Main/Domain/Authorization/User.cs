using Main.Domain.AppBase;
using System.ComponentModel.DataAnnotations.Schema;

namespace Main.Domain.Authorization;

[Table("Users")]
public class User : FullAuditedEntity
{
    public string FirstName { get; set; }

    public string LastName { get; set; }

    public string Username { get; set; }

    public string Email { get; set; }

    public byte[] PasswordHash { get; set; }

    public byte[] PasswordSalt { get; set; }

    public bool IsActive { get; set; }
}
