using ChapterHouse.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace ChapterHouse.ApplicationDbContext
{
    public class AppDbContextion : IdentityDbContext<User>
    {
        public DbSet<Books> Books { get; set; }

        public AppDbContextion(DbContextOptions<AppDbContextion> options) : base(options) { }

        // Method to fetch and seed spooky books into the database using googleapis.com
        public async Task SeedBooksAsync()
        {
            using (var httpClient = new HttpClient())
            {
                // Example URL to fetch books; replace with a suitable API
                string url = "https://www.googleapis.com/books/v1/volumes?q=spooky+books";

                var response = await httpClient.GetStringAsync(url);

                // Deserialize JSON response to dynamic object
                var booksData = JsonSerializer.Deserialize<JsonElement>(response);

                // Fetch existing books from the database
                var existingBooks = await Books.ToListAsync();
                var existingTitles = existingBooks.Select(b => b.Title).ToHashSet();

                var bookList = new List<Books>();
                int count = 0; // Counter for unique books

                foreach (var item in booksData.GetProperty("items").EnumerateArray())
                {
                    if (count >= 20) break; // Stop if we reach the limit

                    var volumeInfo = item.GetProperty("volumeInfo");
                    var title = volumeInfo.GetProperty("title").GetString();

                    // Skip if the book is already in the database
                    if (existingTitles.Contains(title))
                    {
                        continue; // go to the next itema
                    }

                    var book = new Books
                    {
                        Title = title,
                        Author = volumeInfo.GetProperty("authors").EnumerateArray().FirstOrDefault().GetString(),
                        Description = volumeInfo.TryGetProperty("description", out var desc) ? desc.GetString() : "No description available",
                        ImageURL = volumeInfo.TryGetProperty("imageLinks", out var imageLinks) && imageLinks.TryGetProperty("thumbnail", out var thumbnail)
                                    ? thumbnail.GetString() : "No image available",
                        Genre = "Spooky", // Categorize as spooky genre
                        Cost = volumeInfo.TryGetProperty("cost", out var cost) ? cost.GetDecimal() : 0.0m, // Default cost to 0.0 if not available
                        Sale = volumeInfo.TryGetProperty("sale", out var sale) ? sale.GetDecimal() : 0.0m // Default sale to 0.0 if not available
                    };

                    bookList.Add(book);
                    count++; // Increment the unique book counter
                }

                // Add to database and save changes
                if (bookList.Any())
                {
                    await Books.AddRangeAsync(bookList);
                    await SaveChangesAsync();
                }
            }
        }

    }
}
