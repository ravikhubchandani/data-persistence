using System;

namespace StoreEntities
{
    public interface IEntity
    {
        int Id { get; set; }
        bool Deleted { get; set; }
        DateTime? DeletedOn { get; set; }
        string DeletedBy { get; set; }
        DateTime? UpdatedOn { get; set; }
        string UpdatedBy { get; set; }
        int Version { get; set; }
    }
}
