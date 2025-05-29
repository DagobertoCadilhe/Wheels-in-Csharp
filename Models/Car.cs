using Wheels_in_Csharp.Models;

namespace Wheels_in_Csharp.Models
{
    public class Car : Vehicle
    {
        public string FuelType { get; set; }
        public int Seats { get; set; }
        public string Transmission { get; set; }
        public bool HasAC { get; set; }

        public override decimal CalculateRentalCost(DateTime startTime, DateTime endTime)
        {
            var hours = (endTime - startTime).TotalHours;
            return HourlyRate * (decimal)hours * 1.2m;
        }
    }
}