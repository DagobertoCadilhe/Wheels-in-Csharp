using System.ComponentModel.DataAnnotations.Schema;

namespace Wheels_in_Csharp.Models
{
    public class Rental
    {
        public int Id { get; set; }

        [ForeignKey("ApplicationUser")]
        public string CustomerId { get; set; }
        public ApplicationUser Customer { get; set; }

        public int VehicleId { get; set; }
        public Vehicle RentedVehicle { get; set; }

        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }

        public decimal TotalCost { get; set; }
        public RentalStatus Status { get; set; }

        public int? PaymentId { get; set; }
        public Payment PaymentInfo { get; set; }
    }

    public enum RentalStatus
    {
        ACTIVE,
        COMPLETED,
        CANCELLED
    }
}