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
using System.Reflection;

namespace Toshokan.Applications.Webapp.Pages
{
    public partial class Episode
    {
        [Inject]
        public DataService? DataService { get; set; }

        [Inject]
        public NavigationManager? NavigationManager { get; set; }


        [Parameter]
        public string? EpisodeId { get; set; }

        public Libraries.Models.Manga? CurrentManga { get; set; }
        public Libraries.Models.Episode? CurrentEpisode { get; set; }
        public Libraries.Models.Page? SelectedPage { get; set; }

        public Libraries.Models.Episode? PreviousEpisode { get; set; }
        public Libraries.Models.Episode? NextEpisode { get; set; }

        public int CurrentPage { get; set; } = 0;
        public int TotalPages { get; set; } = 0;
        public int Buffer { get; set; } = 4;

        public bool Loading { get; set; } = false;

        public List<Page>? Pages { get; set; }

        public async Task QueueEpisode()
        {
            if (this.CurrentEpisode != null && this.CurrentManga != null && this.DataService != null)
            {
                await DataService.SetQueueEpisode(this.CurrentManga.Id, this.CurrentEpisode.Id);
                this.CurrentEpisode.Queued = true;
                StateHasChanged();
            }
        }

        public async Task ChangePage(bool forwards)
        {
            if (this.Pages != null)
            {
                if (forwards)
                {
                    var newOrder = CurrentPage + 1;

                    // Case for not being processed, skip to next
                    if (TotalPages == 0)
                    {
                        // Navitage to next if exists
                        if (this.NextEpisode != null)
                        {
                            NavigationManager?.NavigateTo($"/episode/{this.NextEpisode.Id}");
                        }
                    }

                    if (newOrder >= TotalPages)
                    {
                        if (this.CurrentManga != null && this.CurrentEpisode != null && this.DataService != null)
                        {
                            // Mark as read and redirect 
                            if (this.CurrentEpisode.Processed)
                            {
                                // Set is as read if it was processed
                                await DataService.SetReadEpisode(this.CurrentManga.Id, this.CurrentEpisode.Id);

                                // Navitage to next if exists
                                if (this.NextEpisode != null)
                                {
                                    NavigationManager?.NavigateTo($"/episode/{this.NextEpisode.Id}");
                                }
                                else
                                {
                                    NavigationManager?.NavigateTo($"/manga/{this.CurrentManga.Id}");
                                }
                            }
                        }
                    }
                    else
                    {
                        // Load next page first
                        {
                            CurrentPage++;
                            this.SelectedPage = this.Pages[CurrentPage];
                            StateHasChanged();
                        }

                        // In the meantime, load more for cache
                        if (!this.Pages.Any(x => x.Order == newOrder + Buffer + 2))
                        {
                            // This is for cache
                            if (!Loading)
                            {
                                Loading = true;

                                if (this.CurrentManga != null && this.CurrentEpisode != null && this.DataService != null)
                                {
                                    var newPages = await DataService.GetPages(this.CurrentManga.Id, this.CurrentEpisode.Id, this.Pages.Count(), Buffer);
                                    this.Pages.AddRange(newPages);
                                }

                                Loading = false;
                            }

                            StateHasChanged();
                        }
                    }
                }
                else
                {
                    if (CurrentPage > 0)
                    {
                        CurrentPage--;
                        this.SelectedPage = this.Pages[CurrentPage];
                        StateHasChanged();
                    }
                    else
                    {
                        // Navitage to next if exists
                        if (this.PreviousEpisode != null && !this.Loading)
                        {
                            NavigationManager?.NavigateTo($"/episode/{this.PreviousEpisode.Id}");
                        }
                    }
                }
            }

        }

        public async Task KeyboardEventHandler(KeyboardEventArgs args)
        {
            var keyPressed = args.Key.ToLowerInvariant();

            if (keyPressed == "arrowright" || keyPressed == "arrowleft")
            {
                await ChangePage(keyPressed == "arrowright");
            }
        }

        protected override async Task OnParametersSetAsync()
        {
            this.CurrentEpisode = null;
            this.CurrentManga = null;
            this.PreviousEpisode = null;
            this.NextEpisode = null;
            this.SelectedPage = null;
            this.Pages = null;
            this.CurrentPage = 0;
            StateHasChanged();

            if (!string.IsNullOrEmpty(this.EpisodeId))
            {
                var parsed = Guid.TryParse(this.EpisodeId, out var parsedEpisodeId);

                if (parsed)
                {
                    if (DataService != null)
                    {
                        this.CurrentEpisode = await DataService.GetSingleEpisode(parsedEpisodeId);

                        if (this.CurrentEpisode == null)
                        {
                            // Something went wrong or missing, go to 404
                            NavigationManager?.NavigateTo("/404");
                        }
                        else
                        {
                            this.CurrentManga = await DataService.GetSingleManga(this.CurrentEpisode.MangaId);

                            this.PreviousEpisode = await DataService.GetSingleEpisode(this.CurrentManga.Id, this.CurrentEpisode.Order - 1);
                            this.NextEpisode = await DataService.GetSingleEpisode(this.CurrentManga.Id, this.CurrentEpisode.Order + 1);
                            StateHasChanged();

                            // Initia load should get first 3
                            this.Pages = await DataService.GetPages(this.CurrentManga.Id, this.CurrentEpisode.Id, CurrentPage, Buffer);
                            this.TotalPages = await DataService.GetPagesCount(this.CurrentManga.Id, this.CurrentEpisode.Id);

                            if (this.Pages.Count > 0)
                            {
                                this.SelectedPage = this.Pages[CurrentPage];
                            }

                            StateHasChanged();
                        }
                    }
                }
                else
                {
                    // Something went wrong or missing, go to 404
                    NavigationManager?.NavigateTo("/404");
                }
            }
            else
            {
                // Something went wrong or missing, go to 404
                NavigationManager?.NavigateTo("/404");
            }
        }
    }
}