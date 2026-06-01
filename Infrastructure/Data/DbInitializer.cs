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
                new Role { Name = "Contador", Description = "Gestión financiera y facturación" },
                new Role { Name = "Cliente", Description = "Propietario del vehículo" }
            );
            await context.SaveChangesAsync();
        }

        var adminRole = await context.Roles.FirstAsync(r => r.Name == "Admin");

        var requiredStatuses = new[]
        {
            new OrderStatus { Name = "Pendiente", IsFinal = false, Description = "Orden registrada, esperando confirmación" },
            new OrderStatus { Name = "Programada", IsFinal = false, Description = "Cita agendada para fecha específica" },
            new OrderStatus { Name = "DiagnosticoCompletado", IsFinal = false, Description = "Mecánico ha diagnosticado y enviado tiempo estimado" },
            new OrderStatus { Name = "EsperandoAprobacionCliente", IsFinal = false, Description = "Factura enviada al cliente para aprobación" },
            new OrderStatus { Name = "EsperandoPago", IsFinal = false, Description = "Cliente aprobó el trabajo, esperando confirmación de pago" },
            new OrderStatus { Name = "EnProceso", IsFinal = false, Description = "El vehículo está siendo atendido" },
            new OrderStatus { Name = "EnEsperaPiezas", IsFinal = false, Description = "Esperando llegada de repuestos" },
            new OrderStatus { Name = "Completada", IsFinal = true, Description = "Trabajo técnico finalizado" },
            new OrderStatus { Name = "Entregada", IsFinal = true, Description = "Vehículo entregado al cliente" },
            new OrderStatus { Name = "Cancelada", IsFinal = true, Description = "Orden cancelada por el cliente" },
            new OrderStatus { Name = "Incumplimiento", IsFinal = true, Description = "Cita vencida por incumplimiento (No asistió)" }
        };

        foreach (var rs in requiredStatuses)
        {
            if (!context.OrderStatuses.Any(s => s.Name == rs.Name))
            {
                context.OrderStatuses.Add(rs);
            }
        }
        await context.SaveChangesAsync();

        // Fix any orders stuck in Programada that already have a diagnostic report
        var diagnosticoStatus = await context.OrderStatuses.FirstOrDefaultAsync(s => s.Name == "DiagnosticoCompletado");
        var programadaStatus = await context.OrderStatuses.FirstOrDefaultAsync(s => s.Name == "Programada");
        
        if (diagnosticoStatus != null && programadaStatus != null)
        {
            var reports = await context.ServiceOrderReports
                .Where(r => r.IsDiagnostic)
                .Select(r => r.ServiceOrderId)
                .ToListAsync();

            var stuckOrders = await context.ServiceOrders
                .Where(o => o.OrderStatusId == programadaStatus.Id && reports.Contains(o.Id))
                .ToListAsync();

            foreach (var order in stuckOrders)
            {
                order.OrderStatusId = diagnosticoStatus.Id;
            }
            if (stuckOrders.Any())
            {
                await context.SaveChangesAsync();
            }
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

        // Seed vehicle models per brand
        if (!context.VehicleModels.Any())
        {
            var brands = await context.VehicleBrands.ToListAsync();
            var modelsByBrand = new Dictionary<string, string[]>
            {
                ["Toyota"]        = new[] { "Corolla", "Camry", "Hilux", "RAV4", "Yaris", "Fortuner", "Land Cruiser", "Prius", "Etios", "Avanza" },
                ["Honda"]         = new[] { "Civic", "Accord", "CR-V", "HR-V", "Fit", "City", "Pilot", "Odyssey", "Ridgeline", "Jazz" },
                ["Chevrolet"]     = new[] { "Spark", "Onix", "Tracker", "Equinox", "Traverse", "Colorado", "Tahoe", "Captiva", "Cruze", "Blazer" },
                ["Renault"]       = new[] { "Logan", "Sandero", "Duster", "Stepway", "Koleos", "Kwid", "Captur", "Symbol", "Clio", "Megane" },
                ["Kia"]           = new[] { "Picanto", "Rio", "Cerato", "Sportage", "Sorento", "Carnival", "Telluride", "Stinger", "Soul", "Seltos" },
                ["Mazda"]         = new[] { "Mazda2", "Mazda3", "Mazda6", "CX-3", "CX-5", "CX-9", "CX-30", "MX-5", "BT-50", "CX-50" },
                ["Ford"]          = new[] { "Fiesta", "Focus", "Fusion", "Mustang", "Explorer", "Escape", "Ranger", "F-150", "EcoSport", "Edge" },
                ["Nissan"]        = new[] { "March", "Versa", "Sentra", "Altima", "Maxima", "X-Trail", "Pathfinder", "Frontier", "Murano", "Kicks" },
                ["Hyundai"]       = new[] { "i10", "i20", "Accent", "Elantra", "Tucson", "Santa Fe", "Creta", "Palisade", "Kona", "Venue" },
                ["Volkswagen"]    = new[] { "Polo", "Golf", "Jetta", "Passat", "Tiguan", "Touareg", "T-Cross", "T-Roc", "Amarok", "Virtus" },
                ["Suzuki"]        = new[] { "Alto", "Swift", "Baleno", "Vitara", "Jimny", "Ertiga", "Ciaz", "S-Cross", "Grand Vitara", "Ignis" },
                ["Fiat"]          = new[] { "Palio", "Siena", "Uno", "Cronos", "Pulse", "Toro", "Mobi", "Argo", "500", "Doblo" },
                ["Peugeot"]       = new[] { "106", "206", "207", "208", "308", "408", "3008", "5008", "Partner", "Expert" },
                ["BMW"]           = new[] { "Serie 1", "Serie 2", "Serie 3", "Serie 4", "Serie 5", "Serie 7", "X1", "X3", "X5", "X7" },
                ["Mercedes Benz"] = new[] { "Clase A", "Clase C", "Clase E", "Clase S", "GLA", "GLC", "GLE", "GLB", "CLA", "AMG GT" },
            };

            foreach (var brand in brands)
            {
                if (modelsByBrand.TryGetValue(brand.BrandName, out var models))
                {
                    foreach (var modelName in models)
                    {
                        context.VehicleModels.Add(new VehicleModel
                        {
                            BrandId = brand.Id,
                            ModelName = modelName,
                            IsActive = true
                        });
                    }
                }
            }
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

        // ----------------------------------------------------
        // Seeder Extenso de Inventario (Partes / Repuestos)
        // ----------------------------------------------------
        if (!context.Parts.Any())
        {
            var categories = await context.PartCategories.ToDictionaryAsync(c => c.Name, c => c.Id);
            int wid = mainWorkshop.Id;
            
            var catalog = new List<Part>
            {
                // Aceites y Lubricantes
                new Part { Code = "LUB-001", Description = "Aceite Sintético 5W-30 (Galón)", PartBrand = "Mobil 1", UnitPrice = 185000, Stock = 45, MinimumStock = 10, StorageLocation = "A1", PartCategoryId = categories["Aceites y Lubricantes"], WorkshopId = wid },
                new Part { Code = "LUB-002", Description = "Aceite Semi-Sintético 10W-40 (Galón)", PartBrand = "Castrol", UnitPrice = 135000, Stock = 30, MinimumStock = 10, StorageLocation = "A1", PartCategoryId = categories["Aceites y Lubricantes"], WorkshopId = wid },
                new Part { Code = "LUB-003", Description = "Aceite Mineral 20W-50 (Cuarto)", PartBrand = "Motul", UnitPrice = 25000, Stock = 100, MinimumStock = 20, StorageLocation = "A2", PartCategoryId = categories["Aceites y Lubricantes"], WorkshopId = wid },
                new Part { Code = "LUB-004", Description = "Líquido de Frenos DOT 4 (500ml)", PartBrand = "Bosch", UnitPrice = 28000, Stock = 60, MinimumStock = 15, StorageLocation = "A2", PartCategoryId = categories["Aceites y Lubricantes"], WorkshopId = wid },
                new Part { Code = "LUB-005", Description = "Refrigerante Anticongelante Verde (Galón)", PartBrand = "Prestone", UnitPrice = 45000, Stock = 50, MinimumStock = 15, StorageLocation = "A3", PartCategoryId = categories["Aceites y Lubricantes"], WorkshopId = wid },

                // Filtros
                new Part { Code = "FIL-001", Description = "Filtro de Aceite Metálico Estándar", PartBrand = "Fram", UnitPrice = 22000, Stock = 120, MinimumStock = 30, StorageLocation = "B1", PartCategoryId = categories["Filtros"], WorkshopId = wid },
                new Part { Code = "FIL-002", Description = "Filtro de Aire de Cabina (Polen)", PartBrand = "K&N", UnitPrice = 45000, Stock = 40, MinimumStock = 10, StorageLocation = "B1", PartCategoryId = categories["Filtros"], WorkshopId = wid },
                new Part { Code = "FIL-003", Description = "Filtro de Combustible en Línea", PartBrand = "Motorcraft", UnitPrice = 35000, Stock = 55, MinimumStock = 15, StorageLocation = "B2", PartCategoryId = categories["Filtros"], WorkshopId = wid },
                new Part { Code = "FIL-004", Description = "Filtro de Aire Motor Seco", PartBrand = "Mahle", UnitPrice = 38000, Stock = 80, MinimumStock = 20, StorageLocation = "B2", PartCategoryId = categories["Filtros"], WorkshopId = wid },

                // Frenos
                new Part { Code = "FRE-001", Description = "Juego Pastillas de Freno Delanteras (Cerámica)", PartBrand = "Brembo", UnitPrice = 160000, Stock = 25, MinimumStock = 8, StorageLocation = "C1", PartCategoryId = categories["Frenos"], WorkshopId = wid },
                new Part { Code = "FRE-002", Description = "Juego Zapatas de Freno Traseras", PartBrand = "Bosch", UnitPrice = 120000, Stock = 20, MinimumStock = 5, StorageLocation = "C1", PartCategoryId = categories["Frenos"], WorkshopId = wid },
                new Part { Code = "FRE-003", Description = "Disco de Freno Ventilado Delantero", PartBrand = "TRW", UnitPrice = 210000, Stock = 15, MinimumStock = 4, StorageLocation = "C2", PartCategoryId = categories["Frenos"], WorkshopId = wid },
                new Part { Code = "FRE-004", Description = "Cilindro Maestro de Freno", PartBrand = "Aisin", UnitPrice = 320000, Stock = 5, MinimumStock = 2, StorageLocation = "C2", PartCategoryId = categories["Frenos"], WorkshopId = wid },

                // Suspensión y Dirección
                new Part { Code = "SUS-001", Description = "Amortiguador Delantero Gas", PartBrand = "Monroe", UnitPrice = 250000, Stock = 16, MinimumStock = 4, StorageLocation = "D1", PartCategoryId = categories["Suspensión y Dirección"], WorkshopId = wid },
                new Part { Code = "SUS-002", Description = "Amortiguador Trasero Aceite", PartBrand = "KYB", UnitPrice = 210000, Stock = 18, MinimumStock = 4, StorageLocation = "D1", PartCategoryId = categories["Suspensión y Dirección"], WorkshopId = wid },
                new Part { Code = "SUS-003", Description = "Terminal de Dirección Izquierdo", PartBrand = "Moog", UnitPrice = 65000, Stock = 30, MinimumStock = 8, StorageLocation = "D2", PartCategoryId = categories["Suspensión y Dirección"], WorkshopId = wid },
                new Part { Code = "SUS-004", Description = "Buje de Tijera Inferior", PartBrand = "Delphi", UnitPrice = 45000, Stock = 50, MinimumStock = 12, StorageLocation = "D2", PartCategoryId = categories["Suspensión y Dirección"], WorkshopId = wid },
                new Part { Code = "SUS-005", Description = "Rodamiento de Rueda Delantero", PartBrand = "SKF", UnitPrice = 140000, Stock = 20, MinimumStock = 5, StorageLocation = "D3", PartCategoryId = categories["Suspensión y Dirección"], WorkshopId = wid },

                // Eléctricos
                new Part { Code = "ELE-001", Description = "Alternador 12V 90A", PartBrand = "Valeo", UnitPrice = 580000, Stock = 4, MinimumStock = 2, StorageLocation = "E1", PartCategoryId = categories["Eléctricos"], WorkshopId = wid },
                new Part { Code = "ELE-002", Description = "Motor de Arranque 1.2kW", PartBrand = "Denso", UnitPrice = 450000, Stock = 6, MinimumStock = 2, StorageLocation = "E1", PartCategoryId = categories["Eléctricos"], WorkshopId = wid },
                new Part { Code = "ELE-003", Description = "Sensor de Oxígeno (Sonda Lambda)", PartBrand = "Bosch", UnitPrice = 185000, Stock = 15, MinimumStock = 4, StorageLocation = "E2", PartCategoryId = categories["Eléctricos"], WorkshopId = wid },
                new Part { Code = "ELE-004", Description = "Bomba de Combustible Eléctrica", PartBrand = "Delphi", UnitPrice = 280000, Stock = 8, MinimumStock = 3, StorageLocation = "E2", PartCategoryId = categories["Eléctricos"], WorkshopId = wid },

                // Motor
                new Part { Code = "MOT-001", Description = "Kit Banda de Distribución + Tensor", PartBrand = "Gates", UnitPrice = 320000, Stock = 10, MinimumStock = 3, StorageLocation = "F1", PartCategoryId = categories["Motor"], WorkshopId = wid },
                new Part { Code = "MOT-002", Description = "Bomba de Agua con Empaque", PartBrand = "Aisin", UnitPrice = 195000, Stock = 12, MinimumStock = 3, StorageLocation = "F1", PartCategoryId = categories["Motor"], WorkshopId = wid },
                new Part { Code = "MOT-003", Description = "Juego de Bujías Iridium (x4)", PartBrand = "NGK", UnitPrice = 150000, Stock = 40, MinimumStock = 10, StorageLocation = "F2", PartCategoryId = categories["Motor"], WorkshopId = wid },
                new Part { Code = "MOT-004", Description = "Bobina de Encendido", PartBrand = "Denso", UnitPrice = 135000, Stock = 25, MinimumStock = 6, StorageLocation = "F2", PartCategoryId = categories["Motor"], WorkshopId = wid },
                new Part { Code = "MOT-005", Description = "Empaque Culata", PartBrand = "Fel-Pro", UnitPrice = 95000, Stock = 15, MinimumStock = 4, StorageLocation = "F3", PartCategoryId = categories["Motor"], WorkshopId = wid },

                // Transmisión
                new Part { Code = "TRA-001", Description = "Kit de Embrague (Prensa, Disco, Balinera)", PartBrand = "LUK", UnitPrice = 650000, Stock = 5, MinimumStock = 2, StorageLocation = "G1", PartCategoryId = categories["Transmisión"], WorkshopId = wid },
                new Part { Code = "TRA-002", Description = "Cilindro Esclavo de Embrague", PartBrand = "Sachs", UnitPrice = 145000, Stock = 8, MinimumStock = 2, StorageLocation = "G1", PartCategoryId = categories["Transmisión"], WorkshopId = wid },
                new Part { Code = "TRA-003", Description = "Aceite de Transmisión Manual 75W-90 (Cuarto)", PartBrand = "Motul", UnitPrice = 42000, Stock = 30, MinimumStock = 10, StorageLocation = "G2", PartCategoryId = categories["Transmisión"], WorkshopId = wid },
                new Part { Code = "TRA-004", Description = "Junta Homocinética (Punta Eje)", PartBrand = "SKF", UnitPrice = 180000, Stock = 12, MinimumStock = 3, StorageLocation = "G2", PartCategoryId = categories["Transmisión"], WorkshopId = wid },

                // Baterías
                new Part { Code = "BAT-001", Description = "Batería Libre Mantenimiento 12V 42Ah", PartBrand = "Mac", UnitPrice = 250000, Stock = 15, MinimumStock = 5, StorageLocation = "H1", PartCategoryId = categories["Baterías"], WorkshopId = wid },
                new Part { Code = "BAT-002", Description = "Batería Alto Desempeño 12V 65Ah", PartBrand = "Bosch", UnitPrice = 380000, Stock = 10, MinimumStock = 3, StorageLocation = "H1", PartCategoryId = categories["Baterías"], WorkshopId = wid },
                new Part { Code = "BAT-003", Description = "Batería Start-Stop AGM 12V 70Ah", PartBrand = "Varta", UnitPrice = 650000, Stock = 4, MinimumStock = 1, StorageLocation = "H2", PartCategoryId = categories["Baterías"], WorkshopId = wid },

                // Neumáticos
                new Part { Code = "NEU-001", Description = "Llanta 185/65 R15 88H", PartBrand = "Michelin", UnitPrice = 320000, Stock = 20, MinimumStock = 8, StorageLocation = "I1", PartCategoryId = categories["Neumáticos"], WorkshopId = wid },
                new Part { Code = "NEU-002", Description = "Llanta 205/55 R16 91V", PartBrand = "Bridgestone", UnitPrice = 410000, Stock = 16, MinimumStock = 8, StorageLocation = "I1", PartCategoryId = categories["Neumáticos"], WorkshopId = wid },
                new Part { Code = "NEU-003", Description = "Llanta SUV 225/65 R17 102T", PartBrand = "Goodyear", UnitPrice = 580000, Stock = 12, MinimumStock = 4, StorageLocation = "I2", PartCategoryId = categories["Neumáticos"], WorkshopId = wid },
                new Part { Code = "NEU-004", Description = "Llanta Todo Terreno 265/70 R16", PartBrand = "BFGoodrich", UnitPrice = 750000, Stock = 8, MinimumStock = 4, StorageLocation = "I2", PartCategoryId = categories["Neumáticos"], WorkshopId = wid },

                // Aire Acondicionado
                new Part { Code = "AIR-001", Description = "Gas Refrigerante R134a (Cilindro 1kg)", PartBrand = "Dupont", UnitPrice = 110000, Stock = 10, MinimumStock = 3, StorageLocation = "J1", PartCategoryId = categories["Aire Acondicionado"], WorkshopId = wid },
                new Part { Code = "AIR-002", Description = "Compresor de Aire Acondicionado", PartBrand = "Denso", UnitPrice = 950000, Stock = 2, MinimumStock = 1, StorageLocation = "J1", PartCategoryId = categories["Aire Acondicionado"], WorkshopId = wid },

                // Luces e Iluminación
                new Part { Code = "LUC-001", Description = "Bombillo Halógeno H4 55/60W", PartBrand = "Osram", UnitPrice = 25000, Stock = 50, MinimumStock = 15, StorageLocation = "K1", PartCategoryId = categories["Luces e Iluminación"], WorkshopId = wid },
                new Part { Code = "LUC-002", Description = "Kit Bombillos LED H7 6000K", PartBrand = "Philips", UnitPrice = 180000, Stock = 20, MinimumStock = 5, StorageLocation = "K1", PartCategoryId = categories["Luces e Iluminación"], WorkshopId = wid },
                new Part { Code = "LUC-003", Description = "Foco Piloto Trasero T20 (x2)", PartBrand = "Bosch", UnitPrice = 15000, Stock = 40, MinimumStock = 10, StorageLocation = "K2", PartCategoryId = categories["Luces e Iluminación"], WorkshopId = wid },

                // Carrocería
                new Part { Code = "CAR-001", Description = "Plumillas Limpiaparabrisas 22\"", PartBrand = "Valeo", UnitPrice = 35000, Stock = 40, MinimumStock = 10, StorageLocation = "L1", PartCategoryId = categories["Carrocería"], WorkshopId = wid },
                new Part { Code = "CAR-002", Description = "Grapas Plásticas Retenedoras (x50)", PartBrand = "Genérico", UnitPrice = 20000, Stock = 30, MinimumStock = 10, StorageLocation = "L1", PartCategoryId = categories["Carrocería"], WorkshopId = wid },
                new Part { Code = "CAR-003", Description = "Espejo Retrovisor Lateral (Cristal)", PartBrand = "Alkar", UnitPrice = 65000, Stock = 15, MinimumStock = 5, StorageLocation = "L2", PartCategoryId = categories["Carrocería"], WorkshopId = wid }
            };

            context.Parts.AddRange(catalog);
            await context.SaveChangesAsync();
        }
    }
}
