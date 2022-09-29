using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Toshokan.Libraries.Data;
using Toshokan.Libraries.Models;
using Toshokan.Libraries.Services;
using Toshokan.Libraries.Shared.Interfaces;

//setup our DI
var serviceProvider = new ServiceCollection()
     .AddDbContext<Context>(
        options => options.UseSqlServer("Server=.;Database=Toshokan;Trusted_Connection=True;"))
    .AddTransient<DbInitialiser>()
    .AddTransient<IService, MangaService>()
    .AddTransient<IService, EpisodeService>()
    .AddTransient<IService, PageService>()
    .BuildServiceProvider();

var initializer = serviceProvider.GetRequiredService<DbInitialiser>();

initializer.Run();

// Add test manga
