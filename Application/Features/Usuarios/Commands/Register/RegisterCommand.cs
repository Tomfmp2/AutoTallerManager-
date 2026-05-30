using Application.Abstractions;
using Application.Common;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Usuarios.Commands.Register;

public class RegisterCommand : IRequest<Result<int>>
{
    public int WorkshopId  { get; set; }
    public int RoleId      { get; set; }
    public string Nombre   { get; set; } = null!;
    public string Email    { get; set; } = null!;
    public string Password { get; set; } = null!;
}

public class RegisterCommandHandler : IRequestHandler<RegisterCommand, Result<int>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IApplicationDbContext _db;

    public RegisterCommandHandler(IUnitOfWork unitOfWork, IApplicationDbContext db)
    {
        _unitOfWork = unitOfWork;
        _db = db;
    }

    public async Task<Result<int>> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var emailParts = request.Email.Split('@');
        if (emailParts.Length != 2)
            return Result<int>.Failure("Formato de correo inválido.");

        var emailUser   = emailParts[0];
        var domainName  = emailParts[1];

        // Verificar que el email no exista
        var exists = await _db.PersonEmails
            .AnyAsync(e => e.EmailUser == emailUser &&
                           e.EmailDomain != null && e.EmailDomain.Domain == domainName,
                      cancellationToken);
        if (exists)
            return Result<int>.Failure("Ya existe un usuario con ese correo.");

        var emailDomainRepo = _unitOfWork.Repository<EmailDomain>();
        var domains = await emailDomainRepo.GetAsync(d => d.Domain == domainName, cancellationToken: cancellationToken);
        var emailDomain = domains.FirstOrDefault() ?? new EmailDomain { Domain = domainName };
        if (emailDomain.Id == 0)
            await emailDomainRepo.AddAsync(emailDomain, cancellationToken);

        var phoneCodeRepo = _unitOfWork.Repository<PhoneCode>();
        var phoneCodes = await phoneCodeRepo.GetAsync(p => p.Code == "+57", cancellationToken: cancellationToken);
        var phoneCode = phoneCodes.FirstOrDefault() ?? new PhoneCode { Code = "+57", Country = "Colombia" };
        if (phoneCode.Id == 0)
            await phoneCodeRepo.AddAsync(phoneCode, cancellationToken);

        var names     = request.Nombre.Split(' ', 2, StringSplitOptions.RemoveEmptyEntries);
        var firstName = names.Length > 0 ? names[0] : string.Empty;
        var lastName  = names.Length > 1 ? names[1] : string.Empty;

        var person = new Person
        {
            FirstName = firstName,
            LastName  = lastName,
            Emails    = new List<PersonEmail>
            {
                new PersonEmail { EmailUser = emailUser, EmailDomain = emailDomain, IsPrimary = true }
            }
        };
        await _unitOfWork.Repository<Person>().AddAsync(person, cancellationToken);

        var user = new User
        {
            WorkshopId   = request.WorkshopId,
            Person       = person,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            IsActive     = true,
            UserRoles    = new List<UserRole>
            {
                new UserRole { RoleId = request.RoleId }
            }
        };
        await _unitOfWork.Repository<User>().AddAsync(user, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);

        return Result<int>.Success(user.Id);
    }
}
