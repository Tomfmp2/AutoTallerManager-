using Domain.Entities;
using Infrastructure.Data.Context;
using Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data;

public static class DbInitializer
{
    public static async Task InitializeAsync(AutoTallerDbContext context)
    {
        await context.Database.MigrateAsync();

        if (!context.Workshops.Any())
        {
            context.Workshops.Add(new Workshop
            {
                Name = "AutoTallerManager Principal",
                Nit = "900.123.456-7",
                BusinessName = "AutoTallerManager S.A.S.",
                Address = "Calle 45 # 12-34 Barrio El Prado",
                City = "Bogotá",
                Phone = "3001234567",
                Email = "info@autotallermanager.com"
            });
            await context.SaveChangesAsync();

        }

        var mainWorkshop = await context.Workshops.FirstAsync();

        if (!context.Roles.Any())
        {
            context.Roles.AddRange(
                new Role { Name = "Admin", Description = "Administrador del sistema con acceso total" },
                new Role { Name = "Mecanico", Description = "Mecánico responsable de reparaciones" },
                new Role { Name = "Recepcionista", Description = "Gestión de clientes, citas y recepción" },
                new Role { Name = "Contador", Description = "Gestión financiera y facturación" }
            );
            await context.SaveChangesAsync();
        }

        var adminRole = await context.Roles.FirstAsync(r => r.Name == "Admin");

        if (!context.OrderStatuses.Any())
        {
            context.OrderStatuses.AddRange(
                new OrderStatus { Name = "Pendiente", IsFinal = false, Description = "Orden registrada, esperando confirmación" },
                new OrderStatus { Name = "Programada", IsFinal = false, Description = "Cita agendada para fecha específica" },
                new OrderStatus { Name = "EnProceso", IsFinal = false, Description = "El vehículo está siendo atendido" },
                new OrderStatus { Name = "EnEsperaPiezas", IsFinal = false, Description = "Esperando llegada de repuestos" },
                new OrderStatus { Name = "Completada", IsFinal = true, Description = "Trabajo técnico finalizado" },
                new OrderStatus { Name = "Entregada", IsFinal = true, Description = "Vehículo entregado al cliente" },
                new OrderStatus { Name = "Cancelada", IsFinal = true, Description = "Orden cancelada por el cliente" }
            );
            await context.SaveChangesAsync();
        }

        if (!context.ServiceTypes.Any())
        {
            context.ServiceTypes.AddRange(
               new ServiceType { Name = "Mantenimiento Preventivo", EstimatedDurationHours = 2.0m, PricePerHour = 45000 },
               new ServiceType { Name = "Cambio de Aceite y Filtro", EstimatedDurationHours = 1.0m, PricePerHour = 40000 },
               new ServiceType { Name = "Alineación y Balanceo", EstimatedDurationHours = 1.5m, PricePerHour = 48000 },
               new ServiceType { Name = "Cambio de Frenos", EstimatedDurationHours = 3.0m, PricePerHour = 52000 },
               new ServiceType { Name = "Diagnóstico General", EstimatedDurationHours = 1.5m, PricePerHour = 35000 },
               new ServiceType { Name = "Reparación Motor", EstimatedDurationHours = 6.0m, PricePerHour = 55000 },
               new ServiceType { Name = "Sistema Eléctrico", EstimatedDurationHours = 3.5m, PricePerHour = 58000 },
               new ServiceType { Name = "Aire Acondicionado", EstimatedDurationHours = 2.5m, PricePerHour = 50000 },
               new ServiceType { Name = "Cambio de Suspensión", EstimatedDurationHours = 4.0m, PricePerHour = 53000 },
               new ServiceType { Name = "Latonería y Pintura", EstimatedDurationHours = 8.0m, PricePerHour = 60000 },
               new ServiceType { Name = "Cambio de Embrague", EstimatedDurationHours = 5.0m, PricePerHour = 55000 },
               new ServiceType { Name = "Revisión Previa a Viaje", EstimatedDurationHours = 2.0m, PricePerHour = 42000 }
           );
            await context.SaveChangesAsync();
        }
        if (!context.PaymentMethods.Any())
        {
            context.PaymentMethods.AddRange(
                new PaymentMethod { Name = "Efectivo" },
                new PaymentMethod { Name = "Tarjeta Crédito" },
                new PaymentMethod { Name = "Tarjeta Débito" },
                new PaymentMethod { Name = "Transferencia Bancaria" },
                new PaymentMethod { Name = "NEQUI" },
                new PaymentMethod { Name = "DaviPlata" },
                new PaymentMethod { Name = "PSE" }
            );
            await context.SaveChangesAsync();
        }
       
        if (!context.AuditActionTypes.Any())
        {
            context.AuditActionTypes.AddRange(
                new AuditActionType { Name = "CREATE" },
                new AuditActionType { Name = "UPDATE" },
                new AuditActionType { Name = "DELETE" },
                new AuditActionType { Name = "CANCEL" },
                new AuditActionType { Name = "STATUS_CHANGE" },
                new AuditActionType { Name = "LOGIN" },
                new AuditActionType { Name = "LOGOUT" },
                new AuditActionType { Name = "PRINT_INVOICE" },
                new AuditActionType { Name = "ASSIGN_MECHANIC" },
                new AuditActionType { Name = "UPLOAD_DOCUMENT" }
            );
            await context.SaveChangesAsync();
        }
        
        if (!context.VehicleBrands.Any())
        {
            context.VehicleBrands.AddRange(
                new VehicleBrand { BrandName = "Toyota" }, new VehicleBrand { BrandName = "Honda" },
                new VehicleBrand { BrandName = "Chevrolet" }, new VehicleBrand { BrandName = "Renault" },
                new VehicleBrand { BrandName = "Kia" }, new VehicleBrand { BrandName = "Mazda" },
                new VehicleBrand { BrandName = "Ford" }, new VehicleBrand { BrandName = "Nissan" },
                new VehicleBrand { BrandName = "Hyundai" }, new VehicleBrand { BrandName = "Volkswagen" },
                new VehicleBrand { BrandName = "Suzuki" }, new VehicleBrand { BrandName = "Fiat" },
                new VehicleBrand { BrandName = "Peugeot" }, new VehicleBrand { BrandName = "BMW" },
                new VehicleBrand { BrandName = "Mercedes Benz" }
            );
            await context.SaveChangesAsync();
        }
        
        if (!context.VehicleColors.Any())
        {
            context.VehicleColors.AddRange(
                new VehicleColor { ColorName = "Blanco" }, new VehicleColor { ColorName = "Negro" },
                new VehicleColor { ColorName = "Rojo" }, new VehicleColor { ColorName = "Azul" },
                new VehicleColor { ColorName = "Gris Plata" }, new VehicleColor { ColorName = "Gris Oscuro" },
                new VehicleColor { ColorName = "Plateado" }, new VehicleColor { ColorName = "Verde" },
                new VehicleColor { ColorName = "Beige" }, new VehicleColor { ColorName = "Naranja" },
                new VehicleColor { ColorName = "Amarillo" }, new VehicleColor { ColorName = "Marrón" },
                new VehicleColor { ColorName = "Bordó" }, new VehicleColor { ColorName = "Blanco Perla" }
            );
            await context.SaveChangesAsync();
        }
        if (!context.PartCategories.Any())
        {
            context.PartCategories.AddRange(
                new PartCategory { Name = "Aceites y Lubricantes" }, new PartCategory { Name = "Filtros" },
                new PartCategory { Name = "Frenos" }, new PartCategory { Name = "Suspensión y Dirección" },
                new PartCategory { Name = "Eléctricos" }, new PartCategory { Name = "Motor" },
                new PartCategory { Name = "Transmisión" }, new PartCategory { Name = "Aire Acondicionado" },
                new PartCategory { Name = "Carrocería" }, new PartCategory { Name = "Luces e Iluminación" },
                new PartCategory { Name = "Neumáticos" }, new PartCategory { Name = "Baterías" }
            );
            await context.SaveChangesAsync();
        }
        
        if (!context.DocumentTypes.Any())
        {
            context.DocumentTypes.AddRange(
                new DocumentType { Code = "CC", Name = "Cédula de Ciudadanía" },
                new DocumentType { Code = "CE", Name = "Cédula de Extranjería" },
                new DocumentType { Code = "NIT", Name = "Número de Identificación Tributaria" },
                new DocumentType { Code = "PAS", Name = "Pasaporte" },
                new DocumentType { Code = "TI", Name = "Tarjeta de Identidad" }
            );
            await context.SaveChangesAsync();
        }
        if (!context.Users.Any())
        {
            var adminPerson = new Person
            {
                FirstName = "Admin",
                LastName = "System"
            };
            context.Persons.Add(adminPerson);
            await context.SaveChangesAsync();

            var passwordHasher = new PasswordHasher();
            var adminUser = new User
            {
                WorkshopId = mainWorkshop.Id,
                PersonId = adminPerson.Id,
               
                PasswordHash = passwordHasher.Hash("Admin123!")
            };
            context.Users.Add(adminUser);
            await context.SaveChangesAsync();
            // Le asignamos el rol
            context.UserRoles.Add(new UserRole
            {
                UserId = adminUser.Id,
                RoleId = adminRole.Id
            });
            await context.SaveChangesAsync();
        }

        // Asegurar que el dominio de email existe
        if (!context.EmailDomains.Any(d => d.Domain == "autotallermanager.com"))
        {
            context.EmailDomains.Add(new EmailDomain { Domain = "autotallermanager.com" });
            await context.SaveChangesAsync();
        }

        // Asegurar que el usuario Admin tenga su correo
        var admin = await context.Users.Include(u => u.Person).FirstOrDefaultAsync();
        if (admin != null && admin.Person != null && !context.PersonEmails.Any(pe => pe.PersonId == admin.PersonId))
        {
            var domain = await context.EmailDomains.FirstAsync(d => d.Domain == "autotallermanager.com");
            context.PersonEmails.Add(new PersonEmail
            {
                PersonId = admin.PersonId,
                EmailDomainId = domain.Id,
                EmailUser = "admin",
                IsPrimary = true
            });
            await context.SaveChangesAsync();
        }
    }
}
