using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace alipay01.Models
{
    public class PatientInfo01
    {
        public int mrn { set; get; }
        public string pName { set; get; }
        public string pSex { set; get; }
        public int pAge { set; get; }
        public string ageUnit { set; get; }
        public PatientInfo01(int mrn, string pName, string pSex, int age, string ageUnit)
        {
            this.mrn = mrn;
            this.pAge = pAge;
            this.pName = pName;
            this.pSex = pSex;
            this.ageUnit = ageUnit;
        }
    }
}