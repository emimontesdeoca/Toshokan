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
        public DataService? DataService { get; set; }

        [Inject]
        public NotifierService NotificationService { get; set; }

        public string? Url { get; set; }
        public bool ProcessDirectly { get; set; }
        public bool Enabled { get; set; }
        public int HoursInterval { get; set; } = 1;
        public bool State { get; set; } = false;

        protected override void OnParametersSet()
        {
            Url = string.Empty;
            Enabled = false;
            ProcessDirectly = false;
            base.OnParametersSet();
        }

        public void Toggle()
        {
            State = !State;

            this.Url = string.Empty;
            this.Enabled = false;
            this.ProcessDirectly = false;
            this.HoursInterval = 1;

            StateHasChanged();
        }

        public string SetButtonSelected(int interval)
        {
            return interval == this.HoursInterval ? "btn-primary" : "";
        }
        public void SetInterval(int interval)
        {
            this.HoursInterval = interval;
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
                    if (this.DataService != null)
                    {
                        await DataService.AddManga(this.Url, this.ProcessDirectly, this.Enabled, this.HoursInterval);
                        await NotificationService.Update();
                        Toggle();
                    }
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