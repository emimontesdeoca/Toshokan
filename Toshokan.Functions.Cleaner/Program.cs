using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Toshokan.Libraries.Data;
using Toshokan.Libraries.Services;
using Toshokan.Libraries.Shared.Interfaces;

// Connection string 
var cs = "Server=db;Database=Toshokan;User Id=sa;Password=P@ssword01;";

// Setup our DI
var serviceProvider = new ServiceCollection()
    .AddDbContext<Context>(options => options.UseSqlServer(cs, builder => builder.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null)), ServiceLifetime.Transient)
    .AddTransient<DbInitialiser>()
    .AddTransient<MangaService>()
    .AddTransient<EpisodeService>()
    .AddTransient<PageService>()
    .AddTransient<CleanService>()
    .BuildServiceProvider();

var service = serviceProvider.GetRequiredService<CleanService>();
var initializer = serviceProvider.GetRequiredService<DbInitialiser>();

while (!await initializer.CanConnect())
{
    while (true)
    {
        await service.Process();
        await Task.Delay(TimeSpan.FromMinutes(1));
    }
}