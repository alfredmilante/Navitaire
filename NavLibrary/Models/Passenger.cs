using System;
using System.Collections.Generic;
using System.Text;

namespace NavLibrary.Models
{
    public class Passenger
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime BirthDate { get; set; }
        public int Age
        {
            get
            {
                return ((BirthDate.Date - DateTime.Now.Date).Days) / 365;
            }
        }
    }
}
