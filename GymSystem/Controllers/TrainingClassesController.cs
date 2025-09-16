using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DataModel;
using Microsoft.AspNetCore.Authorization;
using DataModel.Service;

namespace GymSystem.Controllers
{
    [Authorize(Roles ="C")]
    public class TrainingClassesController : Controller
    {
        private readonly AppDbContext _context;
        private readonly LogService _logService;

        public TrainingClassesController(AppDbContext context,LogService logService)
        {
            _context = context;
            _logService = logService;
        }

        // GET: TrainingClasses
        public async Task<IActionResult> Index()
        {
            return View(await _context.TrainingClasses.ToListAsync());
        }



        // GET: TrainingClasses/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: TrainingClasses/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ClassTypeID,ClassName,ClassLength,ClassDescription")] TrainingClass trainingClass)
        {
            if (ModelState.IsValid)
            {
                _context.Add(trainingClass);
                _logService.Log(trainingClass.ClassTypeID);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(trainingClass);
        }

        // GET: TrainingClasses/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var trainingClass = await _context.TrainingClasses.FindAsync(id);
            if (trainingClass == null)
            {
                return NotFound();
            }
            return View(trainingClass);
        }

        // POST: TrainingClasses/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("ClassTypeID,ClassName,ClassLength,ClassDescription")] TrainingClass trainingClass)
        {
            if (id != trainingClass.ClassTypeID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _logService.Log(trainingClass.ClassTypeID);
                    _context.Update(trainingClass);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TrainingClassExists(trainingClass.ClassTypeID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(trainingClass);
        }


        private bool TrainingClassExists(string id)
        {
            return _context.TrainingClasses.Any(e => e.ClassTypeID == id);
        }
    }
}
