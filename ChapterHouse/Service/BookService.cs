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

    public BookService(AppDbContextion appDbContextion)
    {
        _appDbContextion = appDbContextion;
    }

    public async Task<List<Books>> FetchSpookyBooks()
    {
        if (_appDbContextion.Books == null) throw new Exception("Books not available ");
        
        return await _appDbContextion.Books.ToListAsync();
    }
}
