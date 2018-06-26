using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DMS_FEA.ViewModels
{
    public class CreateCompViewModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int DocNum { get; set; }

        [Required(ErrorMessage = "Please enter Company Code with maximum length of 2.")]
        [MaxLength(2, ErrorMessage = "Maximum length of company code is 2, please revise!")]
        [Remote("CompCodeValidate", "Company", HttpMethod ="POST", ErrorMessage = "Company code has already been taken. Please enter a new company code.")]
        [Display(Name = "Company Code")]
        [RegularExpression(@"^[a-zA-Z0-9]+$", ErrorMessage ="Special character is not permitted, please revise.")]
        public string Comp_Code { get; set; }

        [Required]
        [MaxLength(100, ErrorMessage = "Maximum length of company name is 100, please revise.")]
        [Display(Name = "Company Name")]
        public string Comp_Name { get; set; }

        [Required(ErrorMessage = "Please Enter Folder Name with maximum length of 15.")]
        [MaxLength(15,ErrorMessage ="Maximum length of folder is 15, please revise.")]
        [Display(Name = "Folder Name")]
        [RegularExpression(@"^[a-zA-Z0-9\s\~\`\!\$\%\^\&\(\)\{\}\[\]\;\'\.\,]+$", ErrorMessage ="Special character is not permitted, please revise.")]
        public string Fname { get; set; }

        [Display(Name = "Active")]
        public bool Active { get; set; }

    }

    public class UpdateCompViewModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int DocNum { get; set; }

        [Required(ErrorMessage = "Please enter Company Code with maximum length of 2.")]
        [MaxLength(2, ErrorMessage = "Maximum length of company code is 2, please revise!")]
        [Display(Name = "Company Code")]
        [RegularExpression(@"^[a-zA-Z0-9]+$", ErrorMessage = "Special character is not permitted, please revise.")]
        public string Comp_Code { get; set; }

        [Required]
        [MaxLength(100, ErrorMessage = "Maximum length of company name is 100, please revise.")]
        [Display(Name = "Company Name")]
        public string Comp_Name { get; set; }

        [Required(ErrorMessage = "Please Enter Folder Name.")]
        [MaxLength(15, ErrorMessage = "Maximum length of folder is 15, please revise! ")]
        [Display(Name = "Folder Name")]
        [RegularExpression(@"^[a-zA-Z0-9\s\~\`\!\$\%\^\&\(\)\{\}\[\]\;\'\.\,]+$", ErrorMessage = "Special character is not permitted, please revise.")]
        public string Fname { get; set; }

        [Display(Name = "Active")]
        public bool Active { get; set; }
    }
}