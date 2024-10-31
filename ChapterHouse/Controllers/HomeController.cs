using ChapterHouse.Interface;
using ChapterHouse.Models;
using Microsoft.AspNetCore.Mvc;
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

    

     
    }
}
