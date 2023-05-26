using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using NuGet.Common;

namespace WebChat.Data;

public class WebChatContext : IdentityDbContext
{
    public new DbSet<User> Users { get; set; }
    public new DbSet<Department> Departments { get; set; }
    public new DbSet<PendingInvite> PendingInvites { get; set; }

    public WebChatContext(DbContextOptions<WebChatContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<User>()
            .HasOne(u => u.Department)
            .WithMany(d => d.Users).HasForeignKey(u => u.DepartmentId).OnDelete(DeleteBehavior.SetNull);
        
        base.OnModelCreating(builder);
    }
}
