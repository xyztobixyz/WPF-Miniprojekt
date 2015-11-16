using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using ch.hsr.wpf.gadgeothek.domain;
using ch.hsr.wpf.gadgeothek.service;

namespace ch.hsr.wpf.gadgeothek.runner
{
    public class SampleUsage
    {
        public string ServerUrl { get; set; }

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="serverUrl">server url (e.g. "http://localhost:8080")</param>
        public SampleUsage(string serverUrl)
        {
            ServerUrl = serverUrl;
        }

        

        /// <summary>
        /// demonstrates how to get the full inventory from the gadgeothek
        /// </summary>
        public void ShowInventory()
        {
            var service = new LibraryAdminService(ServerUrl);

            // no need to login as admin, woohoo :-)

            var gadgets = service.GetAllGadgets();
            PrintAll(gadgets);

            var customers = service.GetAllCustomers();
            PrintAll(customers);

            var reservations = service.GetAllReservations();
            PrintAll(reservations);

            var loans = service.GetAllLoans();
            PrintAll(loans);
        }

        /// <summary>
        /// demonstrates the use of the admin functions to add/remove
        /// new objects to/from the gadgeothek
        /// </summary>
        public void ShowAdminInteraction()
        {            
            var service = new LibraryAdminService(ServerUrl);

            var gadget = new Gadget("XBOX360") { Manufacturer = "Microsoft" };
            if (!service.AddGadget(gadget))
            {
                Console.WriteLine($"{gadget} konnte nicht hinzugefügt werden...");
                return;
            }

            var gadgets = service.GetAllGadgets();
            PrintAll(gadgets, "Gadgets (NEW)");

            gadget.Condition = Condition.Damaged;
            if (!service.UpdateGadget(gadget))
            {
                Console.WriteLine($"{gadget} konnte nicht aktualisiert werden...");
                return;
            }


            gadgets = service.GetAllGadgets();
            PrintAll(gadgets, "Gadgets (NEW 2)");

            if (!service.DeleteGadget(gadget))
            {
                Console.WriteLine($"{gadget} konnte nicht gelöscht werden...");
                return;
            }

            gadgets = service.GetAllGadgets();
            PrintAll(gadgets, "Gadgets (NEW 3)");
        }

        /// <summary>
        /// demonstrates the use of the admin functions to add/remove
        /// new loans and reservations to/from the gadgeothek
        /// (internally a bit more complicated since holding
        /// referenced values)
        /// </summary>
        public void ShowAdminInteractionForLoans()
        {
            var service = new LibraryAdminService(ServerUrl);

            var rnd = new Random();
            var randomId = rnd.Next(100000, 999999).ToString();

            var android = service.GetGadget("26"); // Android2
            var michael = service.GetCustomer("10"); // Michael
            var loan = new Loan(randomId, android, michael, DateTime.Today.AddDays(-1), null);
            if (!service.AddLoan(loan))
            {
                Console.WriteLine($"{loan} konnte nicht hinzugefügt werden werden...");
                return;
            }

            var loans = service.GetAllLoans();
            PrintAll(loans, "Loans (NEW)");

            loan.ReturnDate = DateTime.Now;
            if (!service.UpdateLoan(loan))
            {
                Console.WriteLine($"{loan} konnte nicht aktualisiert werden...");
                return;
            }


            loans = service.GetAllLoans();
            PrintAll(loans, "Loans (NEW 2)"); ;

            if (!service.DeleteLoan(loan))
            {
                Console.WriteLine($"{loan} konnte nicht gelöscht werden...");
                return;
            }

            loans = service.GetAllLoans();
            PrintAll(loans, "Loans (NEW 3)");
        }


        public void ShowAdminInteractionWithGenericInterface<T>(T obj, Func<T,string> getIdFunc, Action<T> adjustAction)
            where T: class, new()
        {
            var service = new LibraryAdminService(ServerUrl);

            if (!service.AddItem(obj))
            {
                Console.WriteLine($"{obj} konnte nicht hinzugefügt werden...");
                return;
            }

            var items = service.GetList<T>(false);
            PrintAll(items, $"{typeof(T).Name}s (NEW)");

            adjustAction(obj);

            if (!service.UpdateItem(obj, getIdFunc(obj)))
            {
                Console.WriteLine($"{obj} konnte nicht aktualisiert werden...");
                return;
            }


            items = service.GetList<T>(false);
            PrintAll(items, $"{typeof(T).Name}s (NEW 2)");

            if (!service.DeleteItem(obj, getIdFunc(obj)))
            {
                Console.WriteLine($"{obj} konnte nicht gelöscht werden...");
                return;
            }

            items = service.GetList<T>(false);
            PrintAll(items, $"{typeof(T).Name}s (NEW 3)");
        }



        /// <summary>
        /// demonstrates the use of the public client api (available
        /// under the /public path on the server) to work with 
        /// the gadgeothek
        /// 
        /// corresponds more or less to the way, the LibraryService
        /// works in Java (see Android part of the lecture)
        /// 
        /// in contrast to the admin api, you need to be logged in
        /// to use the public api.
        /// </summary>
        /// <remarks>if you run this demo usage multiple times you will notice
        /// that the reservation will not work on consecutive calls. This is
        /// because the server does not allow to reserve the same gadget twice
        /// by the same user.</remarks>
        public void ShowUserInteraction()
        {
            var service = new LibraryService(ServerUrl);
            if (!service.Register("matt.muster@hsr.ch", "12345", "Matt", "9919"))
            {
                Console.WriteLine("Sorry, registration did not work...");
                return;
            }
            if (!service.Login("matt.muster@hsr.ch", "12345"))
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


        public void PrintAll<T>(List<T> list, string title = null)
        {
            var typeName = typeof(T).Name;
            title = title ?? $"{typeName}s:"; // use type name with an added 's' as the default title if none given

            Console.WriteLine($"{title}:");
            Console.WriteLine(new string('=', title.Length+1));
            list.ForEach(x => Console.WriteLine(x));
            Console.WriteLine();
        }
    }
}
