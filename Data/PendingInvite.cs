using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebChat.Data;

public class PendingInvite
{
	[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
	public Guid PendingInviteId { get; set; }

	[MaxLength(30)]
	[DisplayName("First name")]
	public string FirstName { get; set; }

	[MaxLength(30)]
	[DisplayName("Last name")]
	public string LastName { get; set; }

	[EmailAddress]
	public string Email { get; set; }
	
	[DisplayName("Password")]
	public string PasswordHash { get; set; }

	[DisplayName("Status")]
	public bool IsConfirmed { get; set; }


	[DisplayName("Invited On")]
	[DisplayFormat(DataFormatString = "{0:dd.MM.yyyy HH:mm:ss}")]
	public DateTime InvitedOn { get; set; }

	[ForeignKey(nameof(Department))]
	public Guid? DepartmentId { get; set; }

	
	public virtual Department Department { get; set; }
}
