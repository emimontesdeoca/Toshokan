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
using Toshokan.Libraries.Models;
using Toshokan.Libraries.Services;
using System.ComponentModel;

namespace Toshokan.Applications.Webapp.Pages
{
    public partial class Library
    {
        [Inject]
        public DataService? DataService { get; set; }

        [Parameter]
        public string? Type { get; set; }


        public string? Title { get; set; }


        #region Library

        public List<Libraries.Models.Manga>? MangaList { get; set; }

        #endregion

        #region Actions

        public async Task Process(Guid id)
        {
            if (this.DataService != null)
            {
                await DataService.Process(id);
                await Load();
            }
        }
        public async Task ToggleStatus(Guid id, bool status)
        {
            if (this.DataService != null)
            {
                await DataService.ToggleStatus(id, status);
                await Load();
            }
        }
        public async Task Delete(Guid id, bool status)
        {
            if (this.DataService != null)
            {
                await DataService.Delete(id, status);
                await Load();
            }
        }

        public async Task Load()
        {
            this.MangaList = null;

            switch (Type?.ToLowerInvariant())
            {
                case "latest":
                    Title = "Latest mangas updated";
                    break;
                case "newest":
                    Title = "Newest mangas added";
                    break;
                case "processed":
                    Title = "Processed mangas";
                    break;
                case "unprocessed":
                    Title = "Unprocessed mangas";
                    break;
                case "all":
                default:
                    Title = "All mangas";
                    break;
            }

            if (this.DataService != null)
            {
                this.MangaList = await DataService.GetManga(this.Type);
            }
        }

        #endregion

        #region Overrides

        protected override async Task OnParametersSetAsync()
        {
            await Load();
        }

        #endregion
    }
}