using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DMS_FEA.ViewModels
{
    public class DeptIndexViewModel
    {

        public int Comp_ID { get; set; }

        public string Comp_Code { get; set; }

        [Display(Name = "Company Name")]
        public string Comp_Name { get; set; }

        public string CompFldName { get; set; }


        public int Dept_ID { get; set; }

        public string Dept_Code { get; set; }

        [Display(Name = "Department Name")]
        public string Dept_Name { get; set; }

        public string DeptFldName { get; set; }

        [Display(Name = "Folder Path")]
        public string FolderPath { get; set; }

        [Display(Name = "Active")]
        public bool Active { get; set; }


    }

    public class CreateDeptViewModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int DocNum { get; set; }

        [Required(ErrorMessage = "Please select a company.")]
        [Display(Name = "Company Name")]
        public int Comp_ID { get; set; }

        public IEnumerable<SelectListItem> CompList { get; set; }

        [Required(ErrorMessage = "Please enter a department code.")]
        [Display(Name = "Department Code")]
        [MaxLength(10, ErrorMessage = "Maximum length of department code is 10, please revise.")]
        [Remote("DeptCodeVaildate","Dept", HttpMethod = "POST", AdditionalFields = "CompList", ErrorMessage = "Department code has already assigned. Please enter a new department code.")]
        public string Dept_Code { get; set; }

        [Required(ErrorMessage = "Please enter a department name.")]
        [Display(Name = "Department Name")]
        [MaxLength(50, ErrorMessage ="Maximum length of department name is 50, please revise.")]
        public string Dept_Name { get; set; }

        [Required(ErrorMessage = "Please enter a folder name.")]
        [Display(Name = "Folder Name")]
        [MaxLength(length: 15, ErrorMessage =("Maximum length of folder name is 15, please revise."))]
        [RegularExpression(@"^[a-zA-Z0-9\s\~\`\!\$\%\^\&\(\)\{\}\[\]\;\'\.\,]+$", ErrorMessage = "Special character is not permitted, please revise!")]
        public string ShortName { get; set; }

        [Display(Name = "Active")]
        public bool Active { get; set; }

        public CreateDeptViewModel()
        {
            CompList = new List<SelectListItem>();
        }


    }

    public class UpdateDeptViewModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int DocNum { get; set; }

        [Required(ErrorMessage = "Please select a company.")]
        [Display(Name = "Company Name")]
        public int Comp_ID { get; set; }


        [Required]
        [MaxLength(10, ErrorMessage = "Maximum length of department code is 10 please revise! ")]
        [Display(Name = "Department Code")]
        public string Dept_Code { get; set; }

        [Required]
        [Display(Name = "Department Name")]
        [MaxLength(50, ErrorMessage = "Maximum length of Department Name is 50, please revise.")]
        public string Dept_Name { get; set; }

        [Required(ErrorMessage = "Please Enter Folder Name.")]
        [MaxLength(15, ErrorMessage = "Maximum length of folder name is 15, please revise!")]
        [Display(Name ="Folder Name")]
        [RegularExpression(@"^[a-zA-Z0-9\s\~\`\!\$\%\^\&\(\)\{\}\[\]\;\'\.\,]+$", ErrorMessage = "Special character is not permitted, please revise")]
        public string ShortName { get; set; }

        [Display(Name = "Active")]
        public bool Active { get; set; }

    }
}