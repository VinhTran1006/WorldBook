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
           var authors = await _authorService.GetAuthors();
            return View("/Views/AdminViews/ManageAuthors/Index.cshtml", authors);
        }

        // GET: Authors/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            var author = await _authorService.GetAuthorByID(id.Value);
            if (author == null)
            {
                return NotFound();
            }
            return View("/Views/AdminViews/ManageAuthors/Details.cshtml", author);
        }

        // GET: Authors/Create
        public IActionResult Create()
        {
            return PartialView("~/Views/AdminViews/ManageAuthors/Create.cshtml");
        }

        // POST: Authors/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("AuthorName,AuthorDescription")] Author author)
        {   
            var newauthor = new Models.Author
            {
                AuthorName = author.AuthorName,
                AuthorDescription = author.AuthorDescription,
                IsActive = true
            };
            await _authorService.AddAsync(newauthor);
            return Json(new { success = true });
        }

        // GET: Authors/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            var author = await _authorService.GetAuthorByID(id.Value);
            return PartialView("~/Views/AdminViews/ManageAuthors/Edit.cshtml", author);
        }

        // POST: Authors/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([Bind("AuthorId,AuthorName,AuthorDescription")] Author author)
        {
             await _authorService.UpdateAuthor(author);
            return Json(new { success = true });
        }

        public async Task<IActionResult> Delete(int id)
        {
            var author = await _authorService.GetAuthorByID(id);
            return PartialView("~/Views/AdminViews/ManageAuthors/Delete.cshtml", author);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteConfirm(int id)
        {
            await _authorService.DeleteAuthor(id);
            return Json(new { success = true });
        }
    }
}
