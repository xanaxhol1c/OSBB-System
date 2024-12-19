using Microsoft.AspNetCore.Mvc;
using OSBBProject1.Models;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;
using Microsoft.EntityFrameworkCore;

namespace OSBBProject1.Controllers
{
    [Authorize(Roles = "Admin")]
    public class FlatsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public FlatsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult ShowFlats(int? minFloor)
        {
            var stopwatch = Stopwatch.StartNew();

            var flats = _context.Flats.AsQueryable();

            if (minFloor.HasValue)
            {
                flats = flats.Where(f => f.Floor > minFloor.Value);
            }

            var flatList = flats.ToList();
            stopwatch.Stop();
            ViewBag.ExecutionTime = stopwatch.ElapsedMilliseconds;
            ViewBag.TotalFlats = flatList.Count;

            return View("~/Views/Home/FlatsList.cshtml", flatList);
        }

        public IActionResult ShowFlatsSequential(int? minFloor)
        {
            var stopwatch = Stopwatch.StartNew();


            var flats1 = _context.Flats.AsQueryable();
            if (minFloor.HasValue)
            {
                flats1 = flats1.Where(f => f.Floor > minFloor.Value);
            }
            var flatList1 = flats1.ToList();


            var flats2 = _context.Flats.AsQueryable();
            if (minFloor.HasValue)
            {
                flats2 = flats2.Where(f => f.Floor > minFloor.Value);
            }
            var flatList2 = flats2.ToList();

            stopwatch.Stop();

            ViewBag.ExecutionTimeSequential = stopwatch.ElapsedMilliseconds; 
            ViewBag.TotalFlatsSequential = flatList1.Count; 

            return View("~/Views/Home/FlatsList.cshtml", flatList1); 
        }

        public async Task<IActionResult> ShowFlatsParallel(int? minFloor)
        {
            var stopwatch = Stopwatch.StartNew();


            var scopeFactory = HttpContext.RequestServices.GetService<IServiceScopeFactory>();


            var task1 = Task.Run(async () =>
            {
                using (var scope = scopeFactory.CreateScope())
                using (var context1 = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>())
                {
                    var flats1 = context1.Flats.AsQueryable();
                    if (minFloor.HasValue)
                    {
                        flats1 = flats1.Where(f => f.Floor > minFloor.Value);
                    }
                    return await flats1.ToListAsync();
                }
            });

            var task2 = Task.Run(async () =>
            {
                using (var scope = scopeFactory.CreateScope())
                using (var context2 = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>())
                {
                    var flats2 = context2.Flats.AsQueryable();
                    if (minFloor.HasValue)
                    {
                        flats2 = flats2.Where(f => f.Floor > minFloor.Value);
                    }
                    return await flats2.ToListAsync();
                }
            });

            var flatList1 = await task1;
            var flatList2 = await task2;

            stopwatch.Stop();

            ViewBag.ExecutionTimeParallel = stopwatch.ElapsedMilliseconds;
            ViewBag.TotalFlatsParallel = flatList1.Count;

            return View("~/Views/Home/FlatsList.cshtml", flatList1);
        }



        [HttpPost]
        public async Task<IActionResult> AddFlats()
        {
            var random = new Random();
            var flatsToAdd = new List<Flat>();


            for (int i = 1; i <= 100000; i++)
            {
                var flat = new Flat
                {
                    Id = 100 + i, 
                    Floor = random.Next(1, 10), 
                    ElectricityStatus = false, 
                    WaterStatus = false 
                };
                flatsToAdd.Add(flat);
            }


            _context.Flats.AddRange(flatsToAdd);
            await _context.SaveChangesAsync(); 

            return RedirectToAction("ShowFlats"); 
        }

        [HttpPost]
        public IActionResult RemoveFlats()
        {
            var flatsToRemove = _context.Flats.Where(f => f.Id > 100).Take(100000).ToList();

            _context.Flats.RemoveRange(flatsToRemove);
            _context.SaveChanges();

            return RedirectToAction("ShowFlats"); 
        }
    }
}
