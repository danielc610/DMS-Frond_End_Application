using Dapper;
using DMS_FEA.Models;
using DMS_FEA.ViewModels;
using PagedList;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace DMS_FEA.Controllers
{
    public class DeptController : Controller
    {
        private string path = @"D:\\DATA\\DMS\\DMS_DATA\\";
        private string networkPath = @"M:\";

        private DMSDBContext db = new DMSDBContext();

        // GET: Dept
        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.CompCodeSortParm = String.IsNullOrEmpty(sortOrder) ? "CompCodeDesc" : "";
            ViewBag.CompNameSortParm = sortOrder == "CompName" ? "CompNameDesc" : "";
            ViewBag.CompFldSortParm = sortOrder == "CompFld" ? "CompFldDesc" : "CompFld";
            ViewBag.DeptCodeSortParm = sortOrder == "DeptCode" ? "DeptCodeDesc" : "DeptCode";
            ViewBag.DeptNameSortParm = sortOrder == "DeptName" ? "DeptNameDesc" : "DeptName";
            ViewBag.ActiveSortParm = sortOrder == "Active" ? "ActiveDesc" : "Active";
            ViewBag.DeptFldSortParm = sortOrder == "DeptFld" ? "DeptFldDesc" : "DeptFld";
            //ViewBag.ActiveSortParm = sortOrder == "Active" ? "ActiveDesc" : "Active";

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;

            var compDepts = from compFld in db.OFOTs
                            join deptFld in db.OFOTs on compFld.DocNum equals deptFld.Fparent
                            join comp in db.OCOMs on compFld.compID equals comp.DocNum
                            join dept in db.ODEPs on deptFld.deptID equals dept.DocNum
                            where deptFld.Flevel == 2
                            select new DeptIndexViewModel
                            {
                                Comp_ID = compFld.DocNum,
                                Comp_Code = comp.Comp_Code,
                                Comp_Name = comp.Comp_Name,
                                CompFldName = compFld.Fname,
                                Dept_ID = deptFld.DocNum,
                                Dept_Code = dept.Dept_Code,
                                Dept_Name = dept.Dept_Name,
                                DeptFldName = deptFld.Fname,
                                Active = dept.Active,
                                FolderPath = networkPath + compFld.Fname + "\\" + deptFld.Fname
                            }; 
            if (!String.IsNullOrEmpty(searchString))
            {
                compDepts = compDepts.Where(s => s.Dept_Name.Contains(searchString));
            }

            switch(sortOrder)
            {
                case "CompCodeDesc":
                    compDepts = compDepts.OrderByDescending(cd => cd.Comp_Code);
                    break;
                case "CompName":
                    compDepts = compDepts.OrderBy(cd => cd.Comp_Name);
                    break;
                case "CompNameDesc":
                    compDepts = compDepts.OrderByDescending(cd => cd.Comp_Name);
                    break;
                case  "CompFld":
                    compDepts = compDepts.OrderBy(cd => cd.CompFldName);
                    break;
                case "CompFldDesc":
                    compDepts = compDepts.OrderByDescending(cd => cd.CompFldName);
                    break;
                case "DeptCode":
                    compDepts = compDepts.OrderBy(cd => cd.Dept_Code);
                    break;
                case "DeptCodeDesc":
                    compDepts = compDepts.OrderByDescending(cd => cd.Dept_Code);
                    break;
                case "DeptName":
                    compDepts = compDepts.OrderBy(cd => cd.Dept_Name);
                    break;
                case "DeptNameDesc":
                    compDepts = compDepts.OrderByDescending(cd => cd.Dept_Name);
                    break;
                case "Active":
                    compDepts = compDepts.OrderBy(cd => cd.Active);
                    break;
                case "ActiveDesc":
                    compDepts = compDepts.OrderByDescending(cd => cd.Active);
                    break;
                case "DeptFld":
                    compDepts = compDepts.OrderBy(cd => cd.DeptFldName);
                    break;
                case "DeptFldDesc":
                    compDepts = compDepts.OrderByDescending(cd => cd.DeptFldName);
                    break;

                default:
                    compDepts = compDepts.OrderBy(cd => cd.Comp_Code);
                    break;
            }

            int pageSize = 10;
            int pageNumber = (page ?? 1);

            return View(compDepts.ToPagedList(pageNumber,pageSize));
        }

        // Get: Dept/Create
        [Authorize]
        public ActionResult Create()
        {
            IEnumerable<SelectListItem> compList = from fld in db.OFOTs
                                                   join comp in db.OCOMs on fld.compID equals comp.DocNum
                                                   where (fld.Flevel == 1 && comp.Active ==true)
                                                   select new SelectListItem
                                                   {
                                                       Text = comp.Comp_Name,
                                                       Value = fld.DocNum.ToString()
                                                   };
            compList = compList.OrderBy(x => x.Text);

            CreateDeptViewModel viewModel = new CreateDeptViewModel();
            viewModel.Active = true;
            ViewBag.Comp_ID = compList;

            return View(viewModel);
        }

        // POST: Dept/Create
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CreateDeptViewModel model, string btnSubmit)
        {
            IEnumerable<SelectListItem> compList = from fld in db.OFOTs
                                                   join comp in db.OCOMs on fld.compID equals comp.DocNum
                                                   where (fld.Flevel == 1 && comp.Active ==true)
                                                   select new SelectListItem
                                                   {
                                                       Text = comp.Comp_Name,
                                                       Value = fld.DocNum.ToString()
                                                   };

            compList = compList.OrderBy(x => x.Text);
            CreateDeptViewModel vm = new CreateDeptViewModel();

            ViewBag.Comp_ID = compList;

            switch (btnSubmit)
            {
                case "Create":
                    if (ModelState.IsValid)
                    {

                       

                        var folderPath = "";
                        var compID = model.Comp_ID;
                        var compName = (from comp in db.OCOMs
                                        join compfld in db.OFOTs on comp.DocNum equals compfld.compID
                                        where compfld.DocNum == compID
                                        select comp.Comp_Name).FirstOrDefault().ToString();

                        var compfldName = (from n in db.OFOTs  
                                        where n.DocNum == compID
                                        select n.Fname).FirstOrDefault().ToString();

                        var deptFldName = model.ShortName;

                        folderPath = path + compfldName + "\\" + deptFldName;

                        try
                        {
                            if (!Directory.Exists(folderPath))
                            {
                                DirectoryInfo di = Directory.CreateDirectory(folderPath);

                                CreateDeptToDB("CreateDeptFolder", model, int.Parse(getUserData("id")));

                                ViewBag.Message = String.Format("Department Code {0} with folder name {1} is created under {2}.", model.Dept_Code, deptFldName, compfldName);

                                return View("Create");

                            }
                            //else if (validateDeptCode != null)
                            //{
                            //    ViewBag.Message = String.Format("Department Code {0} is already assigned to company: {1}. Please revise.", model.Dept_Code, compName);
                            //    return View("Create");
                            //}
                            else
                            {
                                ViewBag.Message = String.Format("Department Folder Name {0} of {1} is already exists, please choose another Folder Name.", deptFldName, compName);

                                return View("Create");
                            }

                        }
                        catch (Exception ex)
                        {
                            return View("Error", new HandleErrorInfo(ex, "Dept", "Create"));
                        }
                        

                       
                        
                    }
                    break;

                case "Cancel":
                    return RedirectToAction("Index");   
            }
            return View(model);

        }

        [HttpPost]
        public async Task<JsonResult> DeptCodeVaildate(int Comp_ID, string Dept_Code)
        {
            var validateDeptCode =  await (from dept in db.ODEPs
                                    join fld in db.OFOTs on dept.DocNum equals fld.deptID
                                    where fld.DocNum == Comp_ID && dept.Dept_Code == Dept_Code
                                    select dept.Dept_Code).FirstOrDefaultAsync();
            if (validateDeptCode != null)
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(true, JsonRequestBehavior.AllowGet);
            }
        }

        // GET: Dept/Update/5
        public async Task<ActionResult> Update(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var dept = await db.OFOTs.FindAsync(id);
            if (dept == null)
            {
                return HttpNotFound();
            }
            else
            {
                UpdateDeptViewModel viewModel = new UpdateDeptViewModel();

                viewModel.DocNum = dept.DocNum;
                viewModel.Dept_Code = dept.ODEP.Dept_Code;
                viewModel.Dept_Name = dept.ODEP.Dept_Name;
                viewModel.ShortName = dept.Fname;
                viewModel.Active = dept.ODEP.Active;


                IEnumerable<SelectListItem> compList = from fld in db.OFOTs
                                                       join comp in db.OCOMs on fld.compID equals comp.DocNum
                                                       where fld.Flevel == 1
                                                       select new SelectListItem
                                                       {
                                                           Text = comp.Comp_Name,
                                                           Value = fld.DocNum.ToString(),
                                                           Selected = (fld.DocNum == dept.Fparent)
                                                       };
                compList = compList.OrderBy(x => x.Text);
                ViewBag.Comp_ID = compList;

                return View(viewModel);
            }
        }

        // POST: Dept/Update/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Update(UpdateDeptViewModel model, string btnSubmit)
        {
            IEnumerable<SelectListItem> compList = from fld in db.OFOTs
                                                   join comp in db.OCOMs on fld.compID equals comp.DocNum
                                                   where fld.Flevel == 1
                                                   select new SelectListItem
                                                   {
                                                       Text = comp.Comp_Name,
                                                       Value = fld.DocNum.ToString(),
                                                       Selected = (fld.DocNum == model.Comp_ID)
                                                   };
            compList = compList.OrderBy(x => x.Text);
            ViewBag.Comp_ID = compList;

            switch (btnSubmit)
            {
                case "Update":
                    if (ModelState.IsValid)
                    {
                        try
                        {
                            UpdateDeptToDB("UpdateDeptFolder", model, int.Parse(getUserData("id")));
                            return RedirectToAction("Index");
                        }
                        catch (Exception ex)
                        {
                            return View("Error", new HandleErrorInfo(ex, "Dept", "Update"));
                        }
                    }
                    break;
                case "Cancel":
                    return RedirectToAction("Index");
            }
            return View(model);
        }

        #region AT_Helper

        public string CreateDeptToDB(string fn, CreateDeptViewModel model, int id)
        {
            try
            {
                var p = new DynamicParameters();
                p.Add("@fn", fn);
                p.Add("@DeptCode", model.Dept_Code);
                p.Add("@DeptName", model.Dept_Name);
                p.Add("@FolderName", model.ShortName);
                p.Add("@Active", model.Active);
                p.Add("@UserID", id);
                p.Add("@ParentID", model.Comp_ID);


                int retval = SqlMapperUtil.InsertUpdateOrDeleteStoredProc("_spFolderManagement", p, db.conn);

                return "OK";
            }
            catch(Exception ex)
            {
                return ex.Message;
            }
        }

        public string UpdateDeptToDB(string fn, UpdateDeptViewModel model, int id)
        {
            try
            {
                var p = new DynamicParameters();
                p.Add("@fn", fn);
                p.Add("@DeptCode", model.Dept_Code);
                p.Add("@DeptName", model.Dept_Name);
                p.Add("@Active", model.Active);
                p.Add("@UserID", id);
                p.Add("@compID", model.Comp_ID);
                p.Add("@deptID", model.DocNum);

                int retval = SqlMapperUtil.InsertUpdateOrDeleteStoredProc("_spFolderManagement", p, db.conn);

                return "OK";
            }
            catch(Exception ex)
            {
                return ex.Message;
            }
        }

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

        #endregion

    }
}