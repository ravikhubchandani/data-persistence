using EFCoreConnectorStore;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using Test.NewFolder;

namespace Test
{
    public class ExtendedRepo : GenericRepository<Person>
    {
        public ExtendedRepo(IDbContextFactory dbContextFactory) : base(dbContextFactory)
        {
        }

        public IEnumerable<Person> GetWithDependingObjects()
        {
            using (var dbContext = dbContextFactory.GetDbContext())
            {
                var items = dbContext.Set<Person>();
                    //.Include(x => x.Foo)
                    //.Include(x => x.Bar);

                return items.OrderBy(x => x.Id).ToList();
            }
        }
    }
}
