using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Toshokan.Libraries.Data;
using Toshokan.Libraries.Services;
using Toshokan.Libraries.Shared.Interfaces;

//setup our DI
var serviceProvider = new ServiceCollection()
     .AddDbContext<Context>(
        options => options.UseSqlServer("Server=.;Database=Toshokan;Trusted_Connection=True;"))
    .AddTransient<DbInitialiser>()
    .AddTransient<MangaService, MangaService>()
    .AddTransient<EpisodeService>()
    .AddTransient<PageService>()
    .BuildServiceProvider();

var service = serviceProvider.GetService<PageService>();
var initializer = serviceProvider.GetService<DbInitialiser>();

initializer.Run();

while (true)
{
    await service.Process();
    await Task.Delay(TimeSpan.FromMinutes(5));
}