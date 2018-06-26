using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Mvc;

namespace DMS_FEA.Models
{

    // OUSR: User Table
    public partial class OUSR
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "DocNum")]
        public int DocNum { get; set; }

        [Required]
        [MaxLength(25)]
        [Display(Name = "User Name")]
        public string User_Code { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Active")]
        public bool Active { get; set; }

        [ForeignKey("OCOM")]
        [Display(Name = "Company")]
        public int? Company_Code { get; set; }
        public virtual OCOM OCOM { get; set; }

        [ForeignKey("OFOT")]
        [Display(Name = "Department")]
        public int? Department_Code { get; set; }
        public virtual OFOT OFOT { get; set; }

        [Display(Name = "Staff Name")]
        [MaxLength(155)]
        public string U_Name { get; set; }

        [Display(Name = "Display Name")]
        [MaxLength(50)]
        public string Nick_Name { get; set; }

        [MaxLength(1)]
        [Display(Name = "User Role")]
        public string UserRole { get; set; }

        [Display(Name = "Position")]
        [MaxLength(90)]
        public string Position { get; set; }


        [Display(Name = "Email")]
        [DataType(DataType.EmailAddress)]
        public string E_Mail { get; set; }

        [Display(Name = "Create date")]
        [DataType(DataType.DateTime)]
        public DateTime? CreateDate { get; set; }


        [Display(Name = "Created by")]
        public int? CreateSign { get; set;}
                

        [Display(Name = "Update date")]
        [DataType(DataType.DateTime)]
        public DateTime? UpdateDate { get; set; }


        [Display(Name = "Updated by")]
        public int? UpdateSign { get; set; }

        [Display(Name = "Last password change time")]
        [DataType(DataType.DateTime)]
        public DateTime? LstPwdCht { get; set; }


    }


   

    
             
}