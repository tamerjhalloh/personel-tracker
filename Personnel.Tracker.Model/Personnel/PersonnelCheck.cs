using Personnel.Tracker.Model.Base;
using System;

namespace Personnel.Tracker.Model.Personnel
{
    public class PersonnelCheck : BaseModel, IEntity
    {
        public Guid PersonnelCheckId { get; set; }
        public Guid PersonnelId { get; set; }
        public PersonnelCheckType PersonnelCheckType { get; set; }
    }
}
