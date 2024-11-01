namespace ChapterHouse.Models
{
    public class Cart
    {
        public int Id { get; set; } 
        public Books Book { get; set; } 
        public int BookId { get; set; } 
        public int Quantity { get; set; } // Quantity of the book in the cart
        public decimal Price { get; set; } // total price of the books in the cart


        public User User { get; set; }

        public string UserId { get; set; }
    }
}
