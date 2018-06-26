using System;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using DMS_FEA.Models;
using DMS_FEA.ViewModels;
using System.Threading.Tasks;
using System.IO;
using System.Web.Security;
using PagedList;
using Dapper;

namespace DMS_FEA.Controllers
{
    [Authorize]
    public class CompanyController : Controller
    {
        private DMSDBContext db = new DMSDBContext();

        private string path = @"D:\\DATA\\DMS\\DMS_DATA\\";

        // GET: Company
        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.CompCodeSortParm = String.IsNullOrEmpty(sortOrder) ? "CompCodeDesc" : "";
            ViewBag.CompNameSortParm = sortOrder == "CompName" ? "CompNameDesc" : "CompName";
            ViewBag.FolderNameSortParm = sortOrder == "FolderName" ? "FolderNameDesc" : "FolderName";
            ViewBag.ActiveSortParm = sortOrder == "Active" ? "ActiveDesc" : "Active";

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;

            var comps = from n in db.OCOMs
                        select n;
            if (!String.IsNullOrEmpty(searchString))
            {
                comps = comps.Where(s => s.Comp_Name.Contains(searchString)
                                    || s.Comp_Code.Contains(searchString));
            }

            switch (sortOrder)
            {
                case "CompCodeDesc":
                    comps = comps.OrderByDescending(n => n.Comp_Code);
                    break;
                case "CompName":
                    comps = comps.OrderBy(n => n.Comp_Name);
                    break;
                case "CompNameDesc":
                    comps = comps.OrderByDescending(n => n.Comp_Name);
                    break;
                case "FolderName":
                    comps = comps.OrderBy(n => n.Fname);
                    break;
                case "FolderNameDesc":
                    comps = comps.OrderByDescending(n => n.Fname);
                    break;
                case "Active":
                    comps = comps.OrderByDescending(n => n.Active);
                    break;
                case "ActiveDesc":
                    comps = comps.OrderBy(n => n.Active);
                    break;
                default:
                    comps = comps.OrderBy(n => n.Comp_Code);
                    break;
            }

            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return View(comps.ToPagedList(pageNumber, pageSize));
        }


        // GET: Company/Create
        public ActionResult Create()
        {
            CreateCompViewModel model = new CreateCompViewModel();

            model.Active = true;

            return View(model);
        }

        [HttpPost]
        public async Task<JsonResult> CompCodeValidate(string Comp_Code)
        {
            var validateName = await db.OCOMs.FirstOrDefaultAsync(n => n.Comp_Code == Comp_Code);
            if (validateName != null)
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(true, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public async Task<ActionResult> CompNameValidate(string Comp_Name)
        {
            var validateName = await db.OCOMs.FirstOrDefaultAsync(n => n.Comp_Name == Comp_Name);
            if (validateName != null)
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(true, JsonRequestBehavior.AllowGet);
            }
        }

        // POST: Company/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CreateCompViewModel model)
        {
            if (ModelState.IsValid)
            {
                try { 

                    var CompPath = path + model.Fname;
                    if (!Directory.Exists(CompPath))
                    {

                        DirectoryInfo di = Directory.CreateDirectory(CompPath);

                        CreateCompToDB("CreateComp", model, int.Parse(getUserData("id")));

                        ViewBag.message = String.Format("Company Code {0} with folder name {1} is created." , model.Comp_Code,model.Fname);
                        return View("Create");
                    }
                    else
                    {
                        ViewBag.message = String.Format("Company Folder Name {0} already exists, please choose another name.", model.Fname);

                        return View("Create");
                    }

                }
                catch (Exception ex)
                {
                    return Content(ex.Message);
                    //return View("Error", new HandleErrorInfo(ex, "Company", "Create"));
                }
            }

            return View(model);
        }

        // GET: Company/Update/5
        public async Task<ActionResult> Update(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var comp = await db.OCOMs.FindAsync(id);
            if (comp == null)
            {
                return HttpNotFound();
            }
            else
            {
                UpdateCompViewModel viewModel = new UpdateCompViewModel();

                viewModel.DocNum = comp.DocNum;
                viewModel.Comp_Code = comp.Comp_Code;
                viewModel.Comp_Name = comp.Comp_Name;
                viewModel.Fname = comp.Fname;
                viewModel.Active = comp.Active;

                return View(viewModel);
            }
        }

        // POST: Company/Update/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Update(UpdateCompViewModel model, string btnSubmit)
        {
            switch (btnSubmit)
            {
                case "Update":
                    if (ModelState.IsValid)
                    {
                        try
                        {
                            UpdateCompToDB("UpdateComp", model, int.Parse(getUserData("id")));
                            return RedirectToAction("Index");
                        }
                        catch (Exception ex)
                        {
                            return View("Error", new HandleErrorInfo(ex, "Company", "Update"));
                        }

                    }
                    break;
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



        #region AT_Helper

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


        public string CreateCompToDB(string fn, CreateCompViewModel model, int id)
        {
            try
            {
                var p = new DynamicParameters();
                p.Add("@fn", fn);
                p.Add("@UserID", id);
                p.Add("@CompName", model.Comp_Name);
                p.Add("@CompCode", model.Comp_Code);
                p.Add("@Active", model.Active);
                p.Add("@FolderName", model.Fname);

                int retval = SqlMapperUtil.InsertUpdateOrDeleteStoredProc("_spFolderManagement", p, db.conn);

                return "OK";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

        }

        public string UpdateCompToDB(string fn, UpdateCompViewModel model, int id)
        {
            try
            {
                var p = new DynamicParameters();
                p.Add("@fn", fn);
                p.Add("@CompCode", model.Comp_Code);
                p.Add("@CompName", model.Comp_Name);
                p.Add("@Active", model.Active);
                p.Add("@UserID", id);
                p.Add("@compID", model.DocNum);
                p.Add("@FolderName", model.Fname);

                int retval = SqlMapperUtil.InsertUpdateOrDeleteStoredProc("_spFolderManagement", p, db.conn);

                return "OK";
            }
            catch(Exception ex)
            {
                return ex.Message;
            }
        }

        public string DeactiveComp(string fn, int id, string CompID)
        {
            try
            {
                var p = new DynamicParameters();
                p.Add("@fn", fn);
                p.Add("@Active", 0);
                p.Add("@compID", CompID);
                p.Add("@UserID", id);

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
