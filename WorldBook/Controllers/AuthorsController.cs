using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WorldBook.Models;
using WorldBook.Services;
using WorldBook.Services.Interfaces;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace WorldBook.Controllers
{
    public class AuthorsController : Controller
    {
        private readonly IAuthorService _authorService;

       public AuthorsController(IAuthorService authorService)
        {
            _authorService = authorService;
        }

        // GET: Authors
        public async Task<IActionResult> Index()
        {
            var author = await _authorService.GetAuthorsAsync();
            return View("/Views/AdminViews/ManageAuthor/Index.cshtml",author);
        }

        // GET: Authors/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var author = await _authorService.GetByIdAsync(id.Value);
            if (author == null)
            {
                return NotFound();
            }

            return PartialView("/Views/AdminViews/ManageAuthor/Details.cshtml", author);
        }

        // GET: Authors/Create
        public IActionResult Create()
        {
            return PartialView("/Views/AdminViews/ManageAuthor/Create.cshtml");
        }

        // POST: Authors/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("AuthorId,AuthorName,AuthorDescription,IsActive")] Author author)
        {
            var newAuthor = new Models.Author
            {
                AuthorName = author.AuthorName,
                AuthorDescription = author.AuthorDescription,
                IsActive = true
            };
            await _authorService.AddAsync(newAuthor);
            return Json(new { success = true });
        }

        // GET: Authors/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound("/Views/AdminViews/ManageAuthor/Edit.cshtml");
               
            }

            var author = await _authorService.GetByIdAsync(id.Value);
            if (author == null) 
            {
                return  NotFound("/Views/AdminViews/ManageAuthor/Edit.cshtml");
               
            }
            return   PartialView("/Views/AdminViews/ManageAuthor/Edit.cshtml",author);
        }

        // POST: Authors/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("AuthorId,AuthorName,AuthorDescription,IsActive")] Author author)
        {
            if (id != author.AuthorId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _authorService.UpdateAsync(author);
                }
                catch (DbUpdateConcurrencyException)
                {
                }
                return Json(new { success = false });
            }
            return Json(new { success = true });
        }

        // GET: Authors/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var author = await _authorService.GetByIdAsync(id.Value);
            if (author == null)
            {
                return NotFound();
            }

            return PartialView("/Views/AdminViews/ManageAuthor/Delete.cshtml", author);
        }

        // POST: Authors/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {

           await _authorService.DeleteAsync(id);
            return Json(new { success = true });
        }
    }
}
