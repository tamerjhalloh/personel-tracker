using System;
using System.Collections.Generic;
using System.Text;

namespace Personnel.Tracker.Model.Criteria
{
    public class SearchPersonnelCriteria
    {
        public string Search { get; set; } 
        public List<Guid> PersonnelIds { get; set; }
    }
}
