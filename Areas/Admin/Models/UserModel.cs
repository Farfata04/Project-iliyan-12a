using System.ComponentModel;

namespace WebChat.Areas.Admin.Models;

public class UserModel
{
    public string Id { get; set; }

    public string Username { get; set; }

    public string FirstName { get; set; }

    public string LastName { get; set; }

    public string Email { get; set; }
    
    public string PhoneNumber { get; set; }

    [DisplayName("Department")]
    public string DepartmentName { get; set; }
    
    public string RoleId { get; set; }
    
    [DisplayName("Registered As")]
    public string RoleName { get; set; }
}
