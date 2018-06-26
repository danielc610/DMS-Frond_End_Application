using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace DMS_FEA.Models
{
    public partial class OFOT
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int DocNum { get; set; }

        
        public int? Fparent { get; set; }

        [MaxLength(15)]
        public string Fname { get; set; }
        public int? Fchild { get; set; }
        public int? Flevel { get; set; }
        public string Fauto { get; set; }
        public string Fseries { get; set; }

        public int? Fnumber { get; set; }

        public int? compID { get; set; }

        public int? deptID { get; set; }
        public string Fremarks { get; set; }
        public bool Active { get; set; }
        public int? Owner { get; set; }
        public DateTime? CreateDate { get; set; }
        public int? CreateSign { get; set; }
        public DateTime? UpdateDate { get; set; }
        public int? UpdateSign { get; set; }

        [ForeignKey("compID")]
        public virtual OCOM OCOM { get; set; }

        [ForeignKey("deptID")]
        public virtual ODEP ODEP { get; set; }

        [ForeignKey("Fparent")]
        public virtual OFOT Parent { get; set; }
        public virtual ICollection<OFOT> Childs { get; set; }
    }
}