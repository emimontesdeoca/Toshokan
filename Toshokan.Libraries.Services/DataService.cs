using Microsoft.EntityFrameworkCore;
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

        public async Task<List<Manga>> GetLatestMangaProcessed(int skip, int take)
        {
            return await this.Context.Mangas.Where(x => x.Processed).OrderByDescending(x => x.CreatedAt).Skip(skip).Take(take).AsNoTracking().ToListAsync();
        }

        public async Task<List<EpisodeAdded>> GetLatestEpisodeAdded(int skip, int take)
        {

            var query = from episode in Context.Episodes
                        join manga in Context.Mangas
                            on episode.MangaId equals manga.Id
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
            return await this.Context.Mangas.OrderByDescending(x => x.CreatedAt).Skip(skip).Take(take).AsNoTracking().ToListAsync();
        }
    }
}