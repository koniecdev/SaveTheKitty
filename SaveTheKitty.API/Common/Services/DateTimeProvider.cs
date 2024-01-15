using SaveTheKitty.API.Common.Services.Interfaces;

namespace SaveTheKitty.API.Common.Services;

internal sealed class DateTimeProvider : IDateTime
{
    public DateTimeOffset Now => DateTimeOffset.Now;
}
