using WebChat.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using WebChat.Areas.Admin.Models;
using WebChat.Common;

namespace WebChat.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Administrator")]
public class UserController : Controller
{
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly WebChatContext _dbContext;


    public UserController(UserManager<User> userManager, RoleManager<IdentityRole> roleManager,
        WebChatContext dbContext)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _dbContext = dbContext;
    }

    public async Task<IActionResult> Index()
    {
        var userList = await (from u in _dbContext.Users.Include(u => u.Department)
                join ur in _dbContext.UserRoles on u.Id equals ur.UserId
                join r in _dbContext.Roles on ur.RoleId equals r.Id
                select new UserModel
                {
                    Id = u.Id,
                    Username = u.UserName,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    Email = u.Email,
                    DepartmentName = u.Department.DepartmentName ?? "No department",
                    RoleId = r.Id,
                    RoleName = r.Name
                })
            .ToListAsync();
        
        return View(userList);
    }

    public IActionResult InviteUser()
    {
        return View();
    }


    public async Task<IActionResult> MoveUser(string id)
    {
        User user = await _userManager.Users.FirstAsync(u => u.Id == id);
        
        var departments = await _dbContext.Departments
            .Where(d => d.DepartmentName != Constants.AdminDepartmentName)
            .ToListAsync();

        ViewBag.DepartmentList = new SelectList(departments, "DepartmentId", "DepartmentName");

        return View(user);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> MoveUser(IFormCollection collection)
    {
        if (!ModelState.IsValid)
            return View();

        string? userId = collection["Id"];
        string? departmentId = collection["DepartmentId"];
        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == userId);

        if (departmentId == null || user == null)
            return RedirectToAction(nameof(Index));

        user.DepartmentId = Guid.Parse(departmentId);
        _dbContext.Users.Update(user);
        await _dbContext.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> AddRole(string userId)
    {
        User user = await _userManager.Users.FirstAsync(u => u.Id == userId);
        
        var role = await _roleManager.FindByNameAsync(Constants.DefaultRoleName);
        
        if(role != null)
            await _userManager.AddToRoleAsync(user, role.Name);

        return RedirectToAction(nameof(Index));
    }
}
