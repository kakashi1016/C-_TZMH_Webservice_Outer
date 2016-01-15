using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dbHelper.DAO
{
    class LabTest
    {
        public string testNote { set; get; }
        public int doDate { set; get; }
        public int sID { set; get; }
        public int orderNo { set; get; }
    }

    class LabTestItem
    {
        public string cName { set; get; }
        public string unit { set; get; }
        public string sValue { set; get; }
        public string state { set; get; }
        public string refference { set; get; }
    }
}
