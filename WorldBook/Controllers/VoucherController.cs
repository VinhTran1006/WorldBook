using Microsoft.AspNetCore.Mvc;
using WorldBook.Models;
using WorldBook.Services.Interfaces;
using WorldBook.ViewModel;

namespace WorldBook.Controllers
{
    public class VoucherController : Controller
    {
        private readonly IVoucherService _voucherService;

        public VoucherController(IVoucherService voucherService)
        {
            _voucherService = voucherService;
        }

        // GET: Voucher
        public async Task<IActionResult> Index()
        {
            var vouchers = await _voucherService.GetAllVouchersAsync();
            return View("~/Views/AdminViews/ManageVoucher/Index.cshtml", vouchers);
        }

        // GET: Voucher/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var voucher = await _voucherService.GetVoucherByIdAsync(id.Value);
            if (voucher == null)
            {
                return NotFound();
            }

            return View("~/Views/AdminViews/ManageVoucher/Details.cshtml", voucher);
        }

        // GET: Voucher/Create
        public IActionResult Create()
        {
            return View("~/Views/AdminViews/ManageVoucher/Create.cshtml");
        }

        // POST: Voucher/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(VoucherViewModel model)
        {
            if (ModelState.IsValid)
            {
                var voucher = new Voucher
                {
                    VoucherCode = model.VoucherCode,
                    DiscountPercent = model.DiscountPercent,
                    ExpriryDate = model.ExpriryDate,
                    MinOrderAmount = model.MinOrderAmount,
                    MaxOrderAmount = model.MaxOrderAmount,
                    UsageCount = model.UsageCount ?? 0,
                    IsActive = model.IsActive,
                    VoucherDescription = model.VoucherDescription
                };

                await _voucherService.CreateVoucherAsync(voucher);
                TempData["SuccessMessage"] = "Voucher created successfully!";
                return RedirectToAction(nameof(Index));
            }
            return View("~/Views/AdminViews/ManageVoucher/Create.cshtml", model);
        }

        // GET: Voucher/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var voucher = await _voucherService.GetVoucherByIdAsync(id.Value);
            if (voucher == null)
            {
                return NotFound();
            }

            var model = new VoucherViewModel
            {
                VoucherId = voucher.VoucherId,
                VoucherCode = voucher.VoucherCode ?? string.Empty,
                DiscountPercent = voucher.DiscountPercent ?? 0,
                ExpriryDate = voucher.ExpriryDate ?? DateTime.Now,
                MinOrderAmount = voucher.MinOrderAmount,
                MaxOrderAmount = voucher.MaxOrderAmount,
                UsageCount = voucher.UsageCount,
                IsActive = voucher.IsActive ?? false,
                VoucherDescription = voucher.VoucherDescription
            };

            return View("~/Views/AdminViews/ManageVoucher/Edit.cshtml", model);
        }

        // POST: Voucher/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, VoucherViewModel model)
        {
            if (id != model.VoucherId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var voucher = new Voucher
                    {
                        VoucherId = model.VoucherId,
                        VoucherCode = model.VoucherCode,
                        DiscountPercent = model.DiscountPercent,
                        ExpriryDate = model.ExpriryDate,
                        MinOrderAmount = model.MinOrderAmount,
                        MaxOrderAmount = model.MaxOrderAmount,
                        UsageCount = model.UsageCount,
                        IsActive = model.IsActive,
                        VoucherDescription = model.VoucherDescription
                    };

                    await _voucherService.UpdateVoucherAsync(voucher);
                    TempData["SuccessMessage"] = "Voucher updated successfully!";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception)
                {
                    if (!await _voucherService.VoucherExistsAsync(model.VoucherId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            return View("~/Views/AdminViews/ManageVoucher/Edit.cshtml", model);
        }

        // GET: Voucher/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var voucher = await _voucherService.GetVoucherByIdAsync(id.Value);
            if (voucher == null)
            {
                return NotFound();
            }

            return View("~/Views/AdminViews/ManageVoucher/Delete.cshtml", voucher);
        }

        // POST: Voucher/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var result = await _voucherService.DeleteVoucherAsync(id);
            if (result)
            {
                TempData["SuccessMessage"] = "Voucher deleted successfully!";
            }
            else
            {
                TempData["ErrorMessage"] = "Could not delete voucher!";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}