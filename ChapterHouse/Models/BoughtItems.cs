namespace ChapterHouse.Models
{
    public class BoughtItems
    {
        public int Id { get; set; }


        public List<Books> Books { get; set; }

        public int BookId { get; set; }

        public int Quantity { get; set; }
    }
}
