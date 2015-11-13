using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ch.hsr.wpf.gadgeothek.service
{
    public class LoginToken
    {
        public string CustomerId { get; set; }

        public string SecurityToken { get; set; }
    }
}
