using System;
using System.Collections.Generic;
using System.Text;

namespace Personnel.Tracker.Model.Base
{
    public class BaseModel
    {
        public Guid? CreatorId { get; set; }
        public Guid? UpdaterId { get; set; }
        public DateTime? CreationTime { get; set; }
        public DateTime? UpdateTime { get; set; }
    }
}
