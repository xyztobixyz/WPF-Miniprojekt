using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using ch.hsr.wpf.gadgeothek.domain;
using RestSharp;

namespace ch.hsr.wpf.gadgeothek.service
{
    
    public class LibraryService : RestServiceBase
    {

        public bool IsLoggedIn => Token != null;
        

        public LibraryService(string serverUrl) : base(serverUrl)
        {
        }


        public bool Login(string mail, string password)
        {
            var parameter = new Dictionary<string, string>();
            parameter.Add("email", mail);
            parameter.Add("password", password);

            var token = CallRestApi<LoginToken>("/public/login", Method.POST, parameter);
            if (token != null && !string.IsNullOrEmpty(token.SecurityToken)) {
                Token = token;
                return true;
            }
            Token = null;
            return false;
        }

        public bool Logout(string mail, string password)
        {
            CheckToken();
            var parameter = PrepareDictionaryWithToken();

            if (CallRestApi<bool>("/public/logout", Method.POST, parameter))
            {
                Token = null;
                return true;
            }
            return false;
        }

        public bool Register(string mail, string password, string name, string studentenNumber)
        {
            var parameter = new Dictionary<string, string>();
            parameter.Add("email", mail);
            parameter.Add("password", password);
            parameter.Add("name", name);
            parameter.Add("studentnumber", studentenNumber);

            return CallRestApi<bool>("/public/register", Method.POST, parameter);
        }

        public List<Gadget> GetGadgets()
        {
            return GetList<Gadget>(true, "/public/gadgets");
        }

        public List<Loan> GetLoansForCustomer()
        {
            return GetList<Loan>(true, "/public/loans");
        }

        public List<Reservation> GetReservationsForCustomer()
        {
            return GetList<Reservation>(true, "/public/reservations");
        }

        public bool ReserveGadgetForCustomer(Gadget gadget)
        {
            CheckToken();

            var parameter = PrepareDictionaryWithToken();
            parameter.Add("gadgetId", gadget.InventoryNumber);

            return CallRestApi<bool>("/public/reservations", Method.POST, parameter);
        }

        public bool DeleteReservationForCustomer(Reservation reservation)
        {
            CheckToken();

            var parameter = PrepareDictionaryWithToken();
            parameter.Add("id", reservation.Id);

            return CallRestApi<bool>("/public/reservations", Method.DELETE, parameter);
        }

    }
}
