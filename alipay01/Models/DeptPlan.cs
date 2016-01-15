using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace alipay01.Models
{
    public class DeptPlan
    {
        public string deptID { get; set; }
        public string deptName { get; set; }

        public string amDoctorList { get; set; }
        public string pmDoctorList { get; set; }
        public DeptPlan()
        {

        }
        public DeptPlan(string deptID, string deptName, string am, string pm)
        {
            this.amDoctorList = am;
            this.pmDoctorList = pm;
            this.deptID = deptID;
            this.deptName = deptName;
        }
    }

    class DeptPlanAjaxBean
    {
        public string msg { get; set; }
        public bool isSuccess { get; set; }
        public Dictionary<string,DeptPlan> dict { get; set; }

    } 
}