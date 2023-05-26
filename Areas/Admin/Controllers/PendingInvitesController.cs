using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebChat.Common;
using WebChat.Data;

namespace WebChat.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class PendingInvitesController : Controller
    {
        private readonly WebChatContext _context;

        public PendingInvitesController(WebChatContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var webChatContext = await _context.PendingInvites.Include(p => p.Department).ToListAsync();
            return View(webChatContext);
        }

        public async Task<IActionResult> Create()
        {
            var departments = await _context.Departments
                .Where(d => d.DepartmentName != Constants.AdminDepartmentName)
                .ToListAsync();

            ViewData["DepartmentList"] = new SelectList(departments, "DepartmentId", "DepartmentName");

            var model = new PendingInvite { PasswordHash = Password.Generate() };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("FirstName,LastName,Email,PasswordHash,DepartmentId")] PendingInvite pendingInvite)
        {
            try
            {
                pendingInvite.PendingInviteId = Guid.NewGuid();
                pendingInvite.IsConfirmed = false;

                //TODO sent SMTP message here before safe into database
                //make connection to email server
                //create email body
                
                //use smtp client to send email to receiver 

                

                pendingInvite.PasswordHash = Password.Hash(pendingInvite.PasswordHash);
                pendingInvite.InvitedOn = DateTime.Now;
                _context.Add(pendingInvite);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            catch (Exception e)
            {
                ViewData["DepartmentList"] = new SelectList(_context.Departments, "DepartmentId", "DepartmentName",
                    pendingInvite.DepartmentId);
                return View(pendingInvite);
            }
        }
        
        public async Task<IActionResult> Delete(Guid id)
        {
            var pendingInvite = await _context.PendingInvites
                .Include(p => p.Department)
                .FirstOrDefaultAsync(m => m.PendingInviteId == id);
            
            return pendingInvite == null ? NotFound() : View(pendingInvite);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            if (await _context.PendingInvites.AnyAsync() == false)
                return Problem("PendingInvites set is empty");
            
            var pendingInvite = await _context.PendingInvites.FindAsync(id);
            
            if (pendingInvite != null)
                _context.PendingInvites.Remove(pendingInvite);
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
