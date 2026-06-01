using AutoMapper;
using Application.Features.Vehiculos.DTOs;
using Domain.Entities;

namespace Application.Features.Vehiculos.Mappings;

public class VehiculoProfile : Profile
{
    public VehiculoProfile()
    {
        CreateMap<Vehicle, VehiculoDto>()
            .ForMember(dest => dest.Anio, opt => opt.MapFrom(src => src.Year))
            .ForMember(dest => dest.Kilometraje, opt => opt.MapFrom(src => src.Mileage))
            .ForMember(dest => dest.Placa, opt => opt.MapFrom(src => src.LicensePlate ?? string.Empty))
            .ForMember(dest => dest.Marca, opt => opt.MapFrom(src =>
                src.Model != null && src.Model.Brand != null ? src.Model.Brand.BrandName : string.Empty))
            .ForMember(dest => dest.Modelo, opt => opt.MapFrom(src =>
                src.Model != null ? src.Model.ModelName : string.Empty))
            .ForMember(dest => dest.Color, opt => opt.MapFrom(src =>
                src.Color != null ? src.Color.ColorName : string.Empty))
            .ForMember(dest => dest.PropietarioActual, opt => opt.MapFrom(src =>
                src.OwnerHistories != null
                    ? src.OwnerHistories
                        .Where(h => h.EndDate == null)
                        .Select(h => h.Customer != null && h.Customer.Person != null
                            ? $"{h.Customer.Person.FirstName} {h.Customer.Person.LastName}".Trim()
                            : null)
                        .FirstOrDefault()
                    : null))
            .ForMember(dest => dest.PropietarioId, opt => opt.MapFrom(src =>
                src.OwnerHistories != null
                    ? src.OwnerHistories
                        .Where(h => h.EndDate == null)
                        .Select(h => (int?)h.CustomerId)
                        .FirstOrDefault()
                    : null))
            .ForMember(dest => dest.TotalOrdenesServicio, opt => opt.MapFrom(src =>
                src.ServiceOrders != null ? src.ServiceOrders.Count : 0));
    }
}
