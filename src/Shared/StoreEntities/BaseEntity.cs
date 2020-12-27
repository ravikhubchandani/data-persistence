using System;

namespace StoreEntities
{
    public abstract class BaseEntity : IEntity
    {
        public int Id { get; set; }
        public bool Deleted { get; set; }
        public DateTime DeletedOn { get; set; }
        public string DeletedBy { get; set; }
        public DateTime UpdatedOn { get; set; }
        public string UpdatedBy { get; set; }
    }
}
