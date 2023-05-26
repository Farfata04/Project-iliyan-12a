using WebChat.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebChat.Common;

namespace WebChat.Areas.Admin.Controllers;

[Area("Admin")]
public class RegisterController : Controller
{
    private readonly SignInManager<User> _signInManager;
    private readonly UserManager<User> _userManager;
    private readonly IUserStore<User> _userStore;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly WebChatContext _context;

    public RegisterController(UserManager<User> userManager, IUserStore<User> userStore,
        SignInManager<User> signInManager, RoleManager<IdentityRole> roleManager, WebChatContext context)
    {
        _userManager = userManager;
        _userStore = userStore;
        _signInManager = signInManager;
        _roleManager = roleManager;
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> Index(string username = "admin", string password = "Admin!00")
    {
        if (_userManager.Users.Any())
            return RedirectToAction(nameof(Index), "Home");

        var entry = await _context.Departments.AddAsync(new Department()
        {
            DepartmentName = "Administrator"
        });

        await _context.SaveChangesAsync();

        var user = new User()
        {
            UserName = username,
            FirstName = "Administrator",
            LastName = "",
            Email = "admin@mail.com",
            DepartmentId = entry.Entity.DepartmentId,
            Department = entry.Entity
        };

        await _userStore.SetUserNameAsync(user, user.UserName, CancellationToken.None);
        var result = await _userManager.CreateAsync(user, password);

        if (!result.Succeeded)
            return RedirectToAction(nameof(Index), "Home");

        await _roleManager.CreateAsync(new IdentityRole(Constants.AdministratorRoleName));

        await _userManager.AddToRoleAsync(user, Constants.AdministratorRoleName);

        await _signInManager.SignInAsync(user, isPersistent: false);

        return RedirectToAction(nameof(Index), "Manage");
    }
}