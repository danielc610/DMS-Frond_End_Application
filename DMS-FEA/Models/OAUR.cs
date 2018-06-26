using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace DMS_FEA.Models
{
    public class OAUR
    {
        public int UserID { get; set; }
        public int? Fparent { get; set; }
        public string Fname { get; set; }
        public int? Fchild { get; set; }
        public int? Flevel { get; set; }
        public string Fauto { get; set; }
        public string Fseries{ get; set; }

        public int? Fnumber { get; set; }
        public int? compID { get; set; }
        public DateTime? CreateDate { get; set; }
        public int? CreateSign { get; set; }
        public DateTime? UpdateDate { get; set; }
        public int? UpdateSign { get; set; }
        
        [ForeignKey("compID")]
        public OCOM OCOM { get; set; }

    }
}