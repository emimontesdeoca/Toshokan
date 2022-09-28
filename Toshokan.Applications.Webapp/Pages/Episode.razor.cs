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
using Toshokan.Libraries.Models;

namespace Toshokan.Applications.Webapp.Pages
{
    public partial class Episode
    {
        [Inject]
        public DataService DataService { get; set; }

        [Inject]
        public NavigationManager NavigationManager { get; set; }


        [Parameter]
        public string EpisodeId { get; set; }

        public string Title { get; set; }

        public Libraries.Models.Manga? CurrentManga { get; set; }
        public Libraries.Models.Episode? CurrentEpisode { get; set; }

        public Libraries.Models.Episode? PreviousEpisode { get; set; }
        public Libraries.Models.Episode? NextEpisode { get; set; }

        public List<Page>? Pages { get; set; }

        public async Task QueueEpisode()
        {
            if (this.CurrentEpisode != null)
            {
                await DataService.SetQueueEpisode(this.CurrentManga.Id, this.CurrentEpisode.Id);
                this.CurrentEpisode.Queued = true;
                StateHasChanged();
            }
        }

        protected override async Task OnParametersSetAsync()
        {
            this.CurrentEpisode = null;
            this.CurrentManga = null;
            this.PreviousEpisode = null;
            this.NextEpisode = null;
            this.Pages = null;
            StateHasChanged();

            if (!string.IsNullOrEmpty(this.EpisodeId))
            {
                var parsed = Guid.TryParse(this.EpisodeId, out var parsedEpisodeId);

                if (parsed)
                {
                    this.CurrentEpisode = await DataService.GetSingleEpisode(parsedEpisodeId);

                    if (this.CurrentEpisode == null)
                    {
                        // Something went wrong or missing, go to 404
                        NavigationManager.NavigateTo("/404");
                    }
                    else
                    {
                        this.CurrentManga = await DataService.GetSingleManga(this.CurrentEpisode.MangaId);

                        this.PreviousEpisode = await DataService.GetSingleEpisode(this.CurrentManga.Id, this.CurrentEpisode.Order - 1);
                        this.NextEpisode = await DataService.GetSingleEpisode(this.CurrentManga.Id, this.CurrentEpisode.Order + 1);
                        StateHasChanged();

                        this.Pages = await DataService.GetPages(this.CurrentManga.Id, parsedEpisodeId);
                        StateHasChanged();

                        if (this.CurrentEpisode.Processed)
                        {
                            // Set is as read if it was processed
                            await DataService.SetReadEpisode(this.CurrentManga.Id, parsedEpisodeId);
                        }
                    }
                }
                else
                {
                    // Something went wrong or missing, go to 404
                    NavigationManager.NavigateTo("/404");
                }
            }
            else
            {
                // Something went wrong or missing, go to 404
                NavigationManager.NavigateTo("/404");
            }
        }
    }
}