using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Toshokan.Libraries.Models
{
    public class Episode
    {
        public Guid Id { get; set; }
        public Guid MangaId { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public int Order { get; set; }
        public bool Processed { get; set; }
        public bool Queued { get; set; }
        public bool Read { get; set; }
        public bool Delete { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public Episode()
        {
        }

        public Episode(Guid mangaId, string url, string name, int order, bool queue)
        {
            // Initial properties
            this.Id = Guid.NewGuid();
            this.MangaId = mangaId;
            this.Url = url;
            this.Order = order;
            this.Name = name;

            // Dates
            this.CreatedAt = DateTime.UtcNow;
            this.UpdatedAt = DateTime.UtcNow;

            // State
            this.Processed = false;
            this.Read = false;
            this.Delete = false;
            this.Queued = queue;
        }
    }
}
