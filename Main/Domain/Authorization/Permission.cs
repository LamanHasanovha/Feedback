using Main.Domain.AppBase;
using System.ComponentModel.DataAnnotations.Schema;

namespace Main.Domain.Authorization;

[Table("Permission")]
public class Permission : Entity
{
    public string Name { get; set; }

    public int ParentId { get; set; }
}
