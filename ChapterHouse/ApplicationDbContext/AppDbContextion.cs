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
                string url = "https://www.googleapis.com/books/v1/volumes?q=spooky+books";
                var response = await httpClient.GetStringAsync(url);
                var booksData = JsonSerializer.Deserialize<JsonElement>(response);

                var existingBooks = await Books.ToListAsync();
                var existingTitles = existingBooks.Select(b => b.Title).ToHashSet();

                var bookList = new List<Books>();
                int count = 0;

                foreach (var item in booksData.GetProperty("items").EnumerateArray())
                {
                    if (count >= 20) break;

                    var volumeInfo = item.GetProperty("volumeInfo");
                    var title = volumeInfo.GetProperty("title").GetString();

                    if (existingTitles.Contains(title))
                    {
                        continue;
                    }

                    var book = new Books
                    {
                        Title = title,
                        Author = volumeInfo.TryGetProperty("authors", out var authors) && authors.ValueKind == JsonValueKind.Array && authors.GetArrayLength() > 0
                 ? authors[0].GetString()
                 : "Unknown Author",
                        Description = volumeInfo.TryGetProperty("description", out var desc) ? desc.GetString() : "No description available",
                        ImageURL = volumeInfo.TryGetProperty("imageLinks", out var imageLinks) && imageLinks.TryGetProperty("thumbnail", out var thumbnail)
                 ? thumbnail.GetString() : "No image available",
                        Genre = volumeInfo.TryGetProperty("categories", out var categories) && categories.ValueKind == JsonValueKind.Array && categories.GetArrayLength() > 0
                 ? categories[0].GetString()
                 : "Spooky",
                        PublishedDate = volumeInfo.TryGetProperty("publishedDate", out var publishedDate) ? publishedDate.GetString() : "Unknown",
                        Publisher = volumeInfo.TryGetProperty("publisher", out var publisher) ? publisher.GetString() : "Unknown",
                        PageCount = volumeInfo.TryGetProperty("pageCount", out var pageCount) && pageCount.ValueKind == JsonValueKind.Number
                 ? pageCount.GetInt32()
                 : 0,
                        Language = volumeInfo.TryGetProperty("language", out var language) ? language.GetString() : "Unknown",
                        PreviewLink = volumeInfo.TryGetProperty("previewLink", out var previewLink) ? previewLink.GetString() : "No preview available",
                        RatingsCount = volumeInfo.TryGetProperty("ratingsCount", out var ratingsCount) && ratingsCount.ValueKind == JsonValueKind.Number
                    ? ratingsCount.GetInt32()
                    : 0,
                        AverageRating = volumeInfo.TryGetProperty("averageRating", out var avgRating) && avgRating.ValueKind == JsonValueKind.Number
                     ? avgRating.GetDecimal()
                     : 0.0m
                    };


                    bookList.Add(book);
                    count++;
                }

                if (bookList.Any())
                {
                    await Books.AddRangeAsync(bookList);
                    await SaveChangesAsync();
                }
            }
        }


    }
}
