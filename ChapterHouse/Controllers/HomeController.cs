using ChapterHouse.Interface;
using ChapterHouse.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Diagnostics;

namespace ChapterHouse.Controllers
{
    public class HomeController : Controller
    {
       private readonly IBookService _bookService;

        public HomeController(IBookService bookService)
        {
            _bookService = bookService;
        }

        public async Task<IActionResult> Index()
        {
            var FetchedBooks = await _bookService.FetchSpookyBooks();
            return View(FetchedBooks);
        }


        public async Task<IActionResult> Cart()
        {
            return View();
        }

        public async Task<IActionResult> AddBookToCart(int BookId)
        {
            if (ModelState.IsValid)
            {
                await _bookService.AddBookToCart(BookId);
                return RedirectToAction("Cart", "Home");
            }

            return View("Index", "Home"  );
        }
    }
}
