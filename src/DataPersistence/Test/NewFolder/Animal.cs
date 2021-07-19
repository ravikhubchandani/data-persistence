using StoreEntities;

#nullable disable

namespace Test.NewFolder
{
    public partial class Animal : AuditableEntity
    {
        public string Name { get; set; }
    }
}
