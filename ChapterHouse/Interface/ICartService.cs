using ChapterHouse.Models;

namespace ChapterHouse.Interface
{
    public interface ICartService
    {

        Task<List<Cart>> FetchCart();

        // determines the total cost of cart items 
        Task<decimal> CalculateTotal();

        Task DecreaseQuantity(int CartId);

        Task IncreaseQuantity(int CartId);

        Task<Cart> FetchCartById(int CartId);

        Task RemoveBookFromCart(int CartId);

        
        Task SendEmailAfterCheckout(string email);

        Task ClearCart();
        
    }
}
