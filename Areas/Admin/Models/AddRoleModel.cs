using Microsoft.AspNetCore.Mvc.Rendering;

namespace WebChat.Areas.Admin.Models;

public class AddRoleModel
{
    public string UserId { get; set; }

    public string SelectedRoleId { get; set; }

    public IEnumerable<SelectListItem> Roles { get; set; }
}
