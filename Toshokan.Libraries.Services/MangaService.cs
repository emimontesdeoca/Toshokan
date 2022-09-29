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
    public class MangaService : IService
    {
        private readonly Context Context;

        public MangaService(Context context)
        {
            Context = context;
        }

        public async Task Process()
        {
            LogUtils.Log("Starting MangaService process");

            var unprocessedMangas = await Context.Mangas.Where(x => !x.Processed && x.Enabled).ToListAsync();
            LogUtils.Log($"{unprocessedMangas.Count} mangas to process");

            foreach (var manga in unprocessedMangas)
            {
                if (manga.Url != null)
                {
                    LogUtils.Log($"Processing '{manga.Url}'");

                    // Get data
                    var data = await Shared.WebClientUtils.GetStringAsync(manga.Url);

                    // Get information from HTML
                    manga.Name = this.GetName(data);
                    manga.AlternativeName = this.GetAlternativeName(data);
                    manga.Genres = this.GetGenres(data);
                    manga.Authors = this.GetAuthors(data);
                    manga.Status = this.GetStatus(data);
                    manga.Summary = this.GetSummary(data);
                    manga.Poster = await this.GetPoster(data);

                    // Dates
                    manga.UpdatedAt = DateTime.UtcNow;

                    // State
                    manga.Enabled = true;
                    manga.Processed = true;

                    // Notificaiton
                    var notification = new Notification($"Manga '{manga.Name}' has been processed", $"/manga/{manga.Id}");
                    await Context.Notifications.AddAsync(notification);

                    // Save
                    await Context.SaveChangesAsync();

                    LogUtils.Log($"Manga '{manga.Name}' processed");
                }
            }

            LogUtils.Log("Finished MangaService process");
        }

        #region Private methods

        private string GetName(string data)
        {
            var name = data
                .Split(new string[] { "<ul class=\"manga-info-text\">" }, StringSplitOptions.None)[1]
                .Split(new string[] { "<h1>" }, StringSplitOptions.None)[1]
                .Split(new string[] { "</h1>" }, StringSplitOptions.None)[0];

            return name.Trim();
        }

        private string GetAlternativeName(string data)
        {
            var name = data
                .Split(new string[] { "<h2 class=\"story-alternative\">" }, StringSplitOptions.None)[1]
                .Split(new string[] { "Alternative :" }, StringSplitOptions.None)[1]
                .Split(new string[] { "</h2>" }, StringSplitOptions.None)[0];

            return name.Replace(";", ",").Trim();
        }

        private bool GetStatus(string data)
        {
            var ongoing = data
                .Split(new string[] { "Status : " }, StringSplitOptions.None)[1]
                .Split(new string[] { "</li>" }, StringSplitOptions.None)[0];

            return ongoing == "Ongoing";
        }

        private string GetGenres(string data)
        {
            var genres = new List<string>();
            var genresString = data
                .Split(new string[] { "Genres :" }, StringSplitOptions.None)[1]
                .Split(new string[] { "</li>" }, StringSplitOptions.None)[0];

            var genresSplit = genresString
                .Split(new string[] { "<a" }, StringSplitOptions.None);

            foreach (var item in genresSplit.Skip(1))
            {
                var genre = item
                .Split(new string[] { ">" }, StringSplitOptions.None)[1]
                .Split(new string[] { "<" }, StringSplitOptions.None)[0];

                genres.Add(genre.Trim());
            }

            return string.Join(", ", genres);
        }

        private string GetAuthors(string data)
        {
            var authors = new List<string>();
            var authorsString = data
                .Split(new string[] { "Author(s) :" }, StringSplitOptions.None)[1]
                .Split(new string[] { "</li>" }, StringSplitOptions.None)[0];

            var authorsSplit = authorsString
                .Split(new string[] { "<a" }, StringSplitOptions.None);

            foreach (var item in authorsSplit.Skip(1))
            {
                var genre = item
                .Split(new string[] { ">" }, StringSplitOptions.None)[1]
                .Split(new string[] { "<" }, StringSplitOptions.None)[0];

                authors.Add(genre.Trim());
            }

            return string.Join(", ", authors);
        }

        private string GetSummary(string data)
        {
            var summary = data
                .Split(new string[] { "summary:" }, StringSplitOptions.None)[1]
                .Split(new string[] { "</h2>" }, StringSplitOptions.None)[1]
                .Split(new string[] { "</div>" }, StringSplitOptions.None)[0];

            summary = summary.Replace("\n", "");

            return summary.Trim();
        }

        private async Task<string> GetPoster(string data)
        {
            var posterUrl = data
                .Split(new string[] { "<div class=\"manga-info-pic\">" }, StringSplitOptions.None)[1]
                .Split(new string[] { "src=\"" }, StringSplitOptions.None)[1]
                .Split(new string[] { "\"" }, StringSplitOptions.None)[0];

            // Get data
            var bytes = await Shared.WebClientUtils.GetDataAsync(posterUrl);

            // return data
            return "data:image/png;base64," + Convert.ToBase64String(bytes, 0, bytes.Length);
        }

        #endregion
    }
}