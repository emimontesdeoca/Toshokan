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
    .AddTransient<MangaService>()
    .AddTransient<EpisodeService>()
    .AddTransient<PageService>()
    .BuildServiceProvider();

var service = serviceProvider.GetRequiredService<MangaService>();
var initializer = serviceProvider.GetRequiredService<DbInitialiser>();

initializer.Run();

while (true)
{
    await service.Process();
    await Task.Delay(TimeSpan.FromMinutes(1));
}