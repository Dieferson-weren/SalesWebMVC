using Microsoft.AspNetCore.Mvc;
using SalesWebMVC.Models;
using SalesWebMVC.Models.ViewModels;
using SalesWebMVC.Services;
using SalesWebMVC.Services.Exceptions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace SalesWebMVC.Controllers
{
    public class SellersController : Controller
    {
        private readonly SellerService _sellerService;
        private readonly DepartmentService _departmentService;

        public SellersController(SellerService sellerService, DepartmentService departmentService)
        {
            _sellerService = sellerService;
            _departmentService = departmentService;
        }
        public async Task<IActionResult> Index()
        {
            var list =await _sellerService.FindAllAsync();
            return View(list);
        }

        public async Task<IActionResult> Create()
        {
            var department = await _departmentService.FindAllAsync();
            var viewSeller = new SellerFormViewModel { Departments = department };
            return View(viewSeller);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Seller seller)
        {
            await _sellerService.InsertAsync(seller);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if(id == null)
            {
                return RedirectToAction(nameof(Error), new { messages = "Id not provided" });
            }

            var obj = await _sellerService.FindByIDAsync(id.Value);

            if(obj == null)
            {
                return RedirectToAction(nameof(Error), new { messages = "Id not found" });
            }

            return View(obj);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _sellerService.RemoveAsync(id);
                return RedirectToAction(nameof(Index));
            }
            catch (IntegrityException e)
            {
                return RedirectToAction(nameof(Error), new { messages = e.Message });
            }
            {

            }
           
        }

        public async Task<IActionResult> Details(int? id)
        {

            if(id == null)
            {
                return RedirectToAction(nameof(Error), new { messages = "Id not provided" });
            }
            var obj = await _sellerService.FindByIDAsync(id.Value);

            if(obj == null)
            {
                return RedirectToAction(nameof(Error), new { messages = "Id not found" });
            }
            return View(obj);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if(id == null)
            {
                return RedirectToAction(nameof(Error), new { messages = "Id not provided" });
            }

            Seller obj = await _sellerService.FindByIDAsync(id.Value);

            if(obj == null)
            {
                return RedirectToAction(nameof(Error), new { messages = "Id not found" });
            }

            List<Department> departments = await _departmentService.FindAllAsync();
            SellerFormViewModel viewModel = new SellerFormViewModel { Departments = departments, Seller = obj };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Seller seller)
        {
            if(id != seller.Id)
            {
                return RedirectToAction(nameof(Error), new { messages = "Id mismatch" });
            }

            try
            {
                await _sellerService.UpdateAsync(seller);
                return RedirectToAction(nameof(Index));
            }catch (ApplicationException e)
            {
                return RedirectToAction(nameof(Error), new { messages = e.Message});
            }
        }

        public IActionResult Error(string messages)
        {
            var viewModel = new ErrorViewModel
            {
                Message = messages,
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
            };

            return View(viewModel);
        }
    }
}
