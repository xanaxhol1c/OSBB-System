using OSBBProject1.Models;
using OSBBProject1.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace OSBBProject1.Controllers
{
    public class ChangeHistoryController : Controller
    {
        private readonly IChangeHistoryService _changeHistoryService;

        public ChangeHistoryController(IChangeHistoryService changeHistoryService)
        {
            _changeHistoryService = changeHistoryService;
        }

        // Отримуємо список всіх змін
        public async Task<IActionResult> Index()
        {
            var changes = await _changeHistoryService.GetAllChangesAsync();
            return View("~/Views/Home/ChangeHistoryList.cshtml", changes); // Відображаємо в'ю в папці Home
        }
    }
}
