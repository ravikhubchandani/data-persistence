using StoreEntities;
using System;

#nullable disable

namespace Test.NewFolder2
{
    public partial class Person : BaseEntity
    {
        public DateTime? UpdatedOn { get; set; }
        public string UpdatedBy { get; set; }
        public int Age { get; set; }
        public int Id { get; set; }
        public DateTime? DeletedOn { get; set; }
        public string DeletedBy { get; set; }
        public bool Deleted { get; set; }
        public int Version { get; set; }
    }
}
