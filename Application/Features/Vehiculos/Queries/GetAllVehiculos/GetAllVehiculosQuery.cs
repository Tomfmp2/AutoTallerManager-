using Application.Common;
using Application.Features.Vehiculos.DTOs;
using MediatR;

namespace Application.Features.Vehiculos.Queries.GetAllVehiculos;

public class GetAllVehiculosQuery : IRequest<Result<PagedResult<VehiculoDto>>>
{
    public int PageNumber { get; set; } = 1;
    public int PageSize   { get; set; } = 10;
    public string? Search { get; set; }   // placa, VIN, o propietario
}
