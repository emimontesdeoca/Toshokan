using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using Toshokan.Libraries.Data;
using Toshokan.Libraries.Models;
using Toshokan.Libraries.Shared.Interfaces;
using Toshokan.Libraries.Shared.Utils;

namespace Toshokan.Libraries.Services
{
    public class DataService
    {
        private readonly Context Context;

        public DataService(Context context)
        {
            Context = context;
        }

        public async Task<int> GetTotalMangas()
        {
            return await this.Context.Mangas.CountAsync();
        }

        public async Task<int> GetTotalMangasToday()
        {
            return await this.Context.Mangas.Where(x => x.CreatedAt.Date == DateTime.UtcNow.Date).CountAsync();
        }

        public async Task<int> GetTotalEpisodes()
        {
            return await this.Context.Episodes.CountAsync();
        }

        public async Task<int> GetTotalEspisodesToday()
        {
            return await this.Context.Episodes.Where(x => x.CreatedAt.Date == DateTime.UtcNow.Date).CountAsync();
        }

        public async Task<int> GetTotalPages()
        {
            return await this.Context.Pages.CountAsync();
        }

        public async Task<int> GetTotalPagesToday()
        {
            return await this.Context.Pages.Where(x => x.CreatedAt.Date == DateTime.UtcNow.Date).CountAsync();
        }

        public async Task<List<Notification>> GetNotifications(int skip, int take)
        {
            return await this.Context.Notifications.OrderByDescending(x => x.CreatedAt).Skip(skip).Take(take).AsNoTracking().ToListAsync();
        }

        public async Task DeleteNotifications()
        {
            await Context.Database.ExecuteSqlRawAsync("TRUNCATE table dbo.Notifications");
        }

        public async Task<List<Manga>> GetLatestMangaProcessed(int skip, int take)
        {
            return await this.Context.Mangas.Where(x => x.Processed).OrderByDescending(x => x.CreatedAt).Skip(skip).Take(take).AsNoTracking().ToListAsync();
        }

        public async Task<List<EpisodeAdded>> GetLatestEpisodeAdded(int skip, int take)
        {

            var query = from episode in Context.Episodes
                        join manga in Context.Mangas
                            on episode.MangaId equals manga.Id
                        where episode.Processed
                        orderby episode.UpdatedAt descending

                        select new EpisodeAdded()
                        {
                            Id = episode.Id.ToString(),
                            Name = episode.Name,
                            Poster = manga.Poster
                        };

            return await query.AsNoTracking().Skip(skip).Take(take).ToListAsync();

        }

        public async Task<List<Manga>> GetLatestManga(int skip, int take)
        {
            return await this.Context.Mangas.Where(x => x.Processed).OrderByDescending(x => x.CreatedAt).Skip(skip).Take(take).AsNoTracking().ToListAsync();
        }

        public async Task<List<Manga>> GetManga(string? type)
        {
            switch (type?.ToLowerInvariant())
            {
                case "latest":
                    return await this.Context.Mangas.Where(x => x.Processed).OrderByDescending(x => x.UpdatedAt).AsNoTracking().ToListAsync();
                case "newest":
                    return await this.Context.Mangas.Where(x => x.Processed).OrderByDescending(x => x.CreatedAt).AsNoTracking().ToListAsync();
                case "processed":
                    return await this.Context.Mangas.Where(x => x.Processed).OrderBy(x => x.Name).AsNoTracking().ToListAsync();
                case "unprocessed":
                    return await this.Context.Mangas.Where(x => !x.Processed).OrderBy(x => x.Name).AsNoTracking().ToListAsync();
                case "all":
                default:
                    return await this.Context.Mangas.OrderBy(x => x.Name).AsNoTracking().ToListAsync();
            }
        }

        public async Task<Manga> GetSingleManga(Guid id)
        {
            return await this.Context.Mangas.AsNoTracking().SingleAsync(x => x.Id == id);
        }

        public async Task<Episode> GetSingleEpisode(Guid id)
        {
            return await this.Context.Episodes.AsNoTracking().SingleAsync(x => x.Id == id);
        }

        public async Task<Episode?> GetSingleEpisode(Guid id, int order)
        {
            return await this.Context.Episodes.AsNoTracking().SingleOrDefaultAsync(x => x.MangaId == id && x.Order == order);
        }

        public async Task SetReadEpisode(Guid mangaId, Guid episodeId)
        {
            var episode = await this.Context.Episodes.SingleAsync(x => x.Id == episodeId && x.MangaId == mangaId);

            episode.Read = true;
            await Context.SaveChangesAsync();
        }

        public async Task SetQueueEpisode(Guid mangaId, Guid episodeId)
        {
            var episode = await this.Context.Episodes.SingleAsync(x => x.Id == episodeId && x.MangaId == mangaId);

            episode.Queued = true;
            await Context.SaveChangesAsync();
        }

        public async Task<List<Episode>> GetEpisodes(Guid id)
        {
            return await this.Context.Episodes.AsNoTracking().Where(x => x.MangaId == id).OrderByDescending(x => x.Order).ToListAsync();
        }

        public async Task<List<Page>> GetPages(Guid mangaId, Guid episodeId, int skip, int take)
        {
            return await this.Context.Pages
                .AsNoTracking()
                .Where(x => x.MangaId == mangaId && x.EpisodeId == episodeId)
                .OrderBy(x => x.Order)
                .Skip(skip)
                .Take(take)
                .ToListAsync();
        }

        public async Task<int> GetPagesCount(Guid mangaId, Guid episodeId)
        {
            return await this.Context.Pages
                .AsNoTracking()
                .Where(x => x.MangaId == mangaId && x.EpisodeId == episodeId)
                .CountAsync();
        }

        public async Task Process(Guid id)
        {
            var manga = await this.Context.Mangas.SingleAsync(x => x.Id == id);
            manga.Processed = false;
            await Context.SaveChangesAsync();
        }

        public async Task ToggleStatus(Guid id, bool status)
        {
            var manga = await this.Context.Mangas.SingleAsync(x => x.Id == id);
            manga.Enabled = status;
            await Context.SaveChangesAsync();
        }

        public async Task Delete(Guid id, bool status)
        {
            var manga = await this.Context.Mangas.SingleAsync(x => x.Id == id);
            manga.Delete = status;
            await Context.SaveChangesAsync();
        }

        public async Task AddManga(string url, bool processDirectly, bool enabled, int interval)
        {
            var manga = new Manga(url);

            manga.ProcessDirectly = processDirectly;
            manga.Enabled = enabled;
            manga.Interval = interval;

            var notification = new Notification($"New manga added with link '{url}'", $"/manga/{manga.Id}");

            await Context.Mangas.AddAsync(manga);
            await Context.Notifications.AddAsync(notification);
            await Context.SaveChangesAsync();
        }
    }
}