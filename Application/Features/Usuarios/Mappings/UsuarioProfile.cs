using AutoMapper;
using Application.Features.Usuarios.DTOs;
using Domain.Entities;

namespace Application.Features.Usuarios.Mappings;

public class UsuarioProfile : Profile
{
    public UsuarioProfile()
    {
        CreateMap<User, UsuarioDto>()
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src =>
                src.Person != null && src.Person.Emails.Any()
                    ? $"{src.Person.Emails.First().EmailUser}@{(src.Person.Emails.First().EmailDomain != null ? src.Person.Emails.First().EmailDomain!.Domain : string.Empty)}"
                    : string.Empty))
            .ForMember(dest => dest.RoleId, opt => opt.MapFrom(src =>
                src.UserRoles.Any() ? src.UserRoles.First().RoleId : 0))
            .ForMember(dest => dest.RoleNombre, opt => opt.MapFrom(src =>
                src.UserRoles.Any() && src.UserRoles.First().Role != null
                    ? src.UserRoles.First().Role!.Name
                    : string.Empty));
    }
}
