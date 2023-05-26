using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
namespace WebChat.Data;

public class Department
{
	[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
	[DisplayName("Department ID")]
	public Guid DepartmentId { get; set; }

	[DisplayName("Department Name")]
	public string DepartmentName { get; set; } = "";

	public ICollection<User> Users { get; set; } = new List<User>();

	public ICollection<PendingInvite> PendingInvites { get; set; } = new List<PendingInvite>();
}
