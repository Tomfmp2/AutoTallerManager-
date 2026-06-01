using Application.Abstractions;
using Application.Common;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Dashboard.Commands;

public class RegisterClientVehicleCommand : IRequest<Result<int>>
{
    public int UserId { get; set; }
    // Step 1 - Identity
    public int BrandId { get; set; }
    public int ModelId { get; set; }
    public int Year { get; set; }
    public int ColorId { get; set; }
    // Step 2 - Details
    public string FuelType { get; set; } = string.Empty;
    public string BodyType { get; set; } = string.Empty;
    public int Mileage { get; set; }
    // Step 3 - Registration
    public string LicensePlate { get; set; } = string.Empty;
    public string? VIN { get; set; }
    public string? EngineNumber { get; set; }
    public string? Notes { get; set; }
    // Step 4 - Photos (base64 data URLs)
    public List<string> Photos { get; set; } = new();
}

public class RegisterClientVehicleCommandHandler : IRequestHandler<RegisterClientVehicleCommand, Result<int>>
{
    private readonly IApplicationDbContext _db;

    public RegisterClientVehicleCommandHandler(IApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<Result<int>> Handle(RegisterClientVehicleCommand request, CancellationToken cancellationToken)
    {
        // 1. Get user and customer
        var user = await _db.Users
            .Include(u => u.Person)
            .FirstOrDefaultAsync(u => u.Id == request.UserId && u.IsActive, cancellationToken);

        if (user?.Person == null)
            return Result<int>.Failure("Usuario no encontrado.");

        var customer = await _db.Customers
            .FirstOrDefaultAsync(c => c.IsActive && c.PersonId == user.Person.Id, cancellationToken);

        if (customer == null)
        {
            // Auto-create customer profile if user doesn't have one yet
            // This handles users created via admin panel or Google login before auto-creation existed
            customer = new Customer
            {
                WorkshopId = user.WorkshopId,
                PersonId = user.Person.Id,
                IsActive = true
            };
            _db.Customers.Add(customer);
            await _db.SaveChangesAsync(cancellationToken);
        }

        // 2. Validate model belongs to brand
        var model = await _db.VehicleModels
            .FirstOrDefaultAsync(m => m.Id == request.ModelId && m.BrandId == request.BrandId, cancellationToken);

        if (model == null)
            return Result<int>.Failure("El modelo seleccionado no pertenece a la marca indicada.");

        // 3. Validate license plate is not already registered in the same workshop
        if (!string.IsNullOrWhiteSpace(request.LicensePlate))
        {
            var plate = request.LicensePlate.Trim().ToUpper();
            var plateExists = await _db.Vehicles
                .AnyAsync(v => v.LicensePlate == plate && v.WorkshopId == customer.WorkshopId, cancellationToken);

            if (plateExists)
                return Result<int>.Failure($"La matrícula '{plate}' ya está registrada en el sistema.");

            request.LicensePlate = plate;
        }

        // 4. Create vehicle
        var vehicle = new Vehicle
        {
            WorkshopId = customer.WorkshopId,
            ModelId = request.ModelId,
            ColorId = request.ColorId,
            LicensePlate = string.IsNullOrWhiteSpace(request.LicensePlate) ? null : request.LicensePlate,
            VIN = request.VIN?.Trim().ToUpper() ?? string.Empty,
            Year = request.Year,
            Mileage = request.Mileage,
            FuelType = request.FuelType,
            BodyType = request.BodyType,
            EngineNumber = request.EngineNumber?.Trim(),
            Notes = request.Notes?.Trim(),
            IsActive = true
        };

        _db.Vehicles.Add(vehicle);

        // 5. Add ownership record
        var ownership = new VehicleOwnerHistory
        {
            Vehicle = vehicle,
            CustomerId = customer.Id,
            StartDate = DateTime.UtcNow
        };
        _db.VehicleOwnerHistories.Add(ownership);

        // 6. Add photos (max 5)
        var photos = request.Photos.Take(5).ToList();
        for (int i = 0; i < photos.Count; i++)
        {
            if (!string.IsNullOrWhiteSpace(photos[i]))
            {
                _db.VehiclePhotos.Add(new VehiclePhoto
                {
                    Vehicle = vehicle,
                    PhotoData = photos[i],
                    IsPrimary = i == 0,
                    Caption = $"Foto {i + 1}"
                });
            }
        }

        await _db.SaveChangesAsync(cancellationToken);

        return Result<int>.Success(vehicle.Id);
    }
}
