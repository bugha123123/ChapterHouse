using ChapterHouse.Models;

namespace ChapterHouse.Interface
{
    public interface IBookService
    {
        Task<List<Books>> FetchSpookyBooks();


        Task AddBookToCart(int bookId);

        Task<Books> GetBookById(int BookId);

        Task<List<Books>> FetchNewArrivals();

        Task<Books> FetchBookById(int BookId);

       

        Task<List<Books>> GetBooksByCategory(string category);

        Task AddBookToWishList(int BookId);

        Task<List<WishList>> GetAllWishList(); 

        Task<WishList> GetWishListedBookById(int BookId);

        Task RemoveBookFromWishList(int BookId);

        Task<List<Books>> GetBooksWithHighestPageCount();

        Task<List<Books>> GetPopularPublishersBooks();

            Task<List<Books>> GetPublishersDetails( string publisherName);
    }
}
