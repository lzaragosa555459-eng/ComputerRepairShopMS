using ComputerRepairSystem.company.Entities;

using Microsoft.EntityFrameworkCore;

namespace ComputerRepairSystem.company.Data;

public static class TenantDbSeeder
{
    public static async Task SeedAsync(
        TenantDbContext db)
    {
        // ==========================================
        // CHECK EXISTING DATA
        // ==========================================

        if (await db.Customers.AnyAsync())
        {
            return;
        }


        // ==========================================
        // CUSTOMERS
        // ==========================================

        var customers = new List<Customer>
        {
            new Customer
            {
                FirstName = "Juan",
                MiddleName = "Santos",
                LastName = "Dela Cruz",
                Phone = "09171234567",
                Email = "juan.delacruz@email.com",
                Address = "Davao City"
            },

            new Customer
            {
                FirstName = "Maria",
                MiddleName = "Lopez",
                LastName = "Santos",
                Phone = "09181234567",
                Email = "maria.santos@email.com",
                Address = "Bajada, Davao City"
            },

            new Customer
            {
                FirstName = "Carlos",
                LastName = "Reyes",
                Phone = "09191234567",
                Email = "carlos.reyes@email.com",
                Address = "Matina, Davao City"
            },

            new Customer
            {
                FirstName = "Angela",
                MiddleName = "Cruz",
                LastName = "Garcia",
                Phone = "09201234567",
                Email = "angela.garcia@email.com",
                Address = "Ecoland, Davao City"
            },

            new Customer
            {
                FirstName = "Mark",
                LastName = "Villanueva",
                Phone = "09211234567",
                Email = "mark.villanueva@email.com",
                Address = "Toril, Davao City"
            },

            new Customer
            {
                FirstName = "Sofia",
                MiddleName = "Tan",
                LastName = "Lim",
                Phone = "09221234567",
                Email = "sofia.lim@email.com",
                Address = "Lanang, Davao City"
            },

            new Customer
            {
                FirstName = "Daniel",
                LastName = "Mendoza",
                Phone = "09231234567",
                Email = "daniel.mendoza@email.com",
                Address = "Sasa, Davao City"
            },

            new Customer
            {
                FirstName = "Christine",
                MiddleName = "Ramos",
                LastName = "Navarro",
                Phone = "09241234567",
                Email = "christine.navarro@email.com",
                Address = "Agdao, Davao City"
            },

            new Customer
            {
                FirstName = "Robert",
                LastName = "Fernandez",
                Phone = "09251234567",
                Email = "robert.fernandez@email.com",
                Address = "J.P. Laurel Avenue, Davao City"
            },

            new Customer
            {
                FirstName = "Patricia",
                MiddleName = "Diaz",
                LastName = "Morales",
                Phone = "09261234567",
                Email = "patricia.morales@email.com",
                Address = "Mintal, Davao City"
            },

            new Customer
            {
                FirstName = "Kevin",
                LastName = "Torres",
                Phone = "09271234567",
                Email = "kevin.torres@email.com",
                Address = "Buhangin, Davao City"
            },

            new Customer
            {
                FirstName = "Nicole",
                MiddleName = "Flores",
                LastName = "Castillo",
                Phone = "09281234567",
                Email = "nicole.castillo@email.com",
                Address = "Talomo, Davao City"
            },

            new Customer
            {
                FirstName = "Anthony",
                LastName = "Aquino",
                Phone = "09291234567",
                Email = "anthony.aquino@email.com",
                Address = "Poblacion, Davao City"
            },

            new Customer
            {
                FirstName = "Rachel",
                MiddleName = "Gonzales",
                LastName = "Rivera",
                Phone = "09301234567",
                Email = "rachel.rivera@email.com",
                Address = "Calinan, Davao City"
            },

            new Customer
            {
                FirstName = "Michael",
                LastName = "Domingo",
                Phone = "09311234567",
                Email = "michael.domingo@email.com",
                Address = "Bunawan, Davao City"
            },

            new Customer
            {
                FirstName = "Stephanie",
                MiddleName = "Manalo",
                LastName = "Perez",
                Phone = "09321234567",
                Email = "stephanie.perez@email.com",
                Address = "Paquibato, Davao City"
            },

            new Customer
            {
                FirstName = "Joshua",
                LastName = "Salazar",
                Phone = "09331234567",
                Email = "joshua.salazar@email.com",
                Address = "Catalunan Grande, Davao City"
            },

            new Customer
            {
                FirstName = "Jessica",
                MiddleName = "Bautista",
                LastName = "Cabrera",
                Phone = "09341234567",
                Email = "jessica.cabrera@email.com",
                Address = "Catalunan Pequeño, Davao City"
            },

            new Customer
            {
                FirstName = "Ryan",
                LastName = "Marquez",
                Phone = "09351234567",
                Email = "ryan.marquez@email.com",
                Address = "Baliok, Davao City"
            },

            new Customer
            {
                FirstName = "Elizabeth",
                MiddleName = "Dizon",
                LastName = "Santiago",
                Phone = "09361234567",
                Email = "elizabeth.santiago@email.com",
                Address = "Tigatto, Davao City"
            }
        };


        db.Customers.AddRange(customers);

        await db.SaveChangesAsync();


        // ==========================================
        // DEVICES
        // ==========================================

        var devices = new List<Device>
        {
            // Juan
            new Device
            {
                CustomerId = customers[0].CustomerId,
                DeviceType = "Laptop",
                Brand = "Dell",
                Model = "Inspiron 15",
                SerialNumber = "DL-10001",
                DeviceCondition = "Good"
            },

            new Device
            {
                CustomerId = customers[0].CustomerId,
                DeviceType = "Desktop",
                Brand = "Acer",
                Model = "Aspire TC",
                SerialNumber = "AC-10001",
                DeviceCondition = "Fair"
            },

            // Maria
            new Device
            {
                CustomerId = customers[1].CustomerId,
                DeviceType = "Laptop",
                Brand = "Lenovo",
                Model = "ThinkPad E14",
                SerialNumber = "LN-10001",
                DeviceCondition = "Good"
            },

            // Carlos
            new Device
            {
                CustomerId = customers[2].CustomerId,
                DeviceType = "Desktop",
                Brand = "HP",
                Model = "ProDesk 400",
                SerialNumber = "HP-10001",
                DeviceCondition = "Good"
            },

            // Angela
            new Device
            {
                CustomerId = customers[3].CustomerId,
                DeviceType = "Laptop",
                Brand = "ASUS",
                Model = "VivoBook 15",
                SerialNumber = "AS-10001",
                DeviceCondition = "Good"
            },

            // Mark
            new Device
            {
                CustomerId = customers[4].CustomerId,
                DeviceType = "Laptop",
                Brand = "Acer",
                Model = "Aspire 5",
                SerialNumber = "AC-10002",
                DeviceCondition = "Fair"
            },

            new Device
            {
                CustomerId = customers[4].CustomerId,
                DeviceType = "Printer",
                Brand = "Epson",
                Model = "L3210",
                SerialNumber = "EP-10001",
                DeviceCondition = "Good"
            },

            // Sofia
            new Device
            {
                CustomerId = customers[5].CustomerId,
                DeviceType = "Laptop",
                Brand = "Apple",
                Model = "MacBook Air",
                SerialNumber = "AP-10001",
                DeviceCondition = "Good"
            },

            // Daniel
            new Device
            {
                CustomerId = customers[6].CustomerId,
                DeviceType = "Desktop",
                Brand = "Dell",
                Model = "OptiPlex 7090",
                SerialNumber = "DL-10002",
                DeviceCondition = "Good"
            },

            // Christine
            new Device
            {
                CustomerId = customers[7].CustomerId,
                DeviceType = "Laptop",
                Brand = "HP",
                Model = "Pavilion 14",
                SerialNumber = "HP-10002",
                DeviceCondition = "Good"
            },

            new Device
            {
                CustomerId = customers[7].CustomerId,
                DeviceType = "Tablet",
                Brand = "Samsung",
                Model = "Galaxy Tab A8",
                SerialNumber = "SM-10001",
                DeviceCondition = "Fair"
            },

            // Robert
            new Device
            {
                CustomerId = customers[8].CustomerId,
                DeviceType = "Laptop",
                Brand = "MSI",
                Model = "Modern 14",
                SerialNumber = "MS-10001",
                DeviceCondition = "Good"
            },

            // Patricia
            new Device
            {
                CustomerId = customers[9].CustomerId,
                DeviceType = "Desktop",
                Brand = "Lenovo",
                Model = "IdeaCentre 3",
                SerialNumber = "LN-10002",
                DeviceCondition = "Good"
            },

            // Kevin
            new Device
            {
                CustomerId = customers[10].CustomerId,
                DeviceType = "Laptop",
                Brand = "ASUS",
                Model = "TUF Gaming F15",
                SerialNumber = "AS-10002",
                DeviceCondition = "Fair"
            },

            new Device
            {
                CustomerId = customers[10].CustomerId,
                DeviceType = "Desktop",
                Brand = "Custom",
                Model = "Gaming PC",
                SerialNumber = "CU-10001",
                DeviceCondition = "Good"
            },

            // Nicole
            new Device
            {
                CustomerId = customers[11].CustomerId,
                DeviceType = "Laptop",
                Brand = "Lenovo",
                Model = "IdeaPad 3",
                SerialNumber = "LN-10003",
                DeviceCondition = "Good"
            },

            // Anthony
            new Device
            {
                CustomerId = customers[12].CustomerId,
                DeviceType = "Desktop",
                Brand = "HP",
                Model = "EliteDesk 800",
                SerialNumber = "HP-10003",
                DeviceCondition = "Good"
            },

            // Rachel
            new Device
            {
                CustomerId = customers[13].CustomerId,
                DeviceType = "Laptop",
                Brand = "Dell",
                Model = "Latitude 5420",
                SerialNumber = "DL-10003",
                DeviceCondition = "Good"
            },

            // Michael
            new Device
            {
                CustomerId = customers[14].CustomerId,
                DeviceType = "Printer",
                Brand = "Brother",
                Model = "DCP-T520W",
                SerialNumber = "BR-10001",
                DeviceCondition = "Good"
            },

            // Stephanie
            new Device
            {
                CustomerId = customers[15].CustomerId,
                DeviceType = "Laptop",
                Brand = "Acer",
                Model = "Swift 3",
                SerialNumber = "AC-10003",
                DeviceCondition = "Good"
            },

            // Joshua
            new Device
            {
                CustomerId = customers[16].CustomerId,
                DeviceType = "Desktop",
                Brand = "Dell",
                Model = "Vostro 3910",
                SerialNumber = "DL-10004",
                DeviceCondition = "Fair"
            },

            // Jessica
            new Device
            {
                CustomerId = customers[17].CustomerId,
                DeviceType = "Laptop",
                Brand = "HP",
                Model = "Envy x360",
                SerialNumber = "HP-10004",
                DeviceCondition = "Good"
            },

            // Ryan
            new Device
            {
                CustomerId = customers[18].CustomerId,
                DeviceType = "Laptop",
                Brand = "ASUS",
                Model = "ROG Strix G15",
                SerialNumber = "AS-10003",
                DeviceCondition = "Fair"
            },

            // Elizabeth
            new Device
            {
                CustomerId = customers[19].CustomerId,
                DeviceType = "Desktop",
                Brand = "Acer",
                Model = "Veriton",
                SerialNumber = "AC-10004",
                DeviceCondition = "Good"
            },

            new Device
            {
                CustomerId = customers[19].CustomerId,
                DeviceType = "Laptop",
                Brand = "Dell",
                Model = "Vostro 14",
                SerialNumber = "DL-10005",
                DeviceCondition = "Good"
            }
        };


        db.Devices.AddRange(devices);

        await db.SaveChangesAsync();

        // ==========================================
        // SERVICE REQUESTS
        // ==========================================

        var serviceRequests = new List<ServiceRequest>
        {
            new ServiceRequest
            {
                DeviceId = devices[0].DeviceId,
                RequestDate = DateTime.UtcNow.AddDays(-5),
                Description = "Laptop suddenly shuts down while in use.",
                Status = "Pending",
                Priority = "High"
            },

            new ServiceRequest
            {
                DeviceId = devices[1].DeviceId,
                RequestDate = DateTime.UtcNow.AddDays(-4),
                Description = "Desktop is running slowly and freezes occasionally.",
                Status = "Pending",
                Priority = "Medium"
            },

            new ServiceRequest
            {
                DeviceId = devices[2].DeviceId,
                RequestDate = DateTime.UtcNow.AddDays(-3),
                Description = "Laptop battery drains very quickly.",
                Status = "Pending",
                Priority = "Medium"
            },

            new ServiceRequest
            {
                DeviceId = devices[3].DeviceId,
                RequestDate = DateTime.UtcNow.AddDays(-6),
                Description = "Desktop does not turn on.",
                Status = "Pending",
                Priority = "Urgent"
            },

            new ServiceRequest
            {
                DeviceId = devices[4].DeviceId,
                RequestDate = DateTime.UtcNow.AddDays(-2),
                Description = "Laptop screen flickers when opening applications.",
                Status = "Pending",
                Priority = "High"
            },

            new ServiceRequest
            {
                DeviceId = devices[5].DeviceId,
                RequestDate = DateTime.UtcNow.AddDays(-7),
                Description = "Laptop keyboard has several non-working keys.",
                Status = "Pending",
                Priority = "Medium"
            },

            new ServiceRequest
            {
                DeviceId = devices[6].DeviceId,
                RequestDate = DateTime.UtcNow.AddDays(-8),
                Description = "Printer is printing with faded and incomplete output.",
                Status = "Pending",
                Priority = "Low"
            },

            new ServiceRequest
            {
                DeviceId = devices[7].DeviceId,
                RequestDate = DateTime.UtcNow.AddDays(-1),
                Description = "Laptop is overheating during normal use.",
                Status = "Pending",
                Priority = "High"
            },

            new ServiceRequest
            {
                DeviceId = devices[8].DeviceId,
                RequestDate = DateTime.UtcNow.AddDays(-4),
                Description = "Desktop randomly restarts.",
                Status = "Pending",
                Priority = "High"
            },

            new ServiceRequest
            {
                DeviceId = devices[9].DeviceId,
                RequestDate = DateTime.UtcNow.AddDays(-3),
                Description = "Laptop trackpad is not responding properly.",
                Status = "Pending",
                Priority = "Medium"
            },

            new ServiceRequest
            {
                DeviceId = devices[10].DeviceId,
                RequestDate = DateTime.UtcNow.AddDays(-9),
                Description = "Tablet is not charging consistently.",
                Status = "Pending",
                Priority = "Medium"
            },

            new ServiceRequest
            {
                DeviceId = devices[11].DeviceId,
                RequestDate = DateTime.UtcNow.AddDays(-2),
                Description = "Laptop has no sound output.",
                Status = "Pending",
                Priority = "Low"
            },

            new ServiceRequest
            {
                DeviceId = devices[12].DeviceId,
                RequestDate = DateTime.UtcNow.AddDays(-5),
                Description = "Desktop takes a long time to boot.",
                Status = "Pending",
                Priority = "Medium"
            },

            new ServiceRequest
            {
                DeviceId = devices[13].DeviceId,
                RequestDate = DateTime.UtcNow.AddDays(-1),
                Description = "Gaming laptop gets very hot and becomes slow.",
                Status = "Pending",
                Priority = "High"
            },

            new ServiceRequest
            {
                DeviceId = devices[14].DeviceId,
                RequestDate = DateTime.UtcNow.AddDays(-6),
                Description = "Gaming PC has no display output.",
                Status = "Pending",
                Priority = "Urgent"
            },

            new ServiceRequest
            {
                DeviceId = devices[15].DeviceId,
                RequestDate = DateTime.UtcNow.AddDays(-3),
                Description = "Laptop Wi-Fi disconnects frequently.",
                Status = "Pending",
                Priority = "Medium"
            },

            new ServiceRequest
            {
                DeviceId = devices[16].DeviceId,
                RequestDate = DateTime.UtcNow.AddDays(-4),
                Description = "Desktop shows a blue screen during startup.",
                Status = "Pending",
                Priority = "High"
            },

            new ServiceRequest
            {
                DeviceId = devices[17].DeviceId,
                RequestDate = DateTime.UtcNow.AddDays(-2),
                Description = "Laptop hinge feels loose and makes a clicking sound.",
                Status = "Pending",
                Priority = "Medium"
            },

            new ServiceRequest
            {
                DeviceId = devices[18].DeviceId,
                RequestDate = DateTime.UtcNow.AddDays(-7),
                Description = "Laptop becomes very slow when multiple applications are open.",
                Status = "Pending",
                Priority = "Medium"
            },

            new ServiceRequest
            {
                DeviceId = devices[19].DeviceId,
                RequestDate = DateTime.UtcNow.AddDays(-1),
                Description = "Printer is not detected by the computer.",
                Status = "Pending",
                Priority = "High"
            }
            };

                db.ServiceRequests.AddRange(serviceRequests);

                await db.SaveChangesAsync();
            }


}