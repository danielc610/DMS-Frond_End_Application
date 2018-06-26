using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using DMS_FEA.Models;
using System.Threading.Tasks;
using DMS_FEA.ViewModels;
using System.Web.Security;
using System.IO;
using Dapper;
using jsTree3.Models;
using PagedList;

namespace DMS_FEA.Controllers
{
    [Authorize]
    public class FolderController : Controller
    {
        private DMSDBContext db = new DMSDBContext();
        //private string path = @"C:\\DMS\\DMS_DATA\\";

        private string path = @"D:\\DATA\\DMS\\DMS_DATA\\";
        private string networkPath = @"M:\";

        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
            var dict = new System.Collections.Generic.Dictionary<string, string>();
            dict.Add("@fn", "GetFolderList");

            ViewBag.CurrentSort = sortOrder;
            ViewBag.CompNameSortParm = String.IsNullOrEmpty(sortOrder) ? "CompNameDesc" : "";
            ViewBag.DeptNameSortParm = sortOrder == "DeptName" ? "DeptNameDesc" : "DeptName";
            ViewBag.FolderSortParm = sortOrder == "FolderName" ? "FolderNameDesc" : "FolderName";
            ViewBag.SubFldLv1SortParm = sortOrder == "SubFolderLv1" ? "SubFolderLv1Desc" : "SubFolderLv1";
            ViewBag.SubFldLv2SortParm = sortOrder == "SubFolderLv2" ? "SubFolderLv2Desc" : "SubFolderLv2";



            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }
            ViewBag.CurrentFilter = searchString;

            if(!String.IsNullOrEmpty(searchString))
            {
                dict.Add("@searchKey", searchString);
            }

            switch (sortOrder)
            {
                case "CompNameDesc":
                    dict.Add("@sort", "CompNameDesc");
                    break;
                case "DeptName":
                    dict.Add("@sort", "DeptName");
                    break;
                case "DeptNameDesc":
                    dict.Add("@sort", "DeptNameDesc");
                    break;
                case "FolderName":
                    dict.Add("@sort", "FolderName");
                    break;
                case "FolderNameDesc":
                    dict.Add("@sort", "FolderNameDesc");
                    break;
                case "SubFolderLv1":
                    dict.Add("@sort", "SubFolderLv1");
                    break;
                case "SubFolderLv1Desc":
                    dict.Add("@sort", "SubFolderLv1Desc");
                    break;
                case "SubFolderLv2":
                    dict.Add("@sort", "SubFolderLv2");
                    break;
                case "SubFolderLv2Desc":
                    dict.Add("@sort", "SubFolderLv2Desc");
                    break;
                default:
                    dict.Add("@sort", "CompName");
                    break;

            }


            List<FolderIndexViewModel> results = new List<FolderIndexViewModel>();

