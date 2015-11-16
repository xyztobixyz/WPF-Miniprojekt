using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ch.hsr.wpf.gadgeothek.domain
{
    public class Loan
    {

        public string Id { get; set; }
        public string CustomerId { get; set; }
        public Customer Customer { get; set; }
        public string GadgetId { get; set; }
        public Gadget Gadget { get; set; }
        public DateTime? PickupDate { get; set; }
        public DateTime? ReturnDate { get; set; }

        public DateTime? OverDueDate => PickupDate?.AddDays(DaysToReturn);

        public bool WasReturned => ReturnDate.HasValue;


        private static readonly int DaysToReturn = 7;

        // parameterless constructor is needed for automatic json conversion
        public Loan()
        {
        }

        public Loan(String id, Gadget gadget, Customer customer, DateTime? pickupDate, DateTime? returnDate)
        {
            Id = id;
            GadgetId = gadget.InventoryNumber;
            Gadget = gadget;
            CustomerId = customer.Studentnumber;
            Customer = customer;
            PickupDate = pickupDate;
            ReturnDate = returnDate;
        }

        public bool IsLent => ReturnDate == null;

        public bool IsOverdue => IsLent && OverDueDate < DateTime.Now;


        public override string ToString()
        {
            return $"Loan {Id}: {Gadget} from {PickupDate:yyyy-MM-dd} to {ReturnDate:yyyy-MM-dd}";
        }

    }
}
