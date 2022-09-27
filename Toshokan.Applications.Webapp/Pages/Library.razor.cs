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

namespace Toshokan.Applications.Webapp.Pages
{
    public partial class Library
    {
        [Inject]
        public DataService DataService { get; set; }


        #region Library

        public List<Libraries.Models.Manga> MangaList { get; set; }

        #endregion

        #region Overrides

        protected override async Task OnInitializedAsync()
        {
            this.MangaList = await DataService.GetAllManga();
        }

        #endregion
    }
}