using ChapterHouse.Interface;
using ChapterHouse.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Diagnostics;

namespace ChapterHouse.Controllers
{
    public class HomeController : Controller
    {
       private readonly ICartService _CartService;
        private readonly IBookService _bookservice;

        public HomeController(ICartService cartService, IBookService bookservice)
        {
            _CartService = cartService;
            _bookservice = bookservice;
        }

        public async Task<IActionResult> Index()
        {
            var FetchedBooks = await _bookservice.FetchSpookyBooks();
            return View(FetchedBooks);
        }
        public async Task<IActionResult> newarrivals()
        {
            var NewArrivals = await _bookservice.FetchNewArrivals();
            return View(NewArrivals);
        }

        public async Task<IActionResult> Details(int BookId)
        {
            var FetchedBookById = await _bookservice.FetchBookById(BookId);
            return View(FetchedBookById);
        }


        public async Task<IActionResult> checkout()
        {
            var FetchedCart = await _CartService.FetchCart();
            return View(FetchedCart);
        }
        public async Task<IActionResult> Cart()
        {
            var FetchedCart = await _CartService.FetchCart();

            if (FetchedCart == null || !FetchedCart.Any())
            {
                TempData["CartEmpty"] = "Your cart is empty!";
                return View(new List<Cart>());
            }

            return View(FetchedCart);
        }


        public async Task<IActionResult> AddBookToCart(int BookId)
        {
            if (ModelState.IsValid)
            {
                await _bookservice.AddBookToCart(BookId);
                return RedirectToAction("Cart", "Home");
            }

            return View("Index", "Home"  );
        }

        public async Task<IActionResult> IncreaseQuantity(int CartId)
        {
            if (ModelState.IsValid)
            {
                await _CartService.IncreaseQuantity(CartId);
                return RedirectToAction("Cart", "Home");

            }

            return View("Cart", "Home");
        }

        public async Task<IActionResult> DecreaseQuantity(int CartId)
        {
            if (ModelState.IsValid)
            {
                await _CartService.DecreaseQuantity(CartId);
                return RedirectToAction("Cart", "Home");

            }

            return View("Cart", "Home");
        }

        public async Task<IActionResult> RemoveCart(int CartId)
        {
            if (ModelState.IsValid)
            {
                await _CartService.RemoveBookFromCart(CartId);
                return RedirectToAction("Cart", "Home");

            }

            return View("Cart", "Home");
        }

        public async Task<IActionResult> BuyItems(string email)
        {
            await _CartService.SendEmailAfterCheckout(email);
            return RedirectToAction("Index", "Home");
        }
    }
}
