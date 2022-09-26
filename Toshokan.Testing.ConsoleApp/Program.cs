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

var initializer = serviceProvider.GetService<DbInitialiser>();

//initializer.Run();

// Add test manga

var manga = new Manga("https://mangakakalot.com/read-mo5of158504931270");

var db = serviceProvider.GetService<Context>();
db.Mangas.Add(manga);
db.SaveChanges();
