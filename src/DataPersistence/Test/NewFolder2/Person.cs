using StoreEntities;
using System;

#nullable disable

namespace Test.NewFolder2
{
    public partial class Person : AuditableEntity
    {
        public int Age { get; set; }
    }
}