            try
            {
                
                DataTable dt = ATAT_Lib.ATAT_Lib.GetData("_spFolderManagement", dict);
                

                if (dt.Rows.Count > 0)
                {
                    foreach(DataRow dr in dt.Rows)
                    {
                        results.Add(new FolderIndexViewModel() {
                            CompName = dr["CompName"].ToString(),
                            DeptName = dr["DeptName"].ToString(),
                            FolderName = dr["FolderName"].ToString(),
                            SubFolderLv1 = dr["SubFolderLv1"].ToString(),
                            SubFolderLv2 = dr["SubFolderLv2"].ToString(),
                            FolderPath = dr["FolderPath"].ToString()
                        });
                        
                    }
                }
            }
            catch (Exception ex)
            {
                return View(ex.Message);
            }

            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return View(results.ToPagedList(pageNumber,pageSize));

        }




        public JsonResult GetFoldersData()
        {

            var data = db.OFOTs.Where(x => !x.Fparent.HasValue);


            return Json(data, JsonRequestBehavior.AllowGet);
        }
         
        // GET: Folder/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OFOT oFOT = db.OFOTs.Find(id);
            if (oFOT == null)
            {
                return HttpNotFound();
            }
            return View(oFOT);
        }

        [HttpPost]
        public JsonResult UpdateList(string value, int lv)
        {
            int parentID = Int32.Parse(value);
             
            var list = from u in db.OFOTs
                       where u.Fparent == parentID &&
                       u.Flevel == lv && u.Active == true
                       select new
                       {
                           parID = u.DocNum,
                           name = u.Fname
                       };
            return Json(list.OrderBy(x => x.name), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult UpdateDeptList (string value, int lv)
        {
            int parentID = Int32.Parse(value);

            var list = from u in db.OFOTs
                       join dept in db.ODEPs on u.deptID equals dept.DocNum
                       where u.Fparent == parentID && u.Flevel == 2 && dept.Active == true
                       select new 
                       {
                           parID = u.DocNum,
                           name = dept.Dept_Name
                       };
            return Json(list.OrderBy(x => x.name), JsonRequestBehavior.AllowGet);
        }


        //[Authorize]
        // GET: Folder/Create
        public ActionResult Create()
        {

            IEnumerable<SelectListItem> compList = from u in db.OFOTs
                                                   join comp in db.OCOMs on u.compID equals comp.DocNum
                                                   where (u.Flevel == 1 && comp.Active == true)
                                                   select new SelectListItem
                                                   {
                                                       Text = comp.Comp_Name,
                                                       Value = u.DocNum.ToString()
                                                   };

            IEnumerable<SelectListItem> deptList = from u in db.OFOTs
                                                   join dept in db.ODEPs on u.deptID equals dept.DocNum
                                                   where (u.Flevel == 2 && dept.Active == true)
                                                      select new SelectListItem
                                                      {
                                                          Text = dept.Dept_Name,
                                                          Value = u.DocNum.ToString()
                                                      };
            IEnumerable<SelectListItem> folderList = from u in db.OFOTs.Where(u => u.Flevel == 3)
                                                     select new SelectListItem
                                                     {
                                                         Text = u.Fname,
                                                         Value = u.DocNum.ToString()
                                                     };
            IEnumerable<SelectListItem> subfldlv1 = from u in db.OFOTs.Where(u => u.Flevel == 4)
                                                    select new SelectListItem
                                                    {
                                                        Text = u.Fname,
                                                        Value = u.DocNum.ToString()
                                                    };
           
            CreateFolderViewModel viewModel = new CreateFolderViewModel();

            ViewBag.CompList = compList.OrderBy(x => x.Text);
            ViewBag.DeptList = deptList.OrderBy(x => x.Text);
            ViewBag.FolderList = folderList.OrderBy(x => x.Text);
            ViewBag.FolderLv1 = subfldlv1.OrderBy(x => x.Text);

            return View(viewModel);
        }

        // POST: Folder/Create
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CreateFolderViewModel model, string btnSubmit)
        {
            IEnumerable<SelectListItem> compList = from u in db.OFOTs
                                                   join comp in db.OCOMs on u.compID equals comp.DocNum
                                                   where (u.Flevel == 1 && comp.Active == true)
                                                   select new SelectListItem
                                                   {
                                                       Text = comp.Comp_Name,
                                                       Value = u.DocNum.ToString()
                                                   };

            IEnumerable<SelectListItem> deptList = from u in db.OFOTs
                                                   join dept in db.ODEPs on u.deptID equals dept.DocNum
                                                   where (u.Flevel == 2 && dept.Active == true)
                                                   select new SelectListItem
                                                   {
                                                       Text = dept.Dept_Name,
                                                       Value = u.DocNum.ToString()
                                                   };
            IEnumerable<SelectListItem> folderList = from u in db.OFOTs.Where(u => u.Flevel == 3)
                                                     select new SelectListItem
                                                     {
                                                         Text = u.Fname,
                                                         Value = u.DocNum.ToString()
                                                     };
            IEnumerable<SelectListItem> subfldlv1 = from u in db.OFOTs.Where(u => u.Flevel == 4)
                                                    select new SelectListItem
                                                    {
                                                        Text = u.Fname,
                                                        Value = u.DocNum.ToString()
                                                    };

            CreateFolderViewModel viewModel = new CreateFolderViewModel();

            ViewBag.CompList = compList.OrderBy(x => x.Text);
            ViewBag.DeptList = deptList.OrderBy(x => x.Text);
            ViewBag.FolderList = folderList.OrderBy(x => x.Text);
            ViewBag.FolderLv1 = subfldlv1.OrderBy(x => x.Text);

            switch (btnSubmit)
            {
                case "Create":
                    //int parentID = 0;
                    if (ModelState.IsValid)
                    {

                        var folderPath = "";
                        var displayPath = "";

                        var compName = (from n in db.OFOTs
                                        where n.DocNum == model.CompList
                                        select n.Fname).First().ToString();

                        var deptName = "";
                        var FolderName = "";
                        var SubfolderLv1 = "";


                        // check if create subfolder level 2
                        if (model.FolderLv2Name == null)
                        {
                            
                            // Folder is selected and SubFldLv1Name is entered
                            if ((model.FolderLv1 == 0) && (model.FolderLv1Name != null))
                            {
                                FolderName = (from n in db.OFOTs
                                              where n.DocNum == model.FolderList
                                              select n.Fname).First().ToString();

                                deptName = (from n in db.OFOTs
                                            where n.DocNum == model.DeptList
                                            select n.Fname).First().ToString();

                                folderPath = path + compName + "\\" + deptName + "\\" + FolderName + "\\" + model.FolderLv1Name;

                                displayPath = networkPath + compName + "\\" + deptName + "\\" + FolderName + "\\" + model.FolderLv1Name;

                                //Check if Sub-Folder Lv1 Exists
                                if (!Directory.Exists(folderPath)){

                                    DirectoryInfo di = Directory.CreateDirectory(folderPath);

                                    CreateFolder(model.FolderLv1Name, model.FolderList, 4, int.Parse(getUserData("id")));

                                    ViewBag.Message = String.Format("Folder {0} is created under {1} ", model.FolderLv1Name, displayPath);

                                    return View("Create");

                                }
                                else
                                {
                                    ViewBag.Message = String.Format("Folder {0} already exists, please choose another folder name.", model.FolderLv1Name);
                                    return View("Create");
                                }

                            }
                            else if ((model.FolderList != 0) && (model.FolderLv1 != 0))
                            {
                                deptName = (from n in db.OFOTs
                                            where n.DocNum == model.DeptList
                                            select n.Fname).First().ToString();
                                FolderName = (from n in db.OFOTs
                                              where n.DocNum == model.FolderList
                                              select n.Fname).First().ToString();
                                SubfolderLv1 = (from n in db.OFOTs
                                                where n.DocNum == model.FolderLv1
                                                select n.Fname).First().ToString();
                                displayPath = String.Format(@"{0}{1}\\{2}\\{3}\\{4}", networkPath, compName, deptName, FolderName, SubfolderLv1);

                                ViewBag.Message = String.Format("Subfolder {0} is already exists on the path: {1} Please revise.", SubfolderLv1, displayPath);
                                return View("Create");
                            }
                            

                            // Department is selected and FolderName is entered
                            else if ((model.FolderList == 0) && (model.FolderName != null))
                            {
                                deptName = (from n in db.OFOTs
                                            where n.DocNum == model.DeptList
                                            select n.Fname).First().ToString();

                                folderPath = path + compName + "\\" + deptName + "\\" + model.FolderName;

                                displayPath = networkPath + compName + "\\" + deptName + "\\" + model.FolderName;

                                if (!Directory.Exists(folderPath))
                                {
                                    DirectoryInfo di = Directory.CreateDirectory(folderPath);

                                    CreateFolder(model.FolderName, model.DeptList, 3, int.Parse(getUserData("id")));

                                    ViewBag.Message = "Folder created at: " + displayPath;

                                    return View("Create");
                                }
                                else
                                {
                                    ViewBag.Message = "Folder already exists, please revise!";
                                    return View("Create");
                                }

                            }
                            else if ((model.FolderList != 0) && (model.FolderName != null))
                            {
                                ViewBag.Message = "You have selected folder from Folder list and defined a new folder. Please revise.";
                                return View("Create");
                            }
                            else if ((model.FolderList == 0) && (model.FolderName == null))
                            {
                                ViewBag.Message = "Folder name is missing. Please revise!";
                                return View("Create");
                            }
                            else if ((model.FolderList != 0) && (model.FolderLv1Name == null))
                            {
                                ViewBag.Message = "Folder name of Subfolder Level 1 is missing. Please revise.";
                                return View(model);
                            }

                            // Subfolder level 1 is selected but sub folder level 2 is missing
                            else if ((model.FolderLv1 != 0) && (model.FolderLv2Name == null))
                            {
                                ViewBag.Message = "Folder name of Subfolder Level 2 is missing. Please revise.";
                                return View("Create");
                            }
                        }
                        else if (model.FolderLv2Name != null)
                        {
                            deptName = (from n in db.OFOTs
                                        where n.DocNum == model.DeptList
                                        select n.Fname).First().ToString();

                            FolderName = (from n in db.OFOTs
                                          where n.DocNum == model.FolderList
                                          select n.Fname).First().ToString();
                            SubfolderLv1 = (from n in db.OFOTs
                                            where n.DocNum == model.FolderLv1
                                            select n.Fname).First().ToString();
                            folderPath = path + compName + "\\" + deptName + "\\" + FolderName + "\\" + SubfolderLv1 + "\\" + model.FolderLv2Name;
                            displayPath = networkPath + compName + "\\" + deptName + "\\" + FolderName + "\\" + SubfolderLv1 + "\\" + model.FolderLv2Name;

                            if (!Directory.Exists(folderPath))
                            {
                                DirectoryInfo di = Directory.CreateDirectory(folderPath);

                                CreateFolder(model.FolderLv2Name, model.FolderLv1, 5, int.Parse(getUserData("id")));

                                ViewBag.Message = "Folder is created at: " + displayPath;

                                return View("Create");

                            }
                            else
                            {
                                ViewBag.Message = "Folder already exists, please revise!";
                                return View("Create");
                            }

                        }
                       
                    }
                    return View(model);

                case "Cancel":
                    return RedirectToAction("Index");
            }
            return View(model);
        }

        

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        #region AT_helper


        private string getUserData(string para)
        {
            HttpCookie authCookie = Request.Cookies[FormsAuthentication.FormsCookieName];
            FormsAuthenticationTicket ticket = FormsAuthentication.Decrypt(authCookie.Value);
            string[] data = ticket.UserData.Split(",".ToCharArray());
            string userID = data[0];
            string role = data[1];
            string compID = data[2];
            string deptID = data[3];
            switch (para)
            {
                case "id":
                    return userID;
                case "role":
                    return role;
                case "compID":
                    return compID;
                case "deptID":
                    return deptID;
                default:
                    throw new Exception("para is missing");
            }
        }

        public string CreateFolder(string fldname, int? parentID, int lv, int userID)
        {
            try
            {
                var p = new DynamicParameters();
                p.Add("@fn","CreateFolder");
                p.Add("@UserID", userID);
                p.Add("@FolderName", fldname);
                p.Add("@ParentID", parentID);
                p.Add("@Level", lv);

                int retval = SqlMapperUtil.InsertUpdateOrDeleteStoredProc("_spFolderManagement", p, db.conn);

                return "OK";
            }
            catch(Exception ex)
            {
                return ex.Message;
            }
        }

        #endregion
    }
}
