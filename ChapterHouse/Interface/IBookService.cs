using ChapterHouse.Models;

namespace ChapterHouse.Interface
{
    public interface IBookService
    {
        Task<List<Books>> FetchSpookyBooks();


        Task AddBookToCart(int bookId);

        Task<Books> GetBookById(int BookId);

        Task<List<Books>> FetchNewArrivals();

      
    }
}
