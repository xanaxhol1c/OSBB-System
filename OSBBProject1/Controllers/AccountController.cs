using Microsoft.AspNetCore.Mvc;
using OSBBProject1.Models;
using System.Linq;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace OSBBProject1.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AccountController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View("~/Views/Home/Login.cshtml");
        }

        [HttpPost]
        public async Task<IActionResult> Login(string login, string password)
        {
            if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password))
            {
                ModelState.AddModelError(string.Empty, "Логін та пароль не можуть бути пустими.");
                return View("~/Views/Home/Login.cshtml");
            }

            var adminUser = _context.Admins.FirstOrDefault(a => a.Login == login);
            var residentUser = _context.Residents.FirstOrDefault(r => r.Login == login);

            if (adminUser == null && residentUser == null)
            {
                ModelState.AddModelError(string.Empty, "Користувача не знайдено.");
                return View("~/Views/Home/Login.cshtml");
            }

            bool isPasswordValid = false;

            if (adminUser != null)
            {
                isPasswordValid = adminUser.Password == password; 
            }
            else if (residentUser != null)
            {
                isPasswordValid = residentUser.Password == password; 
            }

            if (isPasswordValid)
            {

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, login),
                    new Claim(ClaimTypes.Role, adminUser != null ? "Admin" : "Resident")
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));


                return RedirectToAction("Index", "Home");
            }


            ModelState.AddModelError(string.Empty, "Неправильний логін або пароль.");
            return View("~/Views/Home/Login.cshtml"); 
        }

        [Authorize]
        public IActionResult UserProfile()
        {

            var userName = User.Identity.Name;


            var adminUser = _context.Admins.FirstOrDefault(a => a.Login == userName);
            var residentUser = _context.Residents.FirstOrDefault(r => r.Login == userName);


            if (adminUser != null)
            {
                return View(adminUser); 
            }
            else if (residentUser != null)
            {
                return View(residentUser); 
            }

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home"); 
        }
    }
}
