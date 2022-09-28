using System.Net;

namespace Toshokan.Libraries.Models
{
    public class Manga
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public string? AlternativeName { get; set; }
        public string Url { get; set; }
        public string? Genres { get; set; }
        public string? Authors { get; set; }
        public string? Summary { get; set; }
        public string? Poster { get; set; }
        public bool? Status { get; set; }
        public bool Enabled { get; set; }
        public bool Processed { get; set; }
        public bool ProcessDirectly { get; set; }
        public bool Delete { get; set; }
        public int Interval { get; set; }
        public DateTime NextCheck { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public Manga()
        {
        }

        public Manga(string url)
        {
            this.Id = Guid.NewGuid();
            this.Name = string.Empty;
            this.AlternativeName = string.Empty;
            this.Url = url;
            this.Enabled = true;
            this.Processed = false;
            this.Delete = false;
            this.ProcessDirectly = false;
            this.Interval = 1;
            this.CreatedAt = DateTime.UtcNow;
            this.UpdatedAt = DateTime.UtcNow;
            this.NextCheck = DateTime.UtcNow;
        }
    }
}