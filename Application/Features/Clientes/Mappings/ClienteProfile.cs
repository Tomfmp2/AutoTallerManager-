using Application.Features.Clientes.DTOs;
using AutoMapper;
using Domain.Entities;

namespace Application.Features.Clientes.Mappings;

public class ClienteProfile : Profile
{
    public ClienteProfile()
    {
        CreateMap<Customer, ClienteDto>()
            .ForMember(dest => dest.Nombre, opt => opt.MapFrom(src => 
                src.Person != null ? $"{src.Person.FirstName} {src.Person.LastName}".Trim() : string.Empty))
            .ForMember(dest => dest.Telefono, opt => opt.MapFrom(src => 
                src.Person != null && src.Person.Phones.Any() ? src.Person.Phones.FirstOrDefault()!.PhoneNumber : string.Empty))
            .ForMember(dest => dest.Correo, opt => opt.MapFrom(src => 
                src.Person != null && src.Person.Emails.Any() ? src.Person.Emails.FirstOrDefault()!.EmailUser : string.Empty))
            .ForMember(dest => dest.Direccion, opt => opt.MapFrom(src => 
                $"{src.AddressStreet} {src.AddressCity}".Trim()))
            .ForMember(dest => dest.TotalVehiculos, opt => opt.MapFrom(src => 
                src.VehicleOwnerHistories != null ? src.VehicleOwnerHistories.Count : 0));

        // Note: Mapping from DTO to Entity for creation/update will usually be handled 
        // manually in the Command Handlers because it involves creating related entities like Person, Phone, Email.
    }
}
