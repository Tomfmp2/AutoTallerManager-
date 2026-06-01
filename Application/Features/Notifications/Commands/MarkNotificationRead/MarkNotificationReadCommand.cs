using Application.Abstractions;
using Application.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

namespace Application.Features.Notifications.Commands.MarkNotificationRead;

public class MarkNotificationReadCommand : IRequest<Result<bool>>
{
    [JsonIgnore]
    public int UserId { get; set; }
    
    public int NotificationId { get; set; }
}

public class MarkNotificationReadCommandHandler : IRequestHandler<MarkNotificationReadCommand, Result<bool>>
{
    private readonly IApplicationDbContext _context;

    public MarkNotificationReadCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<bool>> Handle(MarkNotificationReadCommand request, CancellationToken cancellationToken)
    {
        var notification = await _context.Notifications
            .FirstOrDefaultAsync(n => n.Id == request.NotificationId && n.UserId == request.UserId, cancellationToken);

        if (notification == null) return Result<bool>.Failure("Notificación no encontrada.");

        notification.IsRead = true;
        await _context.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(true);
    }
}
