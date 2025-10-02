using Microsoft.EntityFrameworkCore;
using UserManagementWebapp.Database;

namespace UserManagementWebapp.Services;

public class EmailVerificationCleanupService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private static readonly TimeSpan Interval = TimeSpan.FromMinutes(10);

    public EmailVerificationCleanupService(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(Interval, stoppingToken);
            await CleanupAsync(stoppingToken);
        }
    }

    private async Task CleanupAsync(CancellationToken ct)
    {
        using var scope = _scopeFactory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<UsersDbContext>();
        var threshold = DateTime.UtcNow;

        var expired = await db.EmailVerifications
            .Where(ev => ev.Used || ev.Expiration < threshold)
            .ToListAsync(ct);

        if (expired.Count == 0) return;

        var salts = db.Salts.Where(s => s.Purpose == Data.SaltPurpose.EmailVerification
            && s.EmailVerification != null && expired.Contains(s.EmailVerification));
        
        db.Salts.RemoveRange(salts);
        db.EmailVerifications.RemoveRange(expired);

        await db.SaveChangesAsync(ct);
    }
}
