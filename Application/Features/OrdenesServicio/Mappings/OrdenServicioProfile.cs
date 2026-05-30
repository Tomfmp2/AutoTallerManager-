using AutoMapper;
using Application.Features.OrdenesServicio.DTOs;
using Domain.Entities;

namespace Application.Features.OrdenesServicio.Mappings;

public class OrdenServicioProfile : Profile
{
    public OrdenServicioProfile()
    {
        CreateMap<ServiceOrder, OrdenServicioDto>()
            .ForMember(dest => dest.NumeroOrden,       opt => opt.MapFrom(src => $"OS-{src.Id:D6}"))
            .ForMember(dest => dest.VehicleId,         opt => opt.MapFrom(src => src.VehicleId))
            .ForMember(dest => dest.VehiclePlaca,      opt => opt.MapFrom(src =>
                src.Vehicle != null ? src.Vehicle.LicensePlate ?? src.Vehicle.VIN : string.Empty))
            .ForMember(dest => dest.ClienteNombre,     opt => opt.MapFrom(src =>
                src.Vehicle != null && src.Vehicle.OwnerHistories != null
                    ? src.Vehicle.OwnerHistories
                        .Where(h => h.EndDate == null)
                        .Select(h => h.Customer != null && h.Customer.Person != null
                            ? $"{h.Customer.Person.FirstName} {h.Customer.Person.LastName}".Trim()
                            : string.Empty)
                        .FirstOrDefault() ?? string.Empty
                    : string.Empty))
            .ForMember(dest => dest.TipoServicio,      opt => opt.MapFrom(src =>
                src.ServiceType != null ? src.ServiceType.Name : string.Empty))
            .ForMember(dest => dest.AssignedMechanicId, opt => opt.MapFrom(src => src.MechanicId))
            .ForMember(dest => dest.MecanicoNombre,    opt => opt.MapFrom(src =>
                src.Mechanic != null && src.Mechanic.Person != null
                    ? $"{src.Mechanic.Person.FirstName} {src.Mechanic.Person.LastName}".Trim()
                    : string.Empty))
            .ForMember(dest => dest.Estado,            opt => opt.MapFrom(src => src.OrderStatusId))
            .ForMember(dest => dest.FechaIngreso,      opt => opt.MapFrom(src => src.EntryDate))
            .ForMember(dest => dest.EstimatedDeliveryDate, opt => opt.MapFrom(src =>
                src.EstimatedDeliveryDate ?? DateTime.MinValue))
            .ForMember(dest => dest.MontoEstimado,     opt => opt.MapFrom(src =>
                src.ServiceType != null
                    ? (src.ServiceType.EstimatedDurationHours ?? 0) * src.ServiceType.PricePerHour
                    : 0))
            .ForMember(dest => dest.Descripcion,       opt => opt.MapFrom(src => src.Observations));
    }
}
