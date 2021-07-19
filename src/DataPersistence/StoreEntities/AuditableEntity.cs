using System;

namespace StoreEntities
{
    public class AuditableEntity : Entity, IAuditableEntity
    {
        public bool Deleted { get; set; }
        public DateTime? DeletedOn { get; set; }
        public string DeletedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public string UpdatedBy { get; set; }
        public int Version { get; set; }
    }
}
