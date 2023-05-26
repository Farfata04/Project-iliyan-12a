using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace WebChat.Areas.Admin.Models;

public class RoleModel
{
    public string Id { get; set; }

    [Required]
    [DisplayName("Role name")]
    [MinLength(3)]
    public string Name { get; set; }

    [DisplayName("Active Users")]
    public int ActiveUsers { get; set; }
}
