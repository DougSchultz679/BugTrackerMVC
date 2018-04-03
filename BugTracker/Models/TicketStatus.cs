using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BugTracker.Models
{
    public class TicketStatus
    {
        public int Id { set; get; }
        public string Name { set; get; }

        public virtual ICollection<Ticket> Ticket { get; set; }

        public TicketStatus()
        {
            Ticket = new HashSet<Ticket>();
        }
    }
}