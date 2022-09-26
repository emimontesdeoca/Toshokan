using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Toshokan.Libraries.Models
{
    public class Page
    {
        public Guid Id { get; set; }
        public Guid MangaId { get; set; }
        public Guid EpisodeId { get; set; }
        public string? Data { get; set; }
        public int Order { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public Page()
        {
        }
    }
}
