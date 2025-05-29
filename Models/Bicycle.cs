using Wheels_in_Csharp.Models;

namespace Wheels_in_Csharp.Models
{
    public class Bicycle : Vehicle
    {
        public string BikeType { get; set; }
        public bool HasHelmet { get; set; }
        public bool HasLock { get; set; }

        public override decimal CalculateRentalCost(DateTime startTime, DateTime endTime)
        {
            var hours = (endTime - startTime).TotalHours;
            return HourlyRate * (decimal)hours;
        }
    }
}