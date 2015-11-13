using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ch.hsr.wpf.gadgeothek.service;
using ch.hsr.wpf.gadgeothek.domain;
using System.Diagnostics;

namespace ch.hsr.wpf.gadgeothek
{
    public class SampleUsage
    {
        private void HandleError(string e)
        {
            Console.WriteLine("ERROR: " + e);
        }

        public void ShowInventory()
        {
            var service = new LibraryAdminService("http://localhost:8080");

/*
            if (!service.Login("m@hsr.ch", "12345"))
            {
                Console.WriteLine("Sorry, not authorized");
                return;
            }
*/

            var gadgets = service.GetAllGadgets();
            PrintAll("Gadgets", gadgets);

            var customers = service.GetAllCustomers();
            PrintAll("Customers", customers);

            var reservations = service.GetAllReservations();
            PrintAll("Reservations", reservations);

            var loans = service.GetAllLoans();
            PrintAll("Loans", loans);
        }

        public void ShowAdminInteraction()
        {
            var service = new LibraryAdminService("http://localhost:8080");

            var gadget = new Gadget("XBOX360");
            if (!service.AddGadget(gadget))
            {
                Console.WriteLine($"{gadget} konnte nicht hinzugefügt werden...");
                return;
            }

            var gadgets = service.GetAllGadgets();
            PrintAll("Gadgets (NEW)", gadgets);

        }

        public void ShowUserInteraction()
        {
            var service = new LibraryService("http://localhost:8080");
            if (!service.Register("max.muster@hsr.ch", "12345", "Max", "9918"))
            {
                Console.WriteLine("Sorry, registration did not work...");
                return;
            }
            if (!service.Login("max.muster@hsr.ch", "12345"))
            {
                Console.WriteLine("Sorry, not authorized");
                return;
            }

            var gadgets = service.GetGadgets();
            var myGadget = gadgets.First();

            if (!service.ReserveGadgetForCustomer(myGadget))
            {
                Console.WriteLine("ERROR: Reservation did not work...");
                return;
            }

            var reservations = service.GetReservationsForCustomer();
            if (reservations.Count != 1)
            {
                Console.WriteLine("ERROR: Reservation was not properly registered...");
                return;
            }

            Console.WriteLine("Everything is ok");
        }


        public void PrintAll<T>(string title, List<T> list)
        {
            Console.WriteLine($"{title}:");
            Console.WriteLine(new string('=', title.Length+1));
            list.ForEach(x => Console.WriteLine(x));
            Console.WriteLine();
        }
    }
}
