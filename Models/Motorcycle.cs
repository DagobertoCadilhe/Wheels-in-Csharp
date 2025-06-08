namespace Wheels_in_Csharp.Models
{
    public class Motorcycle : Vehicle
    {
        public int EngineCapacity { get; set; }
        public bool HasHelmet { get; set; }

        public override decimal CalculateRentalCost(DateTime startTime, DateTime endTime)
        {
            var hours = (endTime - startTime).TotalHours;
            return HourlyRate * (decimal)hours * 1.1m;
        }
    }
}