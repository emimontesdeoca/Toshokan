using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Toshokan.Libraries.Models
{
    public class Notification
    {
        public Guid Id { get; set; }
        public string Message { get; set; }
        public string Link { get; set; }
        public DateTime CreatedAt { get; set; }

        public Notification()
        {
        }

        public Notification(string message, string link)
        {
            this.Id = Guid.NewGuid();
            this.CreatedAt = DateTime.UtcNow;
            this.Message = message;
            this.Link = link;
        }
    }
}
