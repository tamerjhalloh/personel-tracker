using Personnel.Tracker.Model.Base;
using System;

namespace Personnel.Tracker.Model.Setting
{
    public class SystemSetting : BaseModel, IEntity
    {
        public Guid SystemSettingId { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
    }
}
