using ChapterHouse.ApplicationDbContext;
using ChapterHouse.Interface;
using ChapterHouse.Models;
using Microsoft.EntityFrameworkCore;

namespace ChapterHouse.Service
{
    public class CartService : ICartService
    {
        private readonly AppDbContextion _appDbContextion;
        private readonly IAuthService _authService;

        public CartService(IAuthService authService, AppDbContextion appDbContextion)
        {
            _authService = authService;
            _appDbContextion = appDbContextion;
        }

        public async Task<List<Cart>> FetchCart()
        {

            var LoggedInUser = await _authService.GetLoggedInUserAsync();

            if (LoggedInUser == null)
            {
                throw new Exception("user not found");
            }

            var Cart = await _appDbContextion.Carts.Where(x => x.UserId == LoggedInUser.Id).Include(x => x.Book).Include(x => x.User).ToListAsync();

            if (Cart.Count == 0)
            {
                return new List<Cart>();
            }
            return Cart;
        }


        // calculates total cost of cart items
        public async Task<decimal> CalculateTotal()
        {
            var CartItems = await FetchCart();

            // Calculate the total cost
            decimal totalCost = 0;
            foreach (var item in CartItems)
            {
                totalCost += 20 * item.Quantity;
            }

            return totalCost;
        }


        // increases quantity of cart items 
        public async Task DecreaseQuantity(int CartId)
        {
            var fetchedCartById = await FetchCartById(CartId);

            // Check if the fetched cart item exists
            if (fetchedCartById == null)
            {
                throw new Exception("Cart item not found");
            }

            // If quantity is greater than 1, just decrease the quantity
            if (fetchedCartById.Quantity > 1)
            {
                fetchedCartById.Quantity -= 1;
            }
            else
            {
                // If quantity is 1, remove the item from the cart
                await RemoveBookFromCart(fetchedCartById.Id);
            }

            // Save changes to the database
            await _appDbContextion.SaveChangesAsync();
        }


        public async Task IncreaseQuantity(int CartId)
        {
            var FetchedCartById = await FetchCartById(CartId);


            FetchedCartById.Quantity += 1;
            await _appDbContextion.SaveChangesAsync();
        }

        public async Task<Cart> FetchCartById(int CartId)
        {
            var CartById = await _appDbContextion.Carts.FirstOrDefaultAsync(x => x.Id == CartId);

            if (CartById == null) return new Cart();


            return CartById;
        }

        public async Task RemoveBookFromCart(int CartId)
        {
            var FetchedCartById = await FetchCartById(CartId);

             _appDbContextion.Carts.Remove(FetchedCartById);
            await _appDbContextion.SaveChangesAsync();

        }
    }
}
