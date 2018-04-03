using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BugTracker.Models.ViewModels
{
    public class DashPMViewModel
    {
        public ApplicationUser PMUser { get; set; }
        public ICollection<Ticket> PriorityTkts { get; set; }
        public ICollection<Ticket> PmTkts { get; set; }

        public int TktsDueThisWeek { get; set; }
        public int TktsDoneThisWeek { get; set; }
        public Single PercentTktsClosedThisWeek { get; set; }
        public int TktsDueThisMonth { get; set; }
        public int TktsDoneThisMonth { get; set; }
        public Single PercentTktsClosedThisMonth { get; set; }

    }
}