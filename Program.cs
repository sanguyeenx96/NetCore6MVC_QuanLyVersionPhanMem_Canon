using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;
using Quanlyversion.Models;
using System;

var builder = WebApplication.CreateBuilder(args);

//Using session
builder.Services.AddSession();
builder.Services.AddHttpContextAccessor();

//builder.Services.Configure<FormOptions>(x =>
//{
//    x.ValueLengthLimit = int.MaxValue;
//    x.MultipartBodyLengthLimit = int.MaxValue; // In case of multipart
//    x.ValueLengthLimit = int.MaxValue;
//});

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration
    .GetConnectionString("MyConnectionString")));

// Add services to the container.
//Using session
builder.Services.AddSession();
builder.Services.AddHttpContextAccessor();

builder.Services.AddControllersWithViews();
builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = int.MaxValue;
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

//Using session
app.UseSession();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Login}/{id?}");

app.Run();