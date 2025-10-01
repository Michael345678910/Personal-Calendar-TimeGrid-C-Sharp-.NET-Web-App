using DotNetCoreCalendar.Data;
using DotNetCoreCalendar.Models; // for ApplicationUser
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Linq;

// Test Email: akapic1@gmail.com
// Password for test email: Dev!Passw0rd123

var builder = WebApplication.CreateBuilder(args);

// --------------------
// Configure Services
// --------------------
builder.Services.AddControllersWithViews();

// Use your ApplicationDbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
           .UseLazyLoadingProxies());

// Identity with your ApplicationUser class
builder.Services.AddDefaultIdentity<ApplicationUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = true;
})
    .AddEntityFrameworkStores<ApplicationDbContext>();



// Razor runtime compilation (optional, but was in your old setup)
builder.Services.AddRazorPages();

builder.Services.AddScoped<IDAL, DAL>();

var app = builder.Build();

// --------------------
// Configure Middleware
// --------------------
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

// --------------------
// Endpoint Routing
// --------------------
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

app.Run();

