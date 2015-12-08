using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using ch.hsr.wpf.gadgeothek.domain;
using RestSharp;

namespace ch.hsr.wpf.gadgeothek.service
{
    /// <summary>
    /// basic admin interface to the Gadgethek REST API
    /// </summary>
    public class LibraryAdminService : RestServiceBase
    {

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="serverUrl">the server url (incl. port)</param>
        public LibraryAdminService(string serverUrl) : base(serverUrl)
        {
        }

        public List<Customer> GetAllCustomers()
        {
            return GetList<Customer>(false);
        }

        public List<Gadget> GetAllGadgets()
        {
            return GetList<Gadget>(false);
        }

        public List<Reservation> GetAllReservations()
        {
            var list = GetList<Reservation>(false);
            list.ForEach(LoadReservationRefs);
            return list;
        }

        public List<Loan> GetAllLoans()
        {
            var list = GetList<Loan>(false);
            list.ForEach(LoadLoanRefs);
            return list;
        }

        public Gadget GetGadget(string inventoryNumber)
        {
            return GetItem<Gadget>(inventoryNumber);
        }

        public Customer GetCustomer(string studentNumber)
        {
            return GetItem<Customer>(studentNumber);
        }

        public Loan GetLoan(string id)
        {
            var obj = GetItem<Loan>(id);
            if (obj != null)
            {
                LoadLoanRefs(obj);
            }
            return obj;
        }

        public Reservation GetReservation(string id)
        {
            var obj = GetItem<Reservation>(id);
            if (obj != null)
            {
                LoadReservationRefs(obj);
            }
            return obj;
        }



        public bool AddGadget(Gadget obj)
        {
            return AddItem(obj);
        }

        public bool AddCustomer(Customer obj)
        {
            return AddItem(obj);
        }
            
        public bool AddLoan(Loan obj)
        {
            return AddItem(obj);
        }

        public bool AddReservation(Reservation obj)
        {
            return AddItem(obj);
        }


        public bool UpdateGadget(Gadget obj)
        {
            return UpdateItem(obj, obj.InventoryNumber);
        }

        public bool UpdateCustomer(Customer obj)
        {
            return UpdateItem(obj, obj.Studentnumber);
        }

        public bool UpdateLoan(Loan obj)
        {
            return UpdateItem(obj, obj.Id);
        }

        public bool UpdateReservation(Reservation obj)
        {
            return UpdateItem(obj, obj.Id);
        }


        public bool DeleteGadget(Gadget obj)
        {
            return DeleteItem(obj, obj.InventoryNumber);
        }

        public bool DeleteCustomer(Customer obj)
        {
            return DeleteItem(obj, obj.Studentnumber);
        }

        public bool DeleteLoan(Loan obj)
        {
            return DeleteItem(obj, obj.Id);
        }

        public bool DeleteReservation(Reservation obj)
        {
            return DeleteItem(obj, obj.Id);
        }



        private void LoadLoanRefs(Loan x)
        {
            if (!string.IsNullOrEmpty(x.CustomerId))
            {
                x.Customer = GetItem<Customer>(x.CustomerId);
            }
            if (!string.IsNullOrEmpty(x.GadgetId))
            {
                x.Gadget = GetItem<Gadget>(x.GadgetId);
            }
        }

        private void LoadReservationRefs(Reservation x)
        {
            if (!string.IsNullOrEmpty(x.CustomerId))
            {
                x.Customer = GetItem<Customer>(x.CustomerId);
            }
            if (!string.IsNullOrEmpty(x.GadgetId))
            {
                x.Gadget = GetItem<Gadget>(x.GadgetId);
            }
        }

    }
}

