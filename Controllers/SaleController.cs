using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ERPSalesSystem.Models;

namespace ERPSalesSystem.Controllers
{
    public class SaleController : Controller
    {
        private readonly ERPContext _context;

        public SaleController(ERPContext context)
        {
            _context = context;
        }

        // GET: Sale
        public async Task<IActionResult> Index()
        {
            var eRPContext = _context.Sales.Include(s => s.Product);
            return View(await eRPContext.ToListAsync());
        }

        // GET: Sale/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sale = await _context.Sales
                .Include(s => s.Product)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (sale == null)
            {
                return NotFound();
            }

            return View(sale);
        }

        // GET: Sale/Create
        public IActionResult Create()
        {
            ViewData["ProductId"] = new SelectList(_context.Products.Where(p=>p.Stock>0), "Id", "Name");
            return View();
        }

        // POST: Sale/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ProductId,Quantity,Date")] Sale sale)
        {
            if (ModelState.IsValid)
            {
                // Pronađi proizvod koji je prodat
                var product = await _context.Products.FindAsync(sale.ProductId);

                if (product == null)
                {
                    return NotFound("Product not found");
                }

                // Smanji količinu proizvoda
                product.Stock -= sale.Quantity;

                // Proveri da li je količina validna
                if (product.Stock < 0)
                {
                    ModelState.AddModelError("", "Not enough stock available");
                    ViewData["ProductId"] = new SelectList(_context.Products.Where(p => p.Stock > 0), "Id", "Name", sale.ProductId);
                    return View(sale);
                }

                _context.Add(sale);
                _context.Update(product);
                await _context.SaveChangesAsync();

                // Load the Product navigation property
                await _context.Entry(sale).Reference(s => s.Product).LoadAsync();

                return RedirectToAction(nameof(Index));


            }

            // Log validation errors
            var errors = ModelState.Values.SelectMany(v => v.Errors);
            foreach (var error in errors)
            {
                Console.WriteLine("Ispisi    " + error.ErrorMessage);
            }

            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Name", sale.ProductId);
            return View(sale);
        }

        // GET: Sale/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sale = await _context.Sales.FindAsync(id);
            if (sale == null)
            {
                return NotFound();
            }
            ViewData["ProductId"] = new SelectList(_context.Products.Where(p=> p.Stock>0 || p.Id == sale.ProductId), "Id", "Name", sale.ProductId);
            return View(sale);
        }

        // POST: Sale/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ProductId,Quantity,Date")] Sale sale)
        {

            var product = await _context.Products.FindAsync(sale.ProductId);

            if (id != sale.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    

                    if (product.Stock < sale.Quantity)
                    {
                        ModelState.AddModelError("", "Not enough stock available. Stock: " + product.Stock);
                        ViewData["ProductId"] = new SelectList(_context.Products.Where(p => p.Stock > 0 || p.Id == sale.ProductId), "Id", "Name", sale.ProductId);
                        return View(sale);
                    }
                    product.Stock -= sale.Quantity;
                    _context.Update(sale);
                    await _context.SaveChangesAsync();

                }
                
                catch (DbUpdateConcurrencyException)
                {
                    if (!SaleExists(sale.Id))
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
            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Name", sale.ProductId);
            return View(sale);
        }

        // GET: Sale/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sale = await _context.Sales
                .Include(s => s.Product)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (sale == null)
            {
                return NotFound();
            }

            return View(sale);
        }

        // POST: Sale/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var sale = await _context.Sales.FindAsync(id);
            if (sale != null)
            {
                _context.Sales.Remove(sale);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SaleExists(int id)
        {
            return _context.Sales.Any(e => e.Id == id);
        }

        public async Task<IActionResult> Report()
        {
            var sales = await _context.Sales.Include(s => s.Product).ToListAsync();
            return View(sales);
        }
    }
}
