using WebChat.Data;
using WebChat.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebChat.Common;

namespace WebChat.Controllers;

public class LoginController : Controller
{
    private readonly SignInManager<User> _signInManager;
    private readonly UserManager<User> _userManager;
    private readonly IUserStore<User> _userStore;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly WebChatContext _context;


    public LoginController(UserManager<User> userManager, IUserStore<User> userStore,
        SignInManager<User> signInManager, RoleManager<IdentityRole> roleManager, WebChatContext context)
    {
        _userManager = userManager;
        _userStore = userStore;
        _signInManager = signInManager;
        _roleManager = roleManager;
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> Index() => View();

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Index(LoginModel model)
    {
        if (ModelState.IsValid)
        {
            var invite = await _context.PendingInvites.Where(pi => !pi.IsConfirmed)
                .FirstOrDefaultAsync(pi => pi.Email == model.Username);

            if (invite != null && invite.PasswordHash == Password.Hash(model.Password))
                return RedirectToAction(nameof(Register), invite);

            var result = await _signInManager.PasswordSignInAsync(model.Username, model.Password, model.RememberMe,
                lockoutOnFailure: false);

            //TODO fix this to go Account/Manage => move ManageController to Account area
            if (result.Succeeded)
                return LocalRedirect("/Home/Chat");
        }

        ModelState.AddModelError(string.Empty, "Invalid login attempt.");
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Index", "Home");
    }


    public async Task<IActionResult> Register(PendingInvite invite)
        => View(new RegisterModel { PendingInviteId = invite.PendingInviteId });

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var invite = await _context.PendingInvites.FirstAsync(p => p.PendingInviteId == model.PendingInviteId);

        var user = new User
        {
            FirstName = invite.FirstName,
            LastName = invite.LastName,
            UserName = invite.Email,
            Email = invite.Email,
            DepartmentId = invite.DepartmentId
        };

        await _userStore.SetUserNameAsync(user, user.UserName, CancellationToken.None);

        var result = await _userManager.CreateAsync(user, model.Password);

        if (result.Succeeded)
        {
            var defaultRole = await _roleManager.FindByNameAsync(Constants.DefaultRoleName);
            if (defaultRole != null)
                await _userManager.AddToRoleAsync(user, defaultRole.Name);

            invite.IsConfirmed = true;
            _context.PendingInvites.Update(invite);
            await _context.SaveChangesAsync();

            await _signInManager.SignInAsync(user, isPersistent: false);
            return RedirectToAction(nameof(Index), "Home");
        }

        foreach (var error in result.Errors)
            ModelState.AddModelError(string.Empty, error.Description);

        return View(model);
    }
}