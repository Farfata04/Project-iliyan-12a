using WebChat.Areas.Admin.Models;
using WebChat.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace WebChat.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize]
public class ManageController : Controller
{
    private readonly UserManager<User> _userManager;

    public ManageController(UserManager<User> userManager)
    {
        _userManager = userManager;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var user = await _userManager.FindByNameAsync(HttpContext.User.Identity?.Name);

        if (user == null)
            return NotFound();

        var model = new ManageProfileModel
        {
            UserId = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            PhoneNumber = user.PhoneNumber
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Index(ManageProfileModel profile)
    {
        IdentityResult result = null;
        if (ModelState.IsValid)
        {
            var user = await _userManager.FindByIdAsync(profile.UserId);

            user.FirstName = profile.FirstName;
            user.LastName = profile.LastName;
            user.Email = profile.Email;
            user.PhoneNumber = profile.PhoneNumber;

            result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
                return LocalRedirect("/");
        }

        if(result == null)
            return RedirectToAction(nameof(Index));

        foreach (var error in result.Errors)
            ModelState.AddModelError(string.Empty, error.Description);

        return RedirectToAction(nameof(Index));
    }
}
