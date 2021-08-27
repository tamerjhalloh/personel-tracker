using Personnel.Tracker.Model.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace Personnel.Tracker.Model.Personnel
{
    public class Personnel : BaseModel, IEntity
    {
        public Guid PersonnelId { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public DateTime? LastLoginTime { get; set; }
        public PersonnelRole PersonnelRole { get; set; }

        public List<PersonnelCheck> PersonnelChecks { get; set; }
    }
}
