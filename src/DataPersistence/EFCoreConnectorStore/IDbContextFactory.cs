using Microsoft.EntityFrameworkCore;

namespace EFCoreConnectorStore
{
    public interface IDbContextFactory
    {
        DbContext GetDbContext();
    }
}
