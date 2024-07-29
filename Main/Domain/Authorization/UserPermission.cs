using Main.Domain.AppBase;
using System.ComponentModel.DataAnnotations.Schema;

namespace Main.Domain.Authorization;

[Table("UserPermission")]
public class UserPermission : Entity
{
    public int UserId { get; set; }

    public int PermissionId { get; set; }
}
