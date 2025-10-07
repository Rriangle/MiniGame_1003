using System.ComponentModel.DataAnnotations.Schema;

namespace GameSpace.Models;

/// <summary>
/// Join entity linking manager accounts to their role permissions.
/// </summary>
[Table("ManagerRole")]
public class ManagerAccountRole
{
    [Column("Manager_Id")]
    public int ManagerId { get; set; }

    [Column("ManagerRole_Id")]
    public int ManagerRoleId { get; set; }

    public virtual ManagerDatum? Manager { get; set; }

    public virtual ManagerRolePermission? Role { get; set; }
}
