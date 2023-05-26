using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace WebChat.Data;

public class User : IdentityUser
{
    [MaxLength(25)]
    [DisplayName("First name")]
    public string FirstName { get; set; } = "";

    [MaxLength(25)]
    [DisplayName("Last name")]
    public string LastName { get; set; } = "";

    [ForeignKey(nameof(Department))]
    public Guid? DepartmentId { get; set; } = null;

    public virtual Department Department { get; set; } = null;
}
