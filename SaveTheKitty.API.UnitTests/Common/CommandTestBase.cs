using SaveTheKitty.API.Databases;
using SaveTheKitty.API.UnitTests.Common.Factories;

namespace SaveTheKitty.API.UnitTests.Common;
public class CommandTestBase : IDisposable
{
    private bool _disposed = false;
    protected readonly MainDbContext _db;
    public CommandTestBase()
    {
        _db = MainDbContextMockFactory.Create();
    }
    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {           
            _disposed = true;
        }
    }

    public void Dispose()
    {
        MainDbContextMockFactory.Destroy(_db);
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    ~CommandTestBase()
    {
        Dispose(false);
    }
}