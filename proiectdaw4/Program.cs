using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using proiectdaw4.Data;
using proiectdaw4.Services;

internal class Program
{
    
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        string bd = "Server=DESKTOP-AQMKK7O;Database=ProprietatiDB;Trusted_Connection=True;TrustServerCertificate=True;";

        builder.Services.AddDbContext<BdContext>(options => options.UseSqlServer(bd));

        builder.Services.AddControllersWithViews();

        builder.Services.AddRazorPages();

        builder.Services.AddScoped<AuthenticationService>();
        builder.Services.AddScoped<ValidationService>();
        builder.Services.AddScoped<CreateAccountService>();


        builder.Services.AddSession(options =>
        {
            options.Cookie.HttpOnly = true;
            options.Cookie.IsEssential = true;
        });   

        builder.Services.AddDistributedMemoryCache();

        var app = builder.Build();

        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Home/Error");
            app.UseHsts();
        }
        app.UseRouting();

        app.UseHttpsRedirection();
        app.UseStaticFiles();
        app.UseSession();


        app.UseAuthorization();

        app.MapControllers();

        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}");

        app.Run();
    }
}