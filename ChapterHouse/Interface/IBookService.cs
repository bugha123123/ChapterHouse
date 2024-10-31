using ChapterHouse.Models;

namespace ChapterHouse.Interface
{
    public interface IBookService
    {
        Task<List<Books>> FetchSpookyBooks();
    }
}
