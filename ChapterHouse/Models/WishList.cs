namespace ChapterHouse.Models
{
    public class WishList
    {

        public int Id { get; set; }

        public Books books { get; set; }

        public int BookId { get; set; }

        public User User { get; set; }

        public string UserId { get; set; }
    }
}
