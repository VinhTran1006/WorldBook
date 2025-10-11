using Microsoft.EntityFrameworkCore;
using WorldBook.Models;
using WorldBook.Repositories;
using WorldBook.Repositories.Interfaces;
using WorldBook.Services;
using WorldBook.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<WorldBookDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

//Regist Repo vs Service
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IBookRepository, BookRepository>();

builder.Services.AddScoped<IRoleRepository, RoleRepository>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IBookService, BookService>();


//Cookie schema
builder.Services.AddAuthentication("MyCookieAuth")
    .AddCookie("MyCookieAuth", options =>
    {
        options.LoginPath = "/Logins/Login";      // Trang login khi chưa đăng nhập
        //options.LogoutPath = "/Account/Logout";    // Trang logout
        //options.AccessDeniedPath = "/Account/Denied"; // Trang khi không đủ quyền
        options.Cookie.Name = "MyCookie";      // Tên cookie
        options.ExpireTimeSpan = TimeSpan.FromMinutes(60); // Thời gian hết hạn
        options.SlidingExpiration = true;          // Tự gia hạn nếu user hoạt động
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}


app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Book}/{action=GetBookHomePage}/{id?}");

app.Run();
