using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using DMS_FEA.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace DMS_FEA.ViewModels
{
    public class FolderIndexViewModel
    {
        public string CompName { get; set; }
        public string DeptName { get; set; }
        public string FolderName { get; set; }
        public string SubFolderLv1 { get; set; }
        public string SubFolderLv2 { get; set; }

        public string FolderPath { get; set; }
    }

    public class CreateFolderViewModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int DocNum { get; set; }

        [Required(ErrorMessage = "Please Select a Company")]
        [Display(Name ="Company")]
        public int CompList { get; set; }

        [Required(ErrorMessage = "Please Select a Department")]
        [Display(Name = "Department")]
        public int DeptList { get; set; }

        [Display(Name = "Folder")]
        public int? FolderList { get; set; }

        [RegularExpression(@"^[a-zA-Z0-9\s\~\`\!\$\%\^\&\(\)\{\}\[\]\;\'\.\,]+$", ErrorMessage = "Special character is not permitted, please revise!")]
        [MaxLength(15, ErrorMessage = "Maximum length of folder name is 15, please revise.")]
        public string FolderName { get; set; }


        [Display(Name = "Subfolder Level 1")]
        public int? FolderLv1 { get; set; }


        [RegularExpression(@"^[a-zA-Z0-9\s\~\`\!\$\%\^\&\(\)\{\}\[\]\;\'\.\,]+$", ErrorMessage = "Special character is not permitted, please revise.")]
        [MaxLength(15, ErrorMessage = "Maximum length of folder name is 15, please revise.")]
        public string FolderLv1Name { get; set; }


        [Display(Name = "Subfolder Level 2")]
        public int? FolderLv2 { get; set; }

        [Display(Name = "Subfolder Level 2")]
        [RegularExpression(@"^[a-zA-Z0-9\s\~\`\!\$\%\^\&\(\)\{\}\[\]\;\'\.\,]+$", ErrorMessage = "Special character is not permitted, please revise")]
        [MaxLength(15, ErrorMessage = "Maximum length of folder name is 15, please revise.")]
        public string FolderLv2Name { get; set; }

        public string Fname { get; set; }

        public int Flevel { get; set; }


    }
}