using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BugTracker.Models
{
    public class TicketType
    {
        public int Id { set; get; }
        public string Name { set; get; }

        public virtual ICollection<Ticket> Ticket { get; set; }

        public TicketType()
        {
            Ticket = new HashSet<Ticket>();
        }
    }
}