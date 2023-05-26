using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebChat.Areas.Admin.Models;
using WebChat.Data;

namespace WebChat.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize]
public class DepartmentController : Controller
{
	private readonly WebChatContext _context;
	
	public DepartmentController(WebChatContext dbContext)
	{
		_context = dbContext;
	}

	[HttpGet]
	public async Task<IActionResult> Index()
	{
		var departments = await _context.Departments
			.Include(d => d.Users)
			.ToListAsync();

		return View(departments);
	}

	[HttpGet]
	public async Task<IActionResult> Update(Guid id)
	{
		var entry = await _context.Departments.FirstOrDefaultAsync(c => c.DepartmentId == id);

		ViewData["Title"] = entry != null ? $"Change {entry.DepartmentName}" : "Create new department";
		return View(entry);
	}

	[HttpPost]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> Update(Department department)
	{
		if (await _context.Departments.ContainsAsync(department))
			_context.Departments.Update(department);
		else
			await _context.Departments.AddAsync(department);


		await _context.SaveChangesAsync();
		return RedirectToAction("Index");
	}

	[HttpGet]
	public async Task<IActionResult> ManageUsers(Guid id)
	{
		var dep = await _context.Departments.FirstAsync(d => d.DepartmentId == id);

		var userList = await (from u in _context.Users where u.DepartmentId == id
				join ur in _context.UserRoles on u.Id equals ur.UserId
				join r in _context.Roles on ur.RoleId equals r.Id
				select new UserModel
				{
					Id = u.Id,
					Username = u.UserName,
					FirstName = u.FirstName,
					LastName = u.LastName,
					Email = u.Email,
					RoleId = r.Id,
					RoleName = r.Name
				}).ToListAsync();
		
		
		ViewData["Title"] = $"People in {dep.DepartmentName}";
		
		return View(userList);
	}

	[HttpGet]
	public async Task<IActionResult> Delete(Guid id)
	{
		var dpt = await _context.Departments.Include(d => d.Users).FirstOrDefaultAsync(c => c.DepartmentId.Equals(id));

		if (dpt == null)
			return RedirectToAction("Index");
		
		_context.Departments.Remove(dpt);
		await _context.SaveChangesAsync();

		return RedirectToAction("Index");
	}
}
