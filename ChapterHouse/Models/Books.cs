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
        public decimal Cost { get; set; } // Assuming cost is a decimal
        public decimal Sale { get; set; } // Assuming sale is a decimal
    }
}
