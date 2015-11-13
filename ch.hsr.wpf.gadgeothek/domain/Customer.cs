namespace ch.hsr.wpf.gadgeothek.domain
{
    public class Customer
    {
        public string Name { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string Studentnumber { get; set; } // Number with lowercase n to conform to the back end (server.js)

        // parameterless constructor is needed for automatic json conversion
        public Customer()
        {            
        }

        public Customer(string name, string password, string email, string studentNumber)
        {
            Name = name;
            Password = password;
            Email = email;
            Studentnumber = studentNumber;
        }

        public override int GetHashCode()
        {
            return Studentnumber?.GetHashCode() ?? 31;
        }

        public override bool Equals(object obj)
        {
            if (this == obj)
                return true;
            if (obj == null)
                return false;
            var other = obj as Customer;
            if (other == null)
                return false;
            if (Studentnumber == null)
                return other.Studentnumber == null;
            return Studentnumber == other.Studentnumber;
        }


        public override string ToString()
        {
            return $"{Name}<{Email}> [{Studentnumber}]";
        }
    }
}