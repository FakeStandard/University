using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using University.Common;
using University.DAL.IRepository;
using University.DAL.Repository;
using University.Data;
using University.Models;

namespace University.Controllers
{
    public class StudentsController : Controller
    {
        //private readonly SchoolContext _context;
        private IStudentRepository _repository;

        public StudentsController(IStudentRepository repository)
        {
            _repository = repository;
        }

        // GET: Students
        public async Task<IActionResult> Index(
            string sortOrder,
            string currentFilter,
            string searchString,
            int? pageNumber)
        {
            ViewData["CurrentSort"] = sortOrder;

            if (searchString != null)
                pageNumber = 1;
            else
                searchString = currentFilter;

            var students = from s in _repository.GetStudents()
                           select s;

            // Filter
            ViewData["CurrentFilter"] = searchString;

            if (!string.IsNullOrEmpty(searchString))
                students = students.Where(
                    s => s.LastName.Contains(searchString) || s.FirstName.Contains(searchString));
            // Sort
            ViewData["NameSortParm"] = string.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewData["DateSortParm"] = sortOrder == "Date" ? "date_desc" : "Date";

            switch (sortOrder)
            {
                case "name_desc":
                    students = students.OrderByDescending(s => s.LastName);
                    break;
                case "Date":
                    students = students.OrderBy(s => s.EnrollmentDate);
                    break;
                case "date_desc":
                    students = students.OrderByDescending(s => s.EnrollmentDate);
                    break;
                default:
                    students = students.OrderBy(s => s.LastName);
                    break;
            }

            int pageSize = 3;

            //return View(await students.AsNoTracking().ToListAsync());
            return View(
                 PaginatedList<Student>.CreateAsync(
                    students, pageNumber ?? 1, pageSize
                ));
        }

        // GET: Students/Details/5
        public IActionResult Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = _repository.GetStudentByID(id);

            if (student == null)
            {
                return NotFound();
            }

            return View(student);
        }

        // GET: Students/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Students/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("EnrollmentDate,FirstName,LastName")] Student student)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    //_context.Add(student);
                    //await _context.SaveChangesAsync();

                    _repository.InsertStudent(student);
                    _repository.Save();

                    return RedirectToAction(nameof(Index));
                }
            }
            catch (DbUpdateException)
            {
                //Log the error (uncomment ex variable name and write a log.
                ModelState.AddModelError("", "Unable to save changes. " +
                    "Try again, and if the problem persists " +
                    "see your system administrator.");
            }

            return View(student);
        }

        // GET: Students/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            //var student = await _context.Students.FindAsync(id);
            var student = _repository.GetStudentByID(id);

            if (student == null)
            {
                return NotFound();
            }

            return View(student);
        }

        // POST: Students/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPost(int id)
        {
            if (id == null)
            {
                return NotFound();
            }

            //var student = await _context.Students.FirstOrDefaultAsync(s => s.ID == id);

            var student = _repository.GetStudentByID(id);

            if (await TryUpdateModelAsync<Student>(student, "", s => s.FirstName, s => s.LastName, s => s.EnrollmentDate))
            {
                try
                {
                    //await _context.SaveChangesAsync();
                    _repository.UpdateStudent(student);
                    _repository.Save();

                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateException)
                {
                    //Log the error (uncomment ex variable name and write a log.)
                    ModelState.AddModelError("", "Unable to save changes. " +
                        "Try again, and if the problem persists, " +
                        "see your system administrator.");
                }
            }

            return View(student);
        }

        // GET: Students/Delete/5
        public async Task<IActionResult> Delete(int? id, bool? saveChangesError = false)
        {
            if (id == null)
            {
                return NotFound();
            }

            //var student = await _context.Students
            //    .AsNoTracking()
            //    .FirstOrDefaultAsync(m => m.ID == id);

            var student = _repository.GetStudentByID(id);

            if (student == null)
            {
                return NotFound();
            }

            if (saveChangesError.GetValueOrDefault())
            {
                ViewData["ErrorMessage"] =
                    "Delete failed. Try again, and if the problem persists " +
                    "see your system administrator.";
            }

            return View(student);
        }

        // POST: Students/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            //var student = await _context.Students.FindAsync(id);

            var student = _repository.GetStudentByID(id);

            if (student == null)
            {
                return RedirectToAction(nameof(Index));
            }

            try
            {
                //_context.Students.Remove(student);
                //await _context.SaveChangesAsync();

                _repository.DeleteStudent(id);
                _repository.Save();

                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException)
            {
                //Log the error (uncomment ex variable name and write a log.)
                return RedirectToAction(nameof(Delete), new { id = id, saveChangesError = true });
            }
        }

        private bool StudentExists(int id)
        {
            //return _context.Students.Any(e => e.ID == id);

            return _repository.IsExists(id);
        }
    }
}
