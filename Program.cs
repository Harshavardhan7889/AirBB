using Microsoft.EntityFrameworkCore;
using AirBB.Models.DataLayer;

var builder = WebApplication.CreateBuilder(args);

// Add session services
builder.Services.AddMemoryCache();
builder.Services.AddSession();


// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<ResidenceContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("ResidenceContext")));

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

app.UseSession();

app.UseAuthorization();

app.MapStaticAssets();

app.MapAreaControllerRoute(
    name: "admin",
    areaName: "Admin",
    pattern: "Admin/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
