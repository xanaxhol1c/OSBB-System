using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OSBBProject1.Models;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace OSBBProject1.Controllers
{
    [Authorize(Roles = "Resident")]
    public class BillsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BillsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {

            var currentUser = _context.Residents.FirstOrDefault(u => u.Login == User.Identity.Name);
            int billId = currentUser.Id;

            var bills = await _context.Bills
                .Where(b => b.UserId == billId)
                .ToListAsync();

            return View("~/Views/Home/PayBills.cshtml", bills);
        }

        [HttpPost]
        public async Task<IActionResult> PayBill(int id)
        {
            var bill = await _context.Bills.FindAsync(id);
            if (bill != null)
            {
                bill.Status = true;
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Index");  
        }
    }
}
