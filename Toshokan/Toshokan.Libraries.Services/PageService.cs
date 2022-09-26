﻿using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Toshokan.Libraries.Data;
using Toshokan.Libraries.Models;
using Toshokan.Libraries.Shared.Interfaces;
using Toshokan.Libraries.Shared.Utils;

namespace Toshokan.Libraries.Services
{
    public class PageService : IService
    {
        private readonly Context Context;

        public PageService(Context context)
        {
            Context = context;
        }

        public async Task Process()
        {
            LogUtils.Log("Starting PageService process");
            var unprocessedEpisodes = await Context.Episodes.Where(x => !x.Processed).OrderBy(x => x.Order).ToListAsync();

            LogUtils.Log($"{unprocessedEpisodes.Count} episodes to process");

            foreach (var episode in unprocessedEpisodes)
            {
                var data = await Shared.WebClientUtils.GetStringAsync(episode.Url);

                // Get pages
                var episodesString = data
                    .Split(new string[] { "<div class=\"container-chapter-reader\">" }, StringSplitOptions.None)[1]
                    .Split(new string[] { "<div style=\"text-align:center;\">" }, StringSplitOptions.None)[0];

                var episodeSplit = episodesString
                    .Split(new string[] { "<img" }, StringSplitOptions.None);

                LogUtils.Log($"Found {episodeSplit.Count() - 1} pages in episode");

                var order = 0;
                foreach (var item in episodeSplit.Skip(1))
                {
                    var url = item
                    .Split(new string[] { "src=\"" }, StringSplitOptions.None)[1]
                    .Split(new string[] { "\"" }, StringSplitOptions.None)[0];

                    // New page
                    var page = new Page
                    {
                        // Initial properties
                        Id = Guid.NewGuid(),
                        EpisodeId = episode.Id,
                        MangaId = episode.MangaId,
                        Order = order
                    };

                    // Get data
                    var bytes = await Shared.WebClientUtils.GetDataAsync(url);

                    // Compress
                    var compressedBytes = Shared.Utils.CompressionUtils.Compress(bytes);

                    // Assign data
                    page.Data = Convert.ToBase64String(compressedBytes, 0, compressedBytes.Length);

                    // Dates
                    page.CreatedAt = DateTime.UtcNow;
                    page.UpdatedAt = DateTime.UtcNow;

                    // Add
                    await Context.Pages.AddAsync(page);

                    LogUtils.Log($"Added new page {order} for episode {episode.Name}");

                    // Update order
                    order++;
                }

                // Update episode
                episode.Processed = true;
                episode.UpdatedAt = DateTime.UtcNow;

                // Save
                await Context.SaveChangesAsync();
            }

            LogUtils.Log("Finished PageService process");
        }
    }
}
