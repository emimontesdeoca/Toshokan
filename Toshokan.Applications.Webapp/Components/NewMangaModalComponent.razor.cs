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
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using Toshokan.Libraries.Services;

namespace Toshokan.Applications.Webapp.Components
{
    public partial class NewMangaModalComponent
    {
        [Inject]
        public DataService DataService { get; set; }

        public string Url { get; set; }
        public bool State { get; set; } = false;

        protected override async Task OnParametersSetAsync()
        {
            this.Url = string.Empty;
        }

        public void Toggle()
        {
            State = !State;
            StateHasChanged();
        }

        public async Task AddManga()
        {
            if (string.IsNullOrEmpty(this.Url))
            {
                // TODO: error management
            }
            else
            {

                if (this.Url.Contains("mangakakalot"))
                {
                    await DataService.AddManga(this.Url);
                    this.State = false;
                }
                else
                {
                    // Another error
                }
            }

            StateHasChanged();
        }
    }
}