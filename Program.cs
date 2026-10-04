using Library_Automation.Data;
using Library_Automation.Models;
using LibraryAutomation.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<ApplicationDbContext>(options=>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
//kullanıcı tipi ApplicationUser olarak ayarlanır. IdentityRole ise roller için kullanılır.
builder.Services.AddIdentity<ApplicationUser,IdentityRole>(options =>
{
    options.Password.RequiredLength=6;
    options.Password.RequireDigit=true;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric=false;
    options.User.RequireUniqueEmail = true;//password kuralları
})
.AddEntityFrameworkStores<ApplicationDbContext>()//kullanıcı ve rol bilgilerini veritabanında saklamak ApplicationDbContext kullanır
.AddDefaultTokenProviders();
//giriş ve erişim reddedildiğinde yönlendirilecek sayfalar ayarlanır
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";//Giriş yapmamış biri korumalı sayfaya girmeye çalışınca nereye yönlendirileceği
    options.AccessDeniedPath = "/Account/AccessDenied";//giriş yapmış ama yetkisi olmayanın nereye gideceği
});


var app = builder.Build();
using (var scope = app.Services.CreateScope())
{
    await DbSeeder.SeedAsync(scope.ServiceProvider);
}
app.UseAuthentication();//kimlik doğrulama işlemlerini etkinleştirir
app.UseAuthorization();//yetkilendirme işlemlerini etkinleştirir

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
