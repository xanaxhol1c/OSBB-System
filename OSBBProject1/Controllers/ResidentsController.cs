using Microsoft.AspNetCore.Mvc;
using OSBBProject1.Models;
using OSBBProject1.Services.Interfaces;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System;

namespace OSBBProject1.Controllers
{
    [Authorize]
    public class ResidentsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ResidentsController> _logger;
        private readonly IChangeHistoryService _changeHistoryService;

        public ResidentsController(ApplicationDbContext context, ILogger<ResidentsController> logger, IChangeHistoryService changeHistoryService)
        {
            _context = context;
            _logger = logger;
            _changeHistoryService = changeHistoryService;
        }

        [Authorize(Roles = "Admin")]
        public IActionResult ResidentsList()
        {
            var residents = _context.Residents.ToList();
            return View("~/Views/Home/ResidentsList.cshtml", residents);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            ViewBag.FlatNumbers = _context.Flats
                .Where(f => !_context.Residents.Any(r => r.FlatNumber == f.Id))
                .Select(f => f.Id)
                .ToList();
            return View("~/Views/Home/CreateResident.cshtml");
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(Resident resident)
        {
            if (ModelState.IsValid)
            {
                if (_context.Residents.Any(r => r.Login == resident.Login))
                {
                    ModelState.AddModelError("Login", "Логін вже існує.");
                }

                if (!_context.Flats.Any(f => f.Id == resident.FlatNumber) ||
                    _context.Residents.Any(r => r.FlatNumber == resident.FlatNumber))
                {
                    ModelState.AddModelError("FlatNumber", "Обраний номер квартири не існує або вже зайнятий.");
                }

                if (string.IsNullOrEmpty(resident.Name) || string.IsNullOrEmpty(resident.Login) || resident.FlatNumber == 0)
                {
                    ModelState.AddModelError("", "Всі поля мають бути заповнені.");
                }

                if (!Regex.IsMatch(resident.Name, @"^[a-zA-Zа-яА-ЯёЁЇїІіЄєҐґ]+$"))
                {
                    ModelState.AddModelError("Name", "В імені можуть бути лише букви.");
                }

                if (ModelState.ErrorCount == 0)
                {
                    int newResidentId = _context.Residents.Max(r => (int?)r.Id) ?? 0;
                    resident.Id = 0;

                    _context.Residents.Add(resident);
                    await _context.SaveChangesAsync();

                    var adminUser = _context.Admins.FirstOrDefault(a => a.Login == User.Identity.Name);
                    string adminName = adminUser?.Name ?? "Не знайдено";

                    int newLogId = _context.ChangeHistories.Max(ch => (int?)ch.Id) ?? 0;
                    newLogId += 1;

                    await _changeHistoryService.AddChangeHistory(newLogId, "Create", adminName, "Resident", resident.Id, "null", resident.Name);

                    return RedirectToAction("ResidentsList");
                }
            }

            ViewBag.FlatNumbers = _context.Flats
                .Where(f => !_context.Residents.Any(r => r.FlatNumber == f.Id))
                .Select(f => f.Id)
                .ToList();
            return View("~/Views/Home/CreateResident.cshtml", resident);
        }


        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult Delete(int id)
        {
            var resident = _context.Residents.Find(id);
            if (resident == null)
            {
                return NotFound();
            }
            return View("~/Views/Home/DeleteResident.cshtml", resident);
        }

        [HttpPost, ActionName("DeleteConfirmed")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var resident = _context.Residents.Find(id);
            if (resident != null)
            {
                _context.Residents.Remove(resident);
                await _context.SaveChangesAsync();


                var adminUser = _context.Admins.FirstOrDefault(a => a.Login == User.Identity.Name);
                string adminName = adminUser?.Name ?? "Не знайдено";

                int newLogId = _context.ChangeHistories.Max(ch => (int?)ch.Id) ?? 0;
                newLogId += 1;

                await _changeHistoryService.AddChangeHistory(newLogId, "Delete", adminName, "Resident", resident.Id, resident.Name, "null");
            }
            return RedirectToAction("ResidentsList");
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult Edit(int id)
        {
            var resident = _context.Residents.Find(id);
            if (resident == null)
            {
                return NotFound();
            }

            var currentFlatNumber = resident.FlatNumber;
            var availableFlats = _context.Flats
                .Where(f => !_context.Residents.Any(r => r.FlatNumber == f.Id) || f.Id == currentFlatNumber)
                .Select(f => f.Id)
                .ToList();

            if (!availableFlats.Contains(currentFlatNumber))
            {
                availableFlats.Add(currentFlatNumber);
            }

            ViewBag.FlatNumbers = availableFlats;
            return View("~/Views/Home/EditResident.cshtml", resident);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(Resident resident)
        {
            if (ModelState.IsValid)
            {
                if (_context.Residents.Any(r => r.Login == resident.Login && r.Id != resident.Id))
                {
                    ModelState.AddModelError("Login", "Логін вже існує.");
                }

                if (!_context.Flats.Any(f => f.Id == resident.FlatNumber) ||
                    _context.Residents.Any(r => r.FlatNumber == resident.FlatNumber && r.Id != resident.Id))
                {
                    ModelState.AddModelError("FlatNumber", "Обраний номер квартири не існує або вже зайнятий.");
                }

                if (string.IsNullOrEmpty(resident.Name) || string.IsNullOrEmpty(resident.Login) || resident.FlatNumber == 0)
                {
                    ModelState.AddModelError("", "Всі поля мають бути заповнені.");
                }

                if (!Regex.IsMatch(resident.Name, @"^[a-zA-Zа-яА-ЯёЁЇїІіЄєҐґ]+$"))
                {
                    ModelState.AddModelError("Name", "В імені можуть бути лише букви.");
                }

                if (ModelState.ErrorCount == 0)
                {
                    var existingResident = await _context.Residents.FindAsync(resident.Id);
                    if (existingResident == null)
                    {
                        return NotFound();
                    }

                    var oldValue = existingResident.Name;

                    existingResident.Name = resident.Name;
                    existingResident.FlatNumber = resident.FlatNumber;
                    existingResident.Login = resident.Login;
                    existingResident.Password = resident.Password;


                    await _context.SaveChangesAsync();

                    var adminUser = _context.Admins.FirstOrDefault(a => a.Login == User.Identity.Name);
                    string adminName = adminUser?.Name ?? "Не знайдено";


                    int newLogId = _context.ChangeHistories.Max(ch => (int?)ch.Id) ?? 0;
                    newLogId += 1;

                    await _changeHistoryService.AddChangeHistory(newLogId, "Edit", adminName, "Resident", existingResident.Id, oldValue, resident.Name);

                    return RedirectToAction("ResidentsList");
                }
            }

            var currentFlatNumber = resident.FlatNumber;

            var availableFlats = _context.Flats
                .Where(f => !_context.Residents.Any(r => r.FlatNumber == f.Id) || f.Id == currentFlatNumber)
                .Select(f => f.Id)
                .ToList();

            if (!availableFlats.Contains(currentFlatNumber))
            {
                availableFlats.Add(currentFlatNumber);
            }

            ViewBag.FlatNumbers = availableFlats;
            return View("~/Views/Home/EditResident.cshtml", resident);
        }

        public async Task<IActionResult> SearchResidents(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                var residents = await _context.Residents.ToListAsync();
                return View("~/Views/Home/ResidentsList.cshtml", residents);
            }
            else
            {
                var residents = await _context.Residents
                    .Where(r => r.Name.Contains(name))
                    .ToListAsync();

                return View("~/Views/Home/ResidentsList.cshtml", residents);
            }
        }

        [HttpGet]
        public async Task<IActionResult> MyFlatPage()
        {
            var userLogin = User.Identity.Name;

            var resident = await _context.Residents
                .FirstOrDefaultAsync(r => r.Login == userLogin);

            if (resident == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var flat = await _context.Flats
                .FirstOrDefaultAsync(f => f.Id == resident.FlatNumber);

            if (flat == null)
            {
                return RedirectToAction("Index", "Home");
            }

            return View("~/Views/Home/MyFlatPage.cshtml", flat);
        }
        // GET-метод для відображення форми
        [HttpGet]
        public IActionResult SubmitComplaint()
        {
            return View("~/Views/Home/SubmitComplaint.cshtml");
        }

        [HttpPost]
        public IActionResult SubmitComplaint(HelpRequest model)
        {
            // Отримуємо поточного користувача з бази даних за допомогою User.Identity.Name
            var currentUser = _context.Residents.FirstOrDefault(u => u.Login == User.Identity.Name);

            if (currentUser != null)
            {
                // Отримуємо ім'я користувача замість логіна
                model.UserName = currentUser.Name; // Це поле Name з вашої таблиці Users
            }
            else
            {
                // Якщо користувача не знайдено, можна повернути помилку або виконати інші дії
                return BadRequest("User not found");
            }

            // Обчислюємо новий ID для скарги
            int newRequestId = _context.HelpRequests.Max(ch => (int?)ch.Id) ?? 0;
            newRequestId += 1;
            model.Id = newRequestId;

            // Заповнюємо всі необхідні поля
            model.SendDate = DateTime.Now;  // Дата подачі скарги
            model.Status = false;  // Статус скарги (False означає, що ще не прочитано)

            // Додаємо нову скаргу в базу даних
            _context.HelpRequests.Add(model);
            _context.SaveChanges();

            // Перенаправляємо на сторінку успіху після подачі скарги
            return View("~/Views/Home/ComplaintSuccess.cshtml", model);
        }

    }

}