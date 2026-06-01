using Application.Abstractions;
using Application.Common;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Usuarios.Commands.Register;

public class RegisterCommand : IRequest<Result<int>>
{
    public int WorkshopId  { get; set; }
    public string RoleName { get; set; } = null!;
    public string FirstName { get; set; } = null!;
    public string LastName  { get; set; } = null!;
    public string Email    { get; set; } = null!;
    public string Password { get; set; } = null!;
    public string? Phone   { get; set; }
}

public class RegisterCommandHandler : IRequestHandler<RegisterCommand, Result<int>>
{
    private readonly IApplicationDbContext _db;

    public RegisterCommandHandler(IApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<Result<int>> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var emailParts = request.Email.Split('@');
        if (emailParts.Length != 2)
            return Result<int>.Failure("Formato de correo inválido.");

        var emailUser   = emailParts[0];
        var domainName  = emailParts[1];

        // Check Email Uniqueness (case-insensitive and ignoring global query filters)
        var domain = await _db.EmailDomains.IgnoreQueryFilters()
            .FirstOrDefaultAsync(d => d.Domain.ToLower() == domainName.ToLower(), cancellationToken);
        if (domain != null)
        {
            var emailExists = await _db.PersonEmails.IgnoreQueryFilters()
                .AnyAsync(e => e.EmailUser.ToLower() == emailUser.ToLower() && e.EmailDomainId == domain.Id, cancellationToken);
            if (emailExists)
                return Result<int>.Failure("correo ya registrado");
        }

        // Check Phone Uniqueness (if phone is provided, ignoring query filters)
        if (!string.IsNullOrWhiteSpace(request.Phone))
        {
            var phoneExists = await _db.Persons.IgnoreQueryFilters()
                .AnyAsync(p => p.Phone == request.Phone, cancellationToken);
            if (phoneExists)
                return Result<int>.Failure("telefono ya registrado");
        }

        var role = await _db.Roles.FirstOrDefaultAsync(r => r.Name == request.RoleName, cancellationToken);
        if (role == null)
            return Result<int>.Failure("El rol especificado no existe.");

        // Cargar emailDomain con seguimiento (tracked) directamente de _db
        var emailDomain = await _db.EmailDomains.FirstOrDefaultAsync(d => d.Domain == domainName, cancellationToken);
        if (emailDomain == null)
        {
            emailDomain = new EmailDomain { Domain = domainName };
            _db.EmailDomains.Add(emailDomain);
        }

        // Cargar phoneCode con seguimiento (tracked) directamente de _db
        var phoneCode = await _db.PhoneCodes.FirstOrDefaultAsync(p => p.Code == "+57", cancellationToken);
        if (phoneCode == null)
        {
            phoneCode = new PhoneCode { Code = "+57", Country = "Colombia" };
            _db.PhoneCodes.Add(phoneCode);
        }

        var person = new Person
        {
            FirstName = request.FirstName,
            LastName  = request.LastName,
            Phone     = request.Phone,
            Emails    = new List<PersonEmail>
            {
                new PersonEmail { EmailUser = emailUser, EmailDomain = emailDomain, IsPrimary = true }
            }
        };
        _db.Persons.Add(person);

        // Si el rol es Cliente, crear automáticamente el registro de Customer
        if (role.Name == "Cliente")
        {
            var customer = new Customer
            {
                WorkshopId = request.WorkshopId,
                Person = person,
                IsActive = true
            };
            _db.Customers.Add(customer);
        }

        var user = new User
        {
            WorkshopId   = request.WorkshopId,
            Person       = person,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            IsActive     = true,
            UserRoles    = new List<UserRole>
            {
                new UserRole { Role = role }
            }
        };
        _db.Users.Add(user);
        
        await _db.SaveChangesAsync(cancellationToken);

        return Result<int>.Success(user.Id);
    }
}
