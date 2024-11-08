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
        public DbSet<Cart> Carts { get; set; }

        public DbSet<WishList> WishList { get; set; }


        public AppDbContextion(DbContextOptions<AppDbContextion> options) : base(options) { }

        // Method to fetch and seed spooky books into the database using googleapis.com
        public async Task SeedBooksAsync()
        {
            using (var httpClient = new HttpClient())
            {
                // List to hold all the book queries we need to make.
                var genres = new List<string> { "Spooky", "Fiction", "Witch", "Slasher", "Paranormal fiction" };
                var genreBooks = new Dictionary<string, List<Books>>();

                // Fetch books from each genre
                foreach (var genre in genres)
                {
                    string url = $"https://www.googleapis.com/books/v1/volumes?q={genre}+books&maxResults=10"; // Max 10 books per genre
                    var response = await httpClient.GetStringAsync(url);
                    var booksData = JsonSerializer.Deserialize<JsonElement>(response);

                    var genreBookList = new List<Books>();

                    // Fetch all existing books from the database
                    var existingBooks = await Books.ToListAsync();
                    var existingTitles = existingBooks.Select(b => b.Title).ToHashSet();

                    // Iterate over each item from the API response
                    foreach (var item in booksData.GetProperty("items").EnumerateArray())
                    {
                        var volumeInfo = item.GetProperty("volumeInfo");
                        var title = volumeInfo.GetProperty("title").GetString();

                        // Skip adding the book if it already exists in the database
                        if (existingTitles.Contains(title))
                        {
                            continue;
                        }

                        // Create a new book object with details from the API
                        var book = new Books
                        {
                            Title = title,
                            Author = volumeInfo.TryGetProperty("authors", out var authors) && authors.ValueKind == JsonValueKind.Array && authors.GetArrayLength() > 0
                                ? authors[0].GetString()
                                : "Unknown Author",
                            Description = volumeInfo.TryGetProperty("description", out var desc) ? desc.GetString() : "No description available",
                            ImageURL = volumeInfo.TryGetProperty("imageLinks", out var imageLinks) && imageLinks.TryGetProperty("thumbnail", out var thumbnail)
                                ? thumbnail.GetString()
                                : "No image available",
                            Genre = genre, // Store genre
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

                        genreBookList.Add(book);
                    }

                    // Store the fetched books in the dictionary for later use
                    genreBooks[genre] = genreBookList;
                }

                // Now select specific number of books from each genre and add to the database
                var booksToAdd = new List<Books>();

                // Fetch 3 spooky books
                booksToAdd.AddRange(genreBooks["Spooky"].Take(10));

                // Fetch 2 fiction books
                booksToAdd.AddRange(genreBooks["Fiction"].Take(10));

                // Fetch 10 witch-themed books
                booksToAdd.AddRange(genreBooks["Witch"].Take(10));

                booksToAdd.AddRange(genreBooks["Slasher"].Take(10));

                booksToAdd.AddRange(genreBooks["Paranormal fiction"].Take(10));

                // Add new books to the database if there are any
                if (booksToAdd.Any())
                {
                    await Books.AddRangeAsync(booksToAdd);
                    await SaveChangesAsync();
                }
            }
        }


    }


}

