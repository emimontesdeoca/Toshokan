using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Toshokan.Libraries.Data;
using Toshokan.Libraries.Models;
using Toshokan.Libraries.Services;
using Toshokan.Libraries.Shared.Interfaces;

// Connection string 
var cs = "Server=db;Database=Toshokan;User Id=sa;Password=P@ssword01;";

// Setup our DI
var serviceProvider = new ServiceCollection()
    .AddDbContext<Context>(
        options => options.UseSqlServer(cs))
    .AddTransient<DbInitialiser>()
    .AddTransient<MangaService>()
    .AddTransient<EpisodeService>()
    .AddTransient<PageService>()
    .BuildServiceProvider();

//var initializer = serviceProvider.GetRequiredService<DbInitialiser>();
