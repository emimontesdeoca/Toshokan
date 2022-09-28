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
    public partial class Manga
    {
        [Inject]
        public DataService DataService { get; set; }

        [Inject]
        public NavigationManager NavigationManager { get; set; }

        [Parameter]
        public string Id { get; set; }

        public string Title { get; set; }

        public Libraries.Models.Manga CurrentManga { get; set; }

        public List<Libraries.Models.Episode>? Episodes { get; set; }

        protected override async Task OnParametersSetAsync()
        {
            this.CurrentManga = null;
            StateHasChanged();

            if (!string.IsNullOrEmpty(this.Id))
            {
                var parsed = Guid.TryParse(Id, out var episodeId);

                if (parsed)
                {
                    this.CurrentManga = await DataService.GetSingleManga(episodeId);
                    StateHasChanged();

                    if (this.CurrentManga == null)
                    {
                        NavigationManager.NavigateTo("/404");
                    }

                    this.Episodes = await DataService.GetEpisodes(episodeId);
                    StateHasChanged();
                }
                else
                {
                    NavigationManager.NavigateTo("/");
                }
            }
            else
            {
                NavigationManager.NavigateTo("/");
            }
        }
    }
}