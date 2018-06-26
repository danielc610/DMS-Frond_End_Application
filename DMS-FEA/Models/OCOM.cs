using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Mvc;

namespace DMS_FEA.Models
{
    // OCOM: Company table
    public partial class OCOM
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverriableMethodsInConstructors")]
        public OCOM()
        {
            this.OUSRs = new HashSet<OUSR>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int DocNum { get; set; }

        [Required]
        [MaxLength(2)]
        [Display(Name = "Company code")]
        public string Comp_Code { get; set; }

        [MaxLength(100)]
        [Display(Name = "Company name")]
        public string Comp_Name { get; set; }

        [MaxLength(15)]
        [Display(Name = "Folder name")]
        public string Fname { get; set; }

        [Display(Name = "Active")]
        public bool Active { get; set; }

        [Display(Name = "Create date")]
        [DataType(DataType.DateTime)]
        public Nullable<DateTime> CreateDate { get; set; }


        [Display(Name = "Created By")]
        public Nullable<int> CreateSign { get; set; }


        [Display(Name = "Update date")]
        [DataType(DataType.DateTime)]
        public Nullable<System.DateTime> UpdateDate { get; set; }


        [Display(Name = "Updated By")]
        public Nullable<int> UpdateSign { get; set; }


        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OUSR> OUSRs { get; set; }

        




    }
}