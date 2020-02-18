using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StringTemplateExamples
{
    public class User
    {
        public int id; // template can directly access via u.id
        private string name; // template can't access this
        public User(int id, string name) { this.id = id; this.name = name; }
        public bool IsManager() { return true; } // u.manager
        public bool HasParkingSpot() { return true; } // u.parkingSpot
        public string GetName() { return name; } // u.name
        public override string ToString() { return id + ":" + name; } // u
    }
}
