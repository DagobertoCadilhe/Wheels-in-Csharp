using Wheels_in_Csharp.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Wheels_in_Csharp.Services.Memory
{
    public class VehicleService : IVehicleService
    {
        private static IList<Vehicle> _vehicles;

        public VehicleService()
        {
            if (_vehicles == null || !_vehicles.Any())
            {
                CarregarListaInicial();
            }
        }

        private void CarregarListaInicial()
        {
            _vehicles = new List<Vehicle>()
            {
                new Car
                {
                    Id = 1,
                    ImagemUri = "/images/honda-civic.webp",
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
                new Motorcycle
                {
                    Id = 2,
                    ImagemUri = "/images/yamaha-MT07-1.webp",
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
                    Id = 3,
                    ImagemUri = "/images/trek-mountain-bike.jpg",
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