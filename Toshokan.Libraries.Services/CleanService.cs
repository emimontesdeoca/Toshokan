using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Immutable;
using System.Net;
using Toshokan.Libraries.Data;
using Toshokan.Libraries.Models;
using Toshokan.Libraries.Shared.Interfaces;
using Toshokan.Libraries.Shared.Utils;

namespace Toshokan.Libraries.Services
{
    public class CleanService : IService
    {
        private readonly Context Context;

        public CleanService(Context context)
        {
            Context = context;
        }

        public async Task Process()
        {
            LogUtils.Log("Starting CleanService process");

            // Mangas to be deled
            LogUtils.Log($"Looking for mangas to be deleted");
            var mangasToBeDeleted = await Context.Mangas.Where(x => x.Delete).ToListAsync();
            LogUtils.Log($"Found {mangasToBeDeleted.Count} to be deleted");

            foreach (var manga in mangasToBeDeleted)
            {
                // Get all episodes from this manga
                var episodes = await Context.Episodes.Where(x => x.MangaId == manga.Id).ToListAsync();
                LogUtils.Log($"Found {episodes.Count} to be deleted from '{manga.Name}'");

                // Get all pages from this manga
                var pages = await Context.Pages.Where(x => x.MangaId == manga.Id).ToListAsync();
                LogUtils.Log($"Found {pages.Count} to be deleted from '{manga.Name}'");

                // Remove them
                Context.Episodes.RemoveRange(episodes);
                Context.Pages.RemoveRange(pages);
            }

            // Remove all mangas to be deleted
            Context.Mangas.RemoveRange(mangasToBeDeleted);
            LogUtils.Log($"Removed mangas from database");

            LogUtils.Log($"Fetching read episodes to be deleted");
            var episodesToBeDeleted = await Context.Episodes.Where(x => x.Delete).ToListAsync();
            LogUtils.Log($"Found {episodesToBeDeleted.Count} episodes to be deleted");

            foreach (var episode in episodesToBeDeleted)
            {
                // Get all pages and remove
                var pages = await Context.Pages.Where(x => x.EpisodeId == episode.Id).ToListAsync();
                LogUtils.Log($"Found {pages.Count} to be deleted from '{episode.Name}'");

                // Remove them
                Context.Pages.RemoveRange(pages);
            }

            Context.Episodes.RemoveRange(episodesToBeDeleted);
            LogUtils.Log($"Removed episodes from database");

            // Clear notifications
            var notificationsToDelete = await Context.Notifications.Where(x => x.CreatedAt < DateTime.UtcNow.AddDays(-15)).ToListAsync();
            Context.Notifications.RemoveRange(notificationsToDelete);
            LogUtils.Log($"Removed {notificationsToDelete} notifications from database");

            // Save it
            await Context.SaveChangesAsync();
            LogUtils.Log($"Saved changes");

            LogUtils.Log("Finished CleanService process");
        }
    }
}