using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OSBBProject1.Models;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;

namespace OSBBProject1.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        // Головна сторінка
        public IActionResult Index()
        {
            ViewBag.ActivePage = "Index";
            return View();
        }

        // Сторінка "Моя квартира"
        public IActionResult MyFlatPage()
        {
            ViewBag.ActivePage = "MyFlatPage";
            return View(); // Це поверне View MyFlatPage.cshtml
        }

        // Сторінка "Проекти"
        public IActionResult ProjectsPage()
        {
            ViewBag.ActivePage = "ProjectsPage";
            return View(); // Тут необхідно мати View ProjectsPage.cshtml
        }

        // Сторінка "Подати скаргу"
        public IActionResult HelpRequestPage()
        {
            ViewBag.ActivePage = "HelpRequestPage";
            return View(); // Тут необхідно мати View HelpRequestPage.cshtml
        }

        // Сторінка "Мій профіль"
        public IActionResult UserProfile()
        {
            // Отримання імені користувача з claims
            var userName = User.Identity.Name;

            // Пошук користувача в базі даних
            var adminUser = _context.Admins.FirstOrDefault(a => a.Login == userName);
            var residentUser = _context.Residents.FirstOrDefault(r => r.Login == userName);

            if (adminUser != null)
            {
                return View(adminUser); // Повертає View для адміністратора
            }
            else if (residentUser != null)
            {
                return View(residentUser); // Повертає View для резидента
            }

            // Якщо користувача не знайдено, перенаправлення на головну сторінку
            return RedirectToAction("Index");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
