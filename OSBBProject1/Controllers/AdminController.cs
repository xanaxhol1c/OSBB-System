using Microsoft.AspNetCore.Mvc;
using OSBBProject1.Models;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace OSBBProject1.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> ViewComplaints()
        {

            var complaints = await _context.HelpRequests.ToListAsync();

            return View("~/Views/Home/ViewComplaints.cshtml", complaints);
        }

        [HttpPost]
        public async Task<IActionResult> MarkAsRead(int id)
        {

            var helpRequest = await _context.HelpRequests.FindAsync(id);

            if (helpRequest != null)
            {

                helpRequest.Status = true;
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("ViewComplaints");
        }


        public async Task<IActionResult> WatchBills()
        {

            var bills = await _context.Bills.ToListAsync();


            return View("~/Views/Home/WatchBills.cshtml", bills);
        }
    }
}
