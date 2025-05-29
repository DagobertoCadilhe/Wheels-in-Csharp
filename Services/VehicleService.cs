using Wheels_in_Csharp.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Wheels_in_CSharp.Services.Memory
{
    public class VehicleService : IVehicleService
    {
        private IList<Vehicle> _vehicles;

        public VehicleService()
            => CarregarListaInicial();

        private void CarregarListaInicial()
        {
            _vehicles = new List<Vehicle>()
            {
                new Car
                {
                    Id = 1,
                    Model = "Honda Civic",
                    Description = "Sedã esportivo 2023, automático, ar-condicionado e econômico.",
                    Year = 2023,
                    Available = true,
                    HourlyRate = 25.00m,
                    LicensePlate = "ABC-1234",
                    LastMaintenance = DateTime.Now.AddDays(-30),
                    Status = VehicleStatus.AVAILABLE,
                    FuelType = "Gasoline",
                    Seats = 5,
                    Transmission = "Automatic",
                    HasAC = true
                },
                new Car
                {
                    Id = 2,
                    Model = "Toyota Corolla",
                    Description = "Híbrido 2022, câmbio CVT, tecnologia e conforto premium.",
                    Year = 2022,
                    Available = true,
                    HourlyRate = 22.00m,
                    LicensePlate = "DEF-5678",
                    LastMaintenance = DateTime.Now.AddDays(-15),
                    Status = VehicleStatus.AVAILABLE,
                    FuelType = "Hybrid",
                    Seats = 5,
                    Transmission = "CVT",
                    HasAC = true
                },
                new Motorcycle
                {
                    Id = 3,
                    Model = "Honda CB 600F",
                    Description = "Naked 600cc, com capacete incluso e manutenção recente.",
                    Year = 2021,
                    Available = false,
                    HourlyRate = 15.00m,
                    LicensePlate = "GHI-9012",
                    LastMaintenance = DateTime.Now.AddDays(-60),
                    Status = VehicleStatus.RENTED,
                    EngineCapacity = 600,
                    HasHelmet = true
                },
                new Motorcycle
                {
                    Id = 4,
                    Model = "Yamaha MT-07",
                    Description = "MT-07 2023, 689cc, ágil e com equipamento de segurança.",
                    Year = 2023,
                    Available = true,
                    HourlyRate = 18.00m,
                    LicensePlate = "JKL-3456",
                    LastMaintenance = DateTime.Now.AddDays(-10),
                    Status = VehicleStatus.AVAILABLE,
                    EngineCapacity = 689,
                    HasHelmet = true
                },
                new Bicycle
                {
                    Id = 5,
                    Model = "Trek Mountain Bike",
                    Description = "Bicicleta aro 29 para trilhas, com capacete e cadeado.",
                    Year = 2022,
                    Available = true,
                    HourlyRate = 8.00m,
                    LicensePlate = "N/A",
                    LastMaintenance = DateTime.Now.AddDays(-5),
                    Status = VehicleStatus.AVAILABLE,
                    BikeType = "Mountain",
                    HasHelmet = true,
                    HasLock = true
                },
                new Bicycle
                {
                    Id = 6,
                    Model = "City Cruiser",
                    Description = "Bicicleta urbana 2023, prática e com cadeado.",
                    Year = 2023,
                    Available = true,
                    HourlyRate = 6.00m,
                    LicensePlate = "N/A",
                    LastMaintenance = DateTime.Now.AddDays(-2),
                    Status = VehicleStatus.AVAILABLE,
                    BikeType = "Urban",
                    HasHelmet = false,
                    HasLock = true
                }
            };
        }

        public IList<Vehicle> ObterTodos()
            => _vehicles;

        public Vehicle Obter(int id)
        {
            return _vehicles.SingleOrDefault(item => item.Id == id);
        }

        public void Incluir(Vehicle vehicle)
        {
            var proximoNumero = _vehicles.Max(item => item.Id) + 1;
            vehicle.Id = proximoNumero;
            _vehicles.Add(vehicle);
        }

        public void Alterar(Vehicle vehicle)
        {
            var vehicleEncontrado = Obter(vehicle.Id);
            if (vehicleEncontrado != null)
            {
                vehicleEncontrado.Model = vehicle.Model;
                vehicleEncontrado.Description = vehicle.Description;
                vehicleEncontrado.Year = vehicle.Year;
                vehicleEncontrado.Available = vehicle.Available;
                vehicleEncontrado.HourlyRate = vehicle.HourlyRate;
                vehicleEncontrado.LicensePlate = vehicle.LicensePlate;
                vehicleEncontrado.LastMaintenance = vehicle.LastMaintenance;
                vehicleEncontrado.Status = vehicle.Status;
            }
        }

        public void Excluir(int id)
        {
            var vehicleEncontrado = Obter(id);
            if (vehicleEncontrado != null)
            {
                _vehicles.Remove(vehicleEncontrado);
            }
        }

        public IList<VehicleStatus> ObterTodosStatus()
        {
            return Enum.GetValues<VehicleStatus>().ToList();
        }
    }
}