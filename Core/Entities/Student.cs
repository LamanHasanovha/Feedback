using Main.Domain.AppBase;
using Main.Persistence.Attributes;

namespace Core.Entities;

[IdIdentity]
public class Student : Entity
{
    public string Name { get; set; }
}
