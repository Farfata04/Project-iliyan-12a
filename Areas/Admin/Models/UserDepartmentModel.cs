using Microsoft.AspNetCore.Mvc.Rendering;

namespace WebChat.Areas.Admin.Models;

public class UserDepartmentModel
{
	public Guid DepartmentId { get; set; }

	public string DepartmentName { get; set; }

	public IEnumerable<SelectListItem> UnassignedUsers { get; set; }
}
