using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Mvc;

namespace DMS_FEA.Models
{
    public partial class ODEP
    {

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOVerridableMethodsInConstructors")]
        public ODEP()
        {
            this.OUSRs = new HashSet<OUSR>();
        }


        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int DocNum { get; set; }

        [Required]
        [Remote("DeptCodeValidate", "Department", HttpMethod ="POST",ErrorMessage ="Department Code has been taken. Please enter a new department code!")]
        [Display(Name = "Department code")]
        [MaxLength(10)]
        public string Dept_Code { get; set; }


        [MaxLength(15)]
        public string Fname { get; set; }

        [MaxLength(50)]
        public string Dept_Name { get; set; }

        [Display(Name = "Active")]
        public bool Active { get; set; }

        [Display(Name = "Create date")]
        [DataType(DataType.DateTime)]
        public Nullable<DateTime> CreateDate { get; set; }


        [Display(Name = "Created by")]
        public Nullable<int> CreateSign { get; set; }

        [Display(Name = "Update date")]
        [DataType(DataType.DateTime)]
        public Nullable<System.DateTime> UpdateDate { get; set; }


        [Display(Name = "Updated by")]
        public Nullable<int> UpdateSign { get; set; }

        public int CompID { get; set; }



        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OUSR> OUSRs { get; set; }
        public virtual OUSR OUSR { get; set; }

    }
}