using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using Foolproof;

namespace DMS_FEA.ViewModels
{
    public class vmCreateSOBarCodeSheet
    {
        [Range(typeof(int), "0", "99999999", ErrorMessage = "SO No must between 0 and 99999999.")]
        [Display(Name = "Start")]
        [Required(ErrorMessage = "Enter SO Number, please.")]
        public int SO_No_Start { get; set; }

        [Range(typeof(int), "1", "99999999", ErrorMessage = "SO No must between 1 and 99999999.")]
        [GreaterThanOrEqualTo("SO_No_Start")]
        [Display(Name = "End")]
        public int SO_No_End { get; set; }

        [Range(typeof(int), "1", "99", ErrorMessage = "Revision No must between 1 and 99.")]
        [Display(Name = "Revision No. (R)")]
        public int? Revision_No { get; set; }

    }
}