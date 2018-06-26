using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DMS_FEA.Models;
using System.ComponentModel.DataAnnotations.Schema;
using Foolproof;

namespace DMS_FEA.ViewModels
{
    public class vmCreateBarCodeSheet
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int DocNum { get; set; }

        [Display(Name = "Company")]
        public int CompList { get; set; }

        [Display(Name = "Department")]
        public int DeptList { get; set; }

        [Display(Name = "Folder")]
        public int FolderList { get; set; }

        [Display(Name = "Sub-Folder L1")]
        public int? SubFolderLv1 { get; set; }

        [Display(Name = "Sub-Folder L2")]
        public int? SubFolderLv2 { get; set; }

        [Display(Name = "BarCode Page")]
        public string BC_KeepBarCodePage { get; set; }

        [Display(Name = "File Name")]
        [Required(ErrorMessage = "Enter File Name, please.")]
        [RegularExpression(@"^[a-zA-Z0-9\s\~\`\!\$\%\^\&\(\)\{\}\[\]\;\'\.\,]+$", ErrorMessage = "Special character is not permitted, please revise.")]
        public string BC_File_Name { get; set; }

        [Range(typeof(int), "0", "99999999", ErrorMessage = "Start No must between 0 and 99999999")]
        [Display(Name = "Start")]

        public int? BC_No_Start { get; set; }

        [Range(typeof(int), "1", "99999999", ErrorMessage = "End No must between 1 and 99999999")]
        [GreaterThanOrEqualTo("BC_No_Start",ErrorMessage = "End No must be greater than or equal to Start No.")]
        [Display(Name = "End")]
        public int? BC_No_End { get; set; }

        public string SelectedCompany { get; set; }

        public string SelectedDepartment { get; set; }

        public string SelectedFolder { get; set; }

        public string SelectedSubFoldeLvl1 { get; set; }

        public string SelectedSubFoldeLvl2 { get; set; }


    }
}