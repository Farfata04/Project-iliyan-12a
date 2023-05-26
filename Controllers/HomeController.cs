using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using WebChat.Data;
using WebChat.Hubs;
using WebChat.Models;

namespace WebChat.Controllers;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [Authorize]
    public async Task<IActionResult> Chat([FromServices] UserManager<User> userManager, [FromServices] WebChatContext context, [FromServices]IHubContext<ChatHub> chatContext)
    {
        var user = await userManager.GetUserAsync(User);
        var department = await context.Departments
            .Include(d => d.Users)
            .FirstAsync(d => d.DepartmentId.Equals(user.DepartmentId));

        ViewBag.DepartmentName = department.DepartmentName;

        ViewBag.People = department.Users.ToList(); 
        
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
