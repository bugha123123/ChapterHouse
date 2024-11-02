using ChapterHouse.ApplicationDbContext;
using ChapterHouse.Interface;
using ChapterHouse.Models;
using Microsoft.EntityFrameworkCore;
using System.Net.Mail;
using System.Net;
using System.Text;

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

            var Cart = await _appDbContextion.Carts.Where(x => x.UserId == LoggedInUser.Id).Include(x => x.Book).ToListAsync();

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

        public async Task SendEmailAfterCheckout(string email)
        {
            var FetchedCart = await FetchCart();

        

            

          

            // clear cart
            await ClearCart();

            var htmlBody = new StringBuilder();

            htmlBody.AppendLine("<html><body>");
            htmlBody.AppendLine("<h1>Thank you for your purchase!</h1>");
            htmlBody.AppendLine($"<p>Dear {email},</p>");
            htmlBody.AppendLine("<p>Your payment has been successfully processed. Here are the details of your order:</p>");
            htmlBody.AppendLine("<table style='width:100%; border-collapse: collapse;'>");
            htmlBody.AppendLine("<tr><th style='border: 1px solid #ddd; padding: 8px;'>Book Title</th><th style='border: 1px solid #ddd; padding: 8px;'>Author</th><th style='border: 1px solid #ddd; padding: 8px;'>Price</th><th style='border: 1px solid #ddd; padding: 8px;'>Quantity</th></tr>");

            foreach (var fetchedItems in FetchedCart)
            {
                var book = fetchedItems.Book;
                int quantity = fetchedItems.Quantity;

                htmlBody.AppendLine("<tr>");
                htmlBody.AppendLine($"<td style='border: 1px solid #ddd; padding: 8px;'>{book.Title}</td>");
                htmlBody.AppendLine($"<td style='border: 1px solid #ddd; padding: 8px;'>{book.Author}</td>");
                htmlBody.AppendLine($"<td style='border: 1px solid #ddd; padding: 8px;'>${20 * quantity}</td>");
                htmlBody.AppendLine($"<td style='border: 1px solid #ddd; padding: 8px;'>{quantity}</td>");
                htmlBody.AppendLine("</tr>");
            }

            htmlBody.AppendLine("</table>");
            htmlBody.AppendLine("<p>Thank you for shopping with us!</p>");
            htmlBody.AppendLine("</body></html>");

            // Send the email
            using (var client = new SmtpClient("smtp.gmail.com", 587))
            {
                client.Host = "smtp.gmail.com";
                client.Port = 587;
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.UseDefaultCredentials = false;
                client.EnableSsl = true;
                client.Credentials = new NetworkCredential("irakliberdzena314@gmail.com", "coca mmba ywsy lvyz");

                using (var message = new MailMessage())
                {
                    message.From = new MailAddress("irakliberdzena314@gmail.com", "Spooky Bookstore");
                    message.To.Add(new MailAddress(email));
                    message.Subject = "Payment Receipt - Spooky Bookstore";
                    message.IsBodyHtml = true;
                    message.Body = htmlBody.ToString();

                    // Send the email
                    await client.SendMailAsync(message);
                }
            }
        }



        public async Task ClearCart()
        {
            var cartItems = await _appDbContextion.Carts.ToListAsync();

            _appDbContextion.Carts.RemoveRange(cartItems);

            await _appDbContextion.SaveChangesAsync();
        }

    }
}
