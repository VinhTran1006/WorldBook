using Microsoft.AspNetCore.Mvc;
using WorldBook.Models;
using WorldBook.Services.Interfaces;
using WorldBook.ViewModel;

namespace WorldBook.Controllers
{
    public class PublisherController : Controller
    {
        private readonly IPublisherService _publisherService;

        public PublisherController(IPublisherService publisherService)
        {
            _publisherService = publisherService;
        }

        // GET: Publisher
        public async Task<IActionResult> Index()
        {
            var publishers = await _publisherService.GetAllPublishersAsync();
            return View("~/Views/AdminViews/ManagePublisher/Index.cshtml", publishers);
        }

        // GET: Publisher/Create
        public IActionResult Create()
        {
            return View("~/Views/AdminViews/ManagePublisher/Create.cshtml");
        }

        // POST: Publisher/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PublisherViewModel model)
        {
            if (ModelState.IsValid)
            {
                var publisher = new Publisher
                {
                    PublisherName = model.PublisherName,
                    IsActive = true
                };

                await _publisherService.CreatePublisherAsync(publisher);
                TempData["SuccessMessage"] = "Publisher created successfully!";
                return RedirectToAction(nameof(Index));
            }
            return View("~/Views/AdminViews/ManagePublisher/Create.cshtml", model);
        }

        // GET: Publisher/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var publisher = await _publisherService.GetPublisherByIdAsync(id.Value);
            if (publisher == null)
            {
                return NotFound();
            }

            var model = new PublisherViewModel
            {
                PublisherId = publisher.PublisherId,
                PublisherName = publisher.PublisherName ?? string.Empty
            };

            return View("~/Views/AdminViews/ManagePublisher/Edit.cshtml", model);
        }

        // POST: Publisher/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, PublisherViewModel model)
        {
            if (id != model.PublisherId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var publisher = new Publisher
                    {
                        PublisherId = model.PublisherId,
                        PublisherName = model.PublisherName,
                        IsActive = true // Giữ nguyên active khi edit
                    };

                    await _publisherService.UpdatePublisherAsync(publisher);
                    TempData["SuccessMessage"] = "Publisher updated successfully!";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception)
                {
                    if (!await _publisherService.PublisherExistsAsync(model.PublisherId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            return View("~/Views/AdminViews/ManagePublisher/Edit.cshtml", model);
        }

        // GET: Publisher/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var publisher = await _publisherService.GetPublisherByIdAsync(id.Value);
            if (publisher == null)
            {
                return NotFound();
            }

            return View("~/Views/AdminViews/ManagePublisher/Delete.cshtml", publisher);
        }

        // POST: Publisher/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var result = await _publisherService.DeletePublisherAsync(id);
            if (result)
            {
                TempData["SuccessMessage"] = "Publisher deleted successfully!";
            }
            else
            {
                TempData["ErrorMessage"] = "Could not delete publisher!";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}