using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Toshokan.Libraries.Data;
using Toshokan.Libraries.Services;

// Connection string
var cs = "Server=db;Database=Toshokan;User Id=sa;Password=P@ssword01;";
cs = "Server=.;Database=Toshokan;Trusted_Connection=True;";

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddDbContext<Context>(
        options => options.UseSqlServer(cs, builder => builder.EnableRetryOnFailure(10, TimeSpan.FromSeconds(2), null)), ServiceLifetime.Transient);
builder.Services.AddTransient<DataService>();
builder.Services.AddTransient<DbInitialiser>();

builder.Services.AddScoped<NotifierService>();

var serviceProvider = builder.Services.BuildServiceProvider();
var initializer = serviceProvider.GetRequiredService<DbInitialiser>();
await initializer.EnsureCreated();

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

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
