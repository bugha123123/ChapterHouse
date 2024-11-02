using ChapterHouse.ApplicationDbContext;
using ChapterHouse.Interface;
using ChapterHouse.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

public class BookService : IBookService


{

    private readonly AppDbContextion _appDbContextion;
    private readonly IAuthService _authService;
    public BookService(AppDbContextion appDbContextion, IAuthService authService)
    {
        _appDbContextion = appDbContextion;
        _authService = authService;
    }


    public async Task<List<Books>> FetchSpookyBooks()
    {
        if (_appDbContextion.Books == null) throw new Exception("Books not available ");
        
        return await _appDbContextion.Books.ToListAsync();
    }

    public async Task AddBookToCart(int bookId)
    {
        var FoundBook = await GetBookById(bookId);
        var LoggedInUser = await _authService.GetLoggedInUserAsync();

        if (FoundBook == null)
        {
            throw new Exception("Book not found by id");
        }

        if (LoggedInUser == null)
        {
            throw new Exception("user not found");
        }

        var existingCart =  await _appDbContextion.Carts.FirstOrDefaultAsync(x => x.BookId == FoundBook.Id && x.UserId == LoggedInUser.Id);

        if (existingCart != null)
        {
            existingCart.Quantity += 1;

                _appDbContextion.Carts.Update(existingCart);
           await   _appDbContextion.SaveChangesAsync();
        }
        else 
        {
            var BookToAdd = new Cart()
            {
                BookId = FoundBook.Id,
                Price = 20,
                Book = FoundBook,
               Quantity = 1,
               User = LoggedInUser,
               UserId = LoggedInUser.Id



            };
           

            await _appDbContextion.Carts.AddAsync(BookToAdd);
            await _appDbContextion.SaveChangesAsync();
        }
      
    }

    public async Task<Books> GetBookById(int BookId)
    {
        var FoundBookById =  await _appDbContextion.Books.FirstOrDefaultAsync(b => b.Id == BookId);
        if (FoundBookById == null)
            throw new Exception("Book not found");

        return FoundBookById;
    }
    public async Task<List<Books>> FetchNewArrivals()
    {
        var allBooks = await _appDbContextion.Books.ToListAsync();

        var newArrivals = allBooks
            .Where(x => DateTime.TryParse(x.PublishedDate, out var publishedDate) && publishedDate > new DateTime(2016, 1, 1))
            .OrderByDescending(b => DateTime.Parse(b.PublishedDate)) 
            .ToList();

        return newArrivals;
    }

    public async Task<Books> FetchBookById(int BookId)
    {
        var FoundBook = await _appDbContextion.Books.FirstOrDefaultAsync(x => x.Id == BookId);

        return FoundBook;
    }
}
