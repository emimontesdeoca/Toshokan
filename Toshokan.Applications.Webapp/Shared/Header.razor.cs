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
using System.Text.Json;

namespace Toshokan.Applications.Webapp.Shared
{
    public partial class Header
    {
        #region Injects

        [Inject]
        public DataService DataService { get; set; }

        [Inject]
        public IJSRuntime IJSRuntime { get; set; }

        #endregion


        #region Properties

        public List<Notification> Notifications { get; set; }

        public NewMangaModalComponent NewMangaModalComponent { get; set; } = new NewMangaModalComponent();

        #endregion

        #region Overrides

        protected override async Task OnInitializedAsync()
        {
            this.Notifications = await DataService.GetNotifications(0, 5);
        }

        public async Task ToggleTheme() => await IJSRuntime.InvokeVoidAsync("toggleTheme");

        public async Task CloseToggle() => await IJSRuntime.InvokeVoidAsync("hideDropdownClick");

        public async Task OpenModal() => this.NewMangaModalComponent.Toggle();

        #endregion
    }
}