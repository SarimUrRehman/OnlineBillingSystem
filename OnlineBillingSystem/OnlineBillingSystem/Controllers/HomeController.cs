using Microsoft.AspNetCore.Mvc;
using OnlineBillingSystem.Models;
using OnlineBillingSystem.ViewModels;
using System.Diagnostics;

namespace OnlineBillingSystem.Controllers
{
    public class HomeController : Controller
    {
        private readonly OnlineBillingSystemContext _context;

        public HomeController(OnlineBillingSystemContext context)
        {
            _context = context;
        }


        public IActionResult Index()
        {
            //var itemlist = _context.ItemBills.ToList();
            //      return View(itemlist); 
            var allItems = _context.ItemBills.ToList();

            // Filter only "Not paid" items
            var unpaidItems = allItems.Where(item => item.Ordered != "ispaid").ToList();

            // Calculate total balance for unpaid items
            ViewBag.TotalBalance = unpaidItems.Sum(item => (item.UnitPrice * item.Quantity) - item.Discount);

            // Send all items to view
            return View(allItems);
        }

       
       

        [HttpGet]
        public IActionResult Items()
        {
            var viewModel = new CombinedViewModel
            {
                ItemBill = new ItemBill(),
                CustomerDetail = new CustomerDetail()
            };

            return View(viewModel);
        }

        
     

        [HttpPost]
        public IActionResult Items(CombinedViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                // Check if CustomerDetail is already in the database
                var existingCustomer = _context.CustomerDetails
                                                .FirstOrDefault(c => c.CardNumber == viewModel.CustomerDetail.CardNumber);

                if (existingCustomer == null) // Customer is not in the database
                {
                    // Save CustomerDetail to the database
                    _context.CustomerDetails.Add(viewModel.CustomerDetail);
                    _context.SaveChanges(); // Save to get CustomerId
                }
                else
                {
                    viewModel.CustomerDetail = existingCustomer;
                }

                // Calculate Total for ItemBill
                viewModel.ItemBill.Total = (viewModel.ItemBill.UnitPrice * viewModel.ItemBill.Quantity) - viewModel.ItemBill.Discount;

                // Link the ItemBill to the CustomerDetail
                viewModel.ItemBill.CustomerId = viewModel.CustomerDetail.CustomerId;

                // Save ItemBill to the database
                _context.ItemBills.Add(viewModel.ItemBill);
                _context.SaveChanges();

                // Calculate FinalTotal for CustomerDetail based on all items for this customer
                var itemTotal = _context.ItemBills
                                        .Where(i => i.CustomerId == viewModel.CustomerDetail.CustomerId)
                                        .Sum(i => i.Total);

                // Update FinalTotal
                viewModel.CustomerDetail.FinalTotal = itemTotal;

                // Update CustomerDetail in the database
                _context.CustomerDetails.Update(viewModel.CustomerDetail);
                _context.SaveChanges();

                // Clear only the ItemBill form data
                ModelState.Clear();
                viewModel.ItemBill = new ItemBill(); // Reset ItemBill data

                return View(viewModel); // Keep customer data intact and reset item data
            }

            // If there is an issue, reload the same view
            return View(viewModel);
        }

       [HttpGet]
    public IActionResult DeleteRecord(int id)
    {
    var deleteData = _context.ItemBills.FirstOrDefault(x => x.ItemId == id);

    if (deleteData == null)
    {
        return NotFound();
    }

    return View(deleteData);  
}

        [HttpPost, ActionName("DeleteRecord")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            Console.WriteLine($"Delete ID: {id}"); // Ye line add karein taake aap dekh saken ke ID aayi hai ya nahi
            var deleteData = _context.ItemBills.FirstOrDefault(x => x.ItemId == id);
            if (deleteData != null)
            {
                _context.ItemBills.Remove(deleteData);
                _context.SaveChanges();
            }
            else
            {
                Console.WriteLine("Item not found!"); // Ye line bhi helpful hogi agar item nahi mil raha
            }

            return RedirectToAction("Index");
        }




        [HttpPost]
        [Route("OrderedAll")]
        public IActionResult Ordered()
        {
            // Saare items retrieve karna
            var items = _context.ItemBills.ToList();

            // Har item ka status 'ispaid' karna
            foreach (var item in items)
            {
                item.Ordered = "ispaid";
            }

            // Changes ko save karna
            _context.SaveChanges();

            // Index page par redirect karna
            return RedirectToAction("Index");
        }

    }
}
