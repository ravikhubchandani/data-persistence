using StoreEntities;

namespace Test.NewFolder
{
    public partial class Person : AuditableEntity
    {
        public int Age { get; set; }
    }
}
