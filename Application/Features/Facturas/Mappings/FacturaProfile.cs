using AutoMapper;
using Application.Features.Facturas.DTOs;
using Domain.Entities;

namespace Application.Features.Facturas.Mappings;

public class FacturaProfile : Profile
{
    public FacturaProfile()
    {
        CreateMap<Invoice, FacturaDto>()
            .ForMember(dest => dest.NumeroFactura,  opt => opt.MapFrom(src => src.InvoiceNumber))
            .ForMember(dest => dest.OrdenServicioId,opt => opt.MapFrom(src => src.ServiceOrderId))
            .ForMember(dest => dest.NumeroOrden,    opt => opt.MapFrom(src => $"OS-{src.ServiceOrderId:D6}"))
            .ForMember(dest => dest.FechaFactura,   opt => opt.MapFrom(src => src.InvoiceDate))
            .ForMember(dest => dest.ManodeObra,     opt => opt.MapFrom(src => src.LaborCost))
            .ForMember(dest => dest.IVA,            opt => opt.MapFrom(src => src.Taxes))
            .ForMember(dest => dest.ClienteNombre,  opt => opt.MapFrom(src =>
                src.ServiceOrder != null &&
                src.ServiceOrder.Vehicle != null &&
                src.ServiceOrder.Vehicle.OwnerHistories.Any()
                    ? src.ServiceOrder.Vehicle.OwnerHistories
                        .Where(h => h.EndDate == null)
                        .Select(h => h.Customer != null && h.Customer.Person != null
                            ? $"{h.Customer.Person.FirstName} {h.Customer.Person.LastName}".Trim()
                            : string.Empty)
                        .FirstOrDefault() ?? string.Empty
                    : string.Empty))
            .ForMember(dest => dest.Vehiculo, opt => opt.MapFrom(src =>
                src.ServiceOrder != null && src.ServiceOrder.Vehicle != null
                    ? src.ServiceOrder.Vehicle.LicensePlate ?? src.ServiceOrder.Vehicle.VIN
                    : string.Empty))
            .ForMember(dest => dest.Detalles, opt => opt.MapFrom(src =>
                src.Details.Select(d => new DetalleOrdenDto
                {
                    RepuestoCodigo      = d.Concept,
                    RepuestoDescripcion = d.Concept,
                    Cantidad            = d.Quantity,
                    PrecioUnitario      = d.UnitPrice,
                    Subtotal            = d.Subtotal
                }).ToList()));
    }
}
