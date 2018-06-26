using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using DMS_FEA.Models;
using System.Web.Mvc;

namespace DMS_FEA.ViewModels
{

    public class LoginUserViewModel 
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int DocNum { get; set; }

        [Required]
        [Display(Name = "User Name")]
        public string User_Code { get; set; }


        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }
    }

    public class CreateUserViewModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int DocNum { get; set; }

        [Required]
        [MaxLength(25)]
        [Display(Name = "User Name")]
        [Remote("UsercodeValid", "User", HttpMethod = "POST", ErrorMessage = "Usercode already exists. Please enter a different usercode")]
        public string User_Code { get; set; }

        [Required(ErrorMessage = "Please Enter Staff Name.")]
        [Display(Name = "Staff Name")]
        [MaxLength(155)]
        public string U_Name { get; set; }

        [Required(ErrorMessage = "Please Enter Display Name.")]
        [Display(Name = "Display Name")]
        [MaxLength(50)]
        public string Nick_Name { get; set; }


        [Required(ErrorMessage = "Please select a default company.")]
        [Display(Name = "Company")]
        public int Company_Code { get; set; }


        [Required(ErrorMessage = "Please select a default department.")]
        [Display(Name = "Department")]
        public int Department_Code { get; set; }

        [Display(Name = "Position")]
        [MaxLength(90)]
        public string Position { get; set; }

        [Display(Name = "User Role")]
        [Required(ErrorMessage ="User role is required.")]
        public string UserRole { get; set; }

        public IList<SelectListItem> Roles { get; set; }

        [Display(Name = "Email")]
        [DataType(DataType.EmailAddress)]
        public string E_Mail { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [MinLength(6, ErrorMessage ="The minimum length of password is 6. Please revise.")]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        [System.ComponentModel.DataAnnotations.Compare("Password",ErrorMessage = "The password and confirm password do not match, please enter again.")]
        public string ConfirmPassword { get; set; }

        [Display(Name = "Active")]
        public bool Active { get; set; }

        public DateTime? CreateDate { get; set; }

        public int? CreateSign { get; set; }

        public virtual OCOM OCOM { get; set; }


        public CreateUserViewModel()
        {
            Roles = new List<SelectListItem>();
        }

    }

    public class UpdateUserViewModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int DocNum { get; set; }

        [Required]
        [MaxLength(25)]
        [Display(Name ="User Name")]
        public string User_Code { get; set; }

        [Display(Name ="Active")]
        public bool Active { get; set; }

        [Display(Name ="User Role")]
        [Required(ErrorMessage = "User role is required")]
        public string UserRole { get; set; }

        public IList<SelectListItem> Roles { get; set; }

        [Required(ErrorMessage = "Please select a default company.")]
        [Display(Name ="Company")]
        public int Company_Code { get; set; }

        [Required(ErrorMessage = "Please select a default department.")]
        [Display(Name ="Department")]
        public int Department_Code { get; set; }

        [Required(ErrorMessage = "Please enter staff name.")]
        [Display(Name ="Staff Name")]
        [MaxLength(155)]
        public string U_Name { get; set; }

        [Required(ErrorMessage = "Please enter a display name.")]
        [Display(Name ="Display Name")]
        [MaxLength(50)]
        public string Nick_Name { get; set; }

        [Display(Name ="Position")]
        public string Position { get; set; }

        [DataType(DataType.EmailAddress)]
        [Display(Name ="Email")]
        public string E_Mail { get; set; }

        public DateTime? UpdateDate { get; set; }
        public int? UpdateSign { get; set; }

        public virtual OCOM OCOM { get; set; }
    }

    public class ResetPasswordViewModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int DocNum { get; set; }


        [Display(Name = "User Name")]
        public string User_Code { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [MinLength(6, ErrorMessage = "The minimum length of password is 6. Please revise.")]
        [Display(Name ="Password")]
        public string Password { get; set; }
        
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        [System.ComponentModel.DataAnnotations.Compare("Password", ErrorMessage="The password and confirm password do not match, please enter again.")]
        public string ConfirmPassword { get; set; }


    }


    public class UserIndexViewModel
    {
        public int DocNum { get; set; }
        public string User_Code { get; set; }
        public string U_Name { get; set; }
        public string Nick_Name { get; set; }
        public string Comp_Name { get; set; }
        public string Dept_Name { get; set; }
        public string Position { get; set; }
        public string E_Mail { get; set; }
        public bool Active { get; set; }
        public string UserRole { get; set; }

    }

}