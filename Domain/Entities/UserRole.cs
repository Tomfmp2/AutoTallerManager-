using Domain.Common;

namespace Domain.Entities;

public class UserRole : BaseEntity
{
    public int UserId { get; set; }
    public int RoleId { get; set; }

    //Navegacion
    public User? User { get; set; }
    public Role? Role { get; set; }
}