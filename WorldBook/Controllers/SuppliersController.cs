using Microsoft.AspNetCore.Mvc;
using WorldBook.Models;
using WorldBook.Services.Interfaces;

namespace WorldBook.Controllers
{
    public class SuppliersController : Controller
    {
        private readonly ISupplierService _supplierService;

        public SuppliersController(ISupplierService supplierService)
        {
            _supplierService = supplierService;
        }
        public async Task<IActionResult> Index()
        {
            var suppliers = await _supplierService.GetSuppliers();
            return View("/Views/AdminViews/ManageSuppliers/Index.cshtml", suppliers);
        }

        // GET: Authors/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            var supp = await _supplierService.GetSupplierByID(id.Value);
            if (supp == null)
            {
                return NotFound();
            }
            return View("/Views/AdminViews/ManageSuppliers/Details.cshtml", supp);
        }

        // GET: Authors/Create
        public IActionResult Create()
        {
            return PartialView("~/Views/AdminViews/ManageSuppliers/Create.cshtml");
        }

        // POST: Authors/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create( Supplier supplier)
        {
            var newsupp = new Models.Supplier
            {
                SupplierName = supplier.SupplierName,
                SupplierEmail = supplier.SupplierEmail,
                PhoneNumber = supplier.PhoneNumber,
                ContactPerson  = supplier.ContactPerson,
                Address = supplier.Address,
            };
            await _supplierService.AddAsync(newsupp);
            return Json(new { success = true });
        }

        // GET: Authors/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            var supp = await _supplierService.GetSupplierByID(id.Value);
            return PartialView("~/Views/AdminViews/ManageSuppliers/Edit.cshtml", supp);
        }

        // POST: Authors/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([Bind("SupplierId,SupplierName,SupplierEmail,PhoneNumber,ContactPerson,Address")] Supplier supp)
        {
            await _supplierService.UpdateSupplier(supp);
            return Json(new { success = true });
        }

        public async Task<IActionResult> Delete(int id)
        {
            var author = await _supplierService.GetSupplierByID(id);
            return PartialView("~/Views/AdminViews/ManageSuppliers/Delete.cshtml", author);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteConfirm(int id)
        {
            await _supplierService.DeleteSupplierAsync(id);
            return Json(new { success = true });
        }
    }
}
