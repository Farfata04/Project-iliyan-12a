using WebChat.Areas.Admin.Models;
using WebChat.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace WebChat.Areas.Admin.Controllers;


[Area("Admin")]
[Authorize(Roles = "Administrator")]
public class RoleController : Controller
{
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly WebChatContext _dbContext;

    public RoleController(RoleManager<IdentityRole> roleManager, WebChatContext dbContext)
    {
        _roleManager = roleManager;
        _dbContext = dbContext;
    }

    public async Task<IActionResult> Index()
    {
        var roles = await _roleManager.Roles
            .Select(r => new RoleModel
            {
                Id = r.Id,
                Name = r.Name,
                ActiveUsers = _dbContext.UserRoles.Count(ur => ur.RoleId == r.Id)
            })
            .ToListAsync();

        return View(roles);
    }

    public async Task<IActionResult> Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(IFormCollection collection)
    {
        if (!ModelState.IsValid)
            return RedirectToAction(nameof(Index));

        string roleName = collection["Name"];
        var result = await _roleManager.CreateAsync(new IdentityRole(roleName));

        foreach (var error in result.Errors)
            ModelState.AddModelError(string.Empty, error.Description);

        return RedirectToAction(nameof(Index));
    }

    [HttpDelete]
    public async Task<IActionResult> Delete(string roleName)
    {
        var role = _roleManager.Roles.FirstOrDefault(r => r.Name.Equals(roleName));
        if (role != null)
            _roleManager.DeleteAsync(role);

        return RedirectToAction(nameof(Index));
    }
}
