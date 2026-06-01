using Application.Abstractions;
using Application.Common;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Usuarios.Commands.RegisterGoogle;

public class RegisterGoogleCommand : IRequest<Result<int>>
{
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Phone { get; set; } = null!;
    public DateTime DateOfBirth { get; set; }
}

public class RegisterGoogleCommandHandler : IRequestHandler<RegisterGoogleCommand, Result<int>>
{
    private readonly IApplicationDbContext _db;

    public RegisterGoogleCommandHandler(IApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<Result<int>> Handle(RegisterGoogleCommand request, CancellationToken cancellationToken)
    {
        var emailParts = request.Email.Split('@');
        if (emailParts.Length != 2)
            return Result<int>.Failure("Formato de correo inválido.");

        var emailUser = emailParts[0];
        var domainName = emailParts[1];

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

        // Obtener Taller por defecto
        var defaultWorkshop = await _db.Workshops.FirstOrDefaultAsync(cancellationToken);
        if (defaultWorkshop == null)
            return Result<int>.Failure("No hay un taller configurado en el sistema.");

        // Obtener o crear dominio de correo
        var emailDomain = await _db.EmailDomains.FirstOrDefaultAsync(d => d.Domain == domainName, cancellationToken);
        if (emailDomain == null)
        {
            emailDomain = new EmailDomain { Domain = domainName };
            _db.EmailDomains.Add(emailDomain);
        }

        // Obtener rol de Cliente (o crearlo si no existe por si acaso)
        var clientRole = await _db.Roles.FirstOrDefaultAsync(r => r.Name == "Cliente", cancellationToken);
        if (clientRole == null)
        {
            clientRole = new Role { Name = "Cliente", Description = "Propietario del vehículo" };
            _db.Roles.Add(clientRole);
            await _db.SaveChangesAsync(cancellationToken);
        }

        // Crear persona y teléfono
        var person = new Person
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            DateOfBirth = request.DateOfBirth.ToUniversalTime(), // Guardamos en UTC
            Phone = request.Phone,
            Emails = new List<PersonEmail>
            {
                new PersonEmail { EmailUser = emailUser, EmailDomain = emailDomain, IsPrimary = true }
            }
        };

        _db.Persons.Add(person);

        // Crear registro de Cliente (Customer) asociado a la persona
        var customer = new Customer
        {
            WorkshopId = defaultWorkshop.Id,
            Person = person,
            IsActive = true
        };
        _db.Customers.Add(customer);

        // Crear usuario asociado
        // Generamos un password aleatorio largo ya que el usuario usará Google para iniciar sesión.
        var user = new User
        {
            WorkshopId = defaultWorkshop.Id,
            Person = person,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(Guid.NewGuid().ToString() + Guid.NewGuid().ToString()), 
            IsActive = true,
            UserRoles = new List<UserRole>
            {
                new UserRole { Role = clientRole }
            }
        };

        _db.Users.Add(user);
        await _db.SaveChangesAsync(cancellationToken);

        return Result<int>.Success(user.Id);
    }
}
