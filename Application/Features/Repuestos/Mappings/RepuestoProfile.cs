using AutoMapper;
using Application.Features.Repuestos.DTOs;
using Domain.Entities;

namespace Application.Features.Repuestos.Mappings;

public class RepuestoProfile : Profile
{
    public RepuestoProfile()
    {
        CreateMap<Part, RepuestoDto>()
            .ForMember(dest => dest.Codigo,          opt => opt.MapFrom(src => src.Code))
            .ForMember(dest => dest.Descripcion,     opt => opt.MapFrom(src => src.Description))
            .ForMember(dest => dest.PrecioUnitario,  opt => opt.MapFrom(src => src.UnitPrice))
            .ForMember(dest => dest.Categoria,       opt => opt.MapFrom(src => src.Category != null ? src.Category.Name : string.Empty))
            .ForMember(dest => dest.StockActual,     opt => opt.MapFrom(src => src.Stock))
            .ForMember(dest => dest.StockDisponible, opt => opt.MapFrom(src => src.Stock > src.MinimumStock))
            .ForMember(dest => dest.TotalUtilizaciones, opt => opt.Ignore()); // populated via separate query if needed
    }
}
