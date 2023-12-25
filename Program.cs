
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using WebApplication1.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

//          new
builder.Services.AddControllers(); // for API   /////// This line adds support for controllers (API endpoints)
builder.Services.AddEndpointsApiExplorer(); ///   new

builder.Services.AddDbContext<DataContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}


app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

//          new 
app.MapControllers(); // for API /////// This line maps API endpoints
app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=index}/{id?}"
    );

app.Run();
