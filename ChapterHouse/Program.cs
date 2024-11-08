using ChapterHouse.ApplicationDbContext;
using ChapterHouse.Interface;
using ChapterHouse.Models;
using ChapterHouse.Service;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<AppDbContextion>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("ChapterHouseConnectionString")));

builder.Services.AddIdentity<User, IdentityRole>(options =>
{
    // Set desired options
    options.Password.RequiredLength = 1;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireDigit = false;
    options.Password.RequireNonAlphanumeric = false;
})
.AddEntityFrameworkStores<AppDbContextion>()
.AddDefaultTokenProviders();

builder.Services.AddScoped<IBookService, BookService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ICartService, CartService>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");


var scope = app.Services.CreateScope();
var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContextion>();
// Seed books after building the app
await dbContext.Database.MigrateAsync();


if (!await dbContext.Books.AnyAsync())  
{
    
    await dbContext.SeedBooksAsync();
}

app.Run();
