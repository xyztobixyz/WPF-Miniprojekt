using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace ch.hsr.wpf.gadgeothek.domain
{
    public class Gadget
    {

        public string InventoryNumber { get; set; }
        public Condition Condition { get; set; }
        public double Price { get; set; }
        public string Manufacturer { get; set; }
        public string Name { get; set; }


        // parameterless constructor is needed for automatic json conversion
        public Gadget()
        {
        }

        public Gadget(string name)
        {
            Name = name;
            var bits = Guid.NewGuid().ToByteArray().Take(8).ToArray();
            var nr = BitConverter.ToUInt64(bits, 0);
            InventoryNumber = nr.ToString();
            Condition = Condition.New;
        }

        public override int GetHashCode()
        {
            return InventoryNumber?.GetHashCode() ?? 31;
        }

        public override bool Equals(object obj)
        {
            if (this == obj)
                return true;
            if (obj == null)
                return false;
            var other = obj as Gadget;
            if (other == null)
                return false;
            if (InventoryNumber == null)
                return other.InventoryNumber == null;
            return InventoryNumber == other.InventoryNumber;
        }

        public override string ToString()
        {
            return FullDescription();
        }

        public string ShortDescription()
        {
            return $"{Name} [{InventoryNumber}]";
        }

        public string FullDescription()
        {
            return $"{Name} [{InventoryNumber}] by {Manufacturer} - Condition: {Condition.ToString().ToUpper()}";
        }
    }


    
}
