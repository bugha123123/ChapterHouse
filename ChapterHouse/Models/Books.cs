namespace ChapterHouse.Models
{
    public class Books
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public string Description { get; set; }
        public string ImageURL { get; set; }
        public string Genre { get; set; }

        // Updated properties for additional details
        public string PublishedDate { get; set; }      // Publication date of the book
        public string Publisher { get; set; }          // Publisher's name
        public int PageCount { get; set; }             // Number of pages in the book
        public string Language { get; set; }           // Language code, e.g., "en"
        public string PreviewLink { get; set; }        // Link to preview the book online
        public int RatingsCount { get; set; }          // Number of user ratings
        public decimal AverageRating { get; set; }

    }
}
