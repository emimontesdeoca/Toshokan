using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using System.Net.Http;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.Web.Virtualization;
using Microsoft.JSInterop;
using Toshokan.Applications.Webapp;
using Toshokan.Applications.Webapp.Shared;
using Toshokan.Applications.Webapp.Components;
using Toshokan.Libraries.Services;
using System.Globalization;
using Toshokan.Libraries.Models;

namespace Toshokan.Applications.Webapp.Pages
{
    public partial class Index
    {
        [Inject]
        public DataService DataService { get; set; }

        #region Dashboard boxes

        public int? TotalMangas { get; set; }
        public int? TotalEpisodes { get; set; }
        public int? TotalPages { get; set; }
        public int? TotalMangasToday { get; set; }
        public int? TotalEpisodesToday { get; set; }
        public int? TotalPagesToday { get; set; }

        #endregion

        #region Notifications

        public List<Notification> LatestNotifications { get; set; }

        #endregion

        #region Latest manga processed

        public List<Libraries.Models.Manga> LatestMangasProcessed { get; set; }

        #endregion

        #region Latest episodes added

        public List<EpisodeAdded> LatestEpisodeAdded { get; set; }

        #endregion

        #region Library

        public List<Libraries.Models.Manga> MangaList { get; set; }

        #endregion

        #region Overrides

        protected override async Task OnInitializedAsync()
        {
            this.TotalMangas = await DataService.GetTotalMangas();
            this.TotalEpisodes = await DataService.GetTotalEpisodes();
            this.TotalPages = await DataService.GetTotalPages();
            this.TotalMangasToday = await DataService.GetTotalMangasToday();
            this.TotalEpisodesToday = await DataService.GetTotalEspisodesToday();
            this.TotalPagesToday = await DataService.GetTotalPagesToday();

            this.LatestNotifications = await DataService.GetNotifications(0, 15);
            this.LatestMangasProcessed = await DataService.GetLatestMangaProcessed(0, 4);
            this.LatestEpisodeAdded = await DataService.GetLatestEpisodeAdded(0, 8);
            this.MangaList = await DataService.GetLatestManga(0, 10);
        }

        #endregion
    }
}