using Application.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Notifications.Queries.GetUnreadNotifications;

public class NotificationDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

public class GetUnreadNotificationsQuery : IRequest<List<NotificationDto>>
{
    public int UserId { get; set; }
}

public class GetUnreadNotificationsQueryHandler : IRequestHandler<GetUnreadNotificationsQuery, List<NotificationDto>>
{
    private readonly IApplicationDbContext _context;

    public GetUnreadNotificationsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<NotificationDto>> Handle(GetUnreadNotificationsQuery request, CancellationToken cancellationToken)
    {
        return await _context.Notifications
            .Where(n => n.UserId == request.UserId && !n.IsRead)
            .OrderByDescending(n => n.CreatedAt)
            .Select(n => new NotificationDto
            {
                Id = n.Id,
                Title = n.Title,
                Message = n.Message,
                CreatedAt = n.CreatedAt
            })
            .ToListAsync(cancellationToken);
    }
}
