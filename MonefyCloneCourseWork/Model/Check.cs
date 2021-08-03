using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonefyCloneCourseWork.Model
{
    public class Check
    {
        public double Money { get; set; }
        public bool IsIncome { get; set; }
        public string Category { get; set; }
        public DateTime Date { get; set; }
    }
}
