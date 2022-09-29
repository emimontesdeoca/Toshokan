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
    public class EpisodeService : IService
    {
        private readonly Context Context;

        public EpisodeService(Context context)
        {
            Context = context;
        }

        public async Task Process()
        {
            LogUtils.Log("Starting EpisodeService process");
            var processedMangas = await Context.Mangas.Where(x => x.Processed && x.Enabled && x.NextCheck < DateTime.UtcNow).ToListAsync();

            LogUtils.Log($"{processedMangas.Count} mangas to process");

            foreach (var manga in processedMangas)
            {
                if (manga.Url != null)
                {
                    LogUtils.Log($"Processing {manga.Name}");

                    var data = await Shared.WebClientUtils.GetStringAsync(manga.Url);

                    // Get episodes
                    var episodesString = data
                        .Split(new string[] { "<div class=\"manga-info-chapter\">" }, StringSplitOptions.None)[1]
                        .Split(new string[] { "<div class=\"chapter-list\">" }, StringSplitOptions.None)[1]
                        .Split(new string[] { "<div class=\"comment-info\">" }, StringSplitOptions.None)[0];

                    var episodeSplit = episodesString
                        .Split(new string[] { "<div class=\"row\">" }, StringSplitOptions.None);

                    LogUtils.Log($"Found {episodeSplit.Count() - 1} episodes for '{manga.Name}'");

                    var order = 0;
                    foreach (var item in episodeSplit.Skip(1).Reverse())
                    {
                        var episodeUrl = item
                        .Split(new string[] { "<a href=\"" }, StringSplitOptions.None)[1]
                        .Split(new string[] { "\"" }, StringSplitOptions.None)[0];

                        var name = item
                       .Split(new string[] { "<a href=\"" }, StringSplitOptions.None)[1]
                       .Split(new string[] { ">" }, StringSplitOptions.None)[1]
                       .Split(new string[] { "<" }, StringSplitOptions.None)[0];

                        // Here we check if this url is on the database, we have to keep the order tho
                        if (!(await Context.Episodes.Where(x => x.Url == episodeUrl && x.MangaId == manga.Id).AnyAsync()))
                        {
                            // Create episode
                            var episode = new Episode(manga.Id, episodeUrl, name, order, manga.ProcessDirectly);

                            // Add to context
                            await Context.Episodes.AddAsync(episode);

                            // Update manga
                            manga.UpdatedAt = DateTime.UtcNow;

                            LogUtils.Log($"Added '{episode.Name}' episode to '{manga.Name}'");

                            // Create notification
                            var notification = new Notification($"Added '{episode.Name}' episode to '{manga.Name}'", $"/episode/{episode.Id}");
                            await Context.Notifications.AddAsync(notification);
                        }

                        order++;
                    }

                    // Update next date to check
                    manga.NextCheck = DateTime.UtcNow.AddHours(manga.Interval);

                    // Save everything
                    await Context.SaveChangesAsync();
                }
            }

            LogUtils.Log("Finished EpisodeService process");
        }
    }
}