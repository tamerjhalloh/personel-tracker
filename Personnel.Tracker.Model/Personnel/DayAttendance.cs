using Personnel.Tracker.Model.Base;
using System;

namespace Personnel.Tracker.Model.Personnel
{
    public class DayAttendance
    {
        public DateTime Date { get; set; } 
        public DateTime FirstCheckIn { get; set; } 
        public DateTime LastCheckIn { get; set; } 
        public DateTime FirstCheckOut { get; set; }
        public DateTime LastCheckOut { get; set; } 
        public DateTime LastCheck { get; set; } 
        public PersonnelCheckType LastCheckType { get; set; }
        public Guid PersonnelId { get; set; }
        public Guid Personnel  { get; set; }

    }
}
