using Application.Abstractions;
using Application.Common;
using Domain.Entities;
using MediatR;

namespace Application.Features.Clientes.Commands.CreateCliente;

public class CreateClienteCommandHandler : IRequestHandler<CreateClienteCommand, Result<int>>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreateClienteCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<int>> Handle(CreateClienteCommand request, CancellationToken cancellationToken)
    {
        var names = request.Nombre.Split(' ', 2, StringSplitOptions.RemoveEmptyEntries);
        var firstName = names.Length > 0 ? names[0] : string.Empty;
        var lastName = names.Length > 1 ? names[1] : string.Empty;

        var emailParts = request.Correo.Split('@');
        if (emailParts.Length != 2) return Result<int>.Failure("Formato de correo inválido.");

        var emailUser = emailParts[0];
        var domainName = emailParts[1];

        var emailDomainRepo = _unitOfWork.Repository<EmailDomain>();
        var domains = await emailDomainRepo.GetAsync(d => d.Domain == domainName, cancellationToken: cancellationToken);
        var emailDomain = domains.FirstOrDefault();
        if (emailDomain == null)
        {
            emailDomain = new EmailDomain { Domain = domainName };
            await emailDomainRepo.AddAsync(emailDomain, cancellationToken);
        }

        var phoneCodeRepo = _unitOfWork.Repository<PhoneCode>();
        var phoneCodes = await phoneCodeRepo.GetAsync(p => p.Code == "+57", cancellationToken: cancellationToken);
        var phoneCode = phoneCodes.FirstOrDefault();
        if (phoneCode == null)
        {
            phoneCode = new PhoneCode { Code = "+57", Country = "Colombia" };
            await phoneCodeRepo.AddAsync(phoneCode, cancellationToken);
        }

        var person = new Person
        {
            FirstName = firstName,
            LastName = lastName,
            Emails = new List<PersonEmail>
            {
                new PersonEmail
                {
                    EmailUser = emailUser,
                    EmailDomain = emailDomain,
                    IsPrimary = true
                }
            },
            Phones = new List<PersonPhone>
            {
                new PersonPhone
                {
                    PhoneNumber = request.Telefono,
                    PhoneCode = phoneCode,
                    IsPrimary = true
                }
            }
        };

        await _unitOfWork.Repository<Person>().AddAsync(person, cancellationToken);

        var customer = new Customer
        {
            WorkshopId = request.WorkshopId,
            Person = person,
            AddressStreet = request.Direccion,
            IsActive = true
        };

        await _unitOfWork.Repository<Customer>().AddAsync(customer, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);

        return Result<int>.Success(customer.Id);
    }
}
