using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
//可以使用验证注释
using System.ComponentModel.DataAnnotations;


namespace alipay01.Models
{
    public class Recharge
    {
        [Required(ErrorMessage = "必须输入流水号")]
        [StringLength(40,MinimumLength=5)]
        public string flowid { get; set; }

        [Required(ErrorMessage = "必填")]
        public string desc { get; set; }

        [Required(ErrorMessage = "必须输入充值金额")]
        [Range(0, 1000000)]
        [DataType(DataType.Currency)]
        public decimal fee { get; set; }

        public long mrn { get; set; }

    }
}