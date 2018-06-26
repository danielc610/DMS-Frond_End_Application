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
using System.Web.Security;
using PagedList;
using System.Collections.Generic;
using Dapper;

namespace DMS_FEA.Controllers
{
    public class UserController : Controller
    {
        private DMSDBContext db = new DMSDBContext();

        // GET: User
        [Authorize]
        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
            var dict = new Dictionary<string, string>();
            dict.Add("@fn", "GetUserList");


            ViewBag.CurrentSort = sortOrder;
            ViewBag.UserNameSortParm = String.IsNullOrEmpty(sortOrder) ? "UserNameDesc" : "";
            ViewBag.StaffNameSortParm = sortOrder == "StaffName" ? "StaffNameDesc" : "StaffName";
            ViewBag.DispNameSortParm = sortOrder == "DispName" ? "DispNameDesc" : "DispName";
            ViewBag.CompNameSortParm = sortOrder == "CompName" ? "CompNameDesc" : "CompName";
            ViewBag.DeptNameSortParm = sortOrder == "DeptName" ? "DeptNameDesc" : "DeptName";
            ViewBag.PositionSortParm = sortOrder == "Position" ? "PositionDesc" : "Position";
            ViewBag.EmailSortParm = sortOrder == "Email" ? "EmailDesc" : "Email";
            ViewBag.ActiveSortParm = sortOrder == "Active" ? "ActiveDesc" : "Active";
            ViewBag.UserRoleSortParm = sortOrder == "UserRole" ? "UserRoleDesc" : "UserRole";

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;



            if (!String.IsNullOrEmpty(searchString))
            {
                dict.Add("@searchKey", searchString);
            }

            switch (sortOrder)
            {
                case "UserNameDesc":
                    dict.Add("@sort", "UserNameDesc");
                    break;

                case "StaffName":
                    dict.Add("@sort", "StaffName");
                    break;
                case "StaffNameDesc":
                    dict.Add("@sort", "StaffNameDesc");
                    break;
                case "DispName":
                    dict.Add("@sort", "DispName");
                    break;            
                case "DispNameDesc":  
                    dict.Add("@sort", "DispNameDesc");
                    break;            
                case "CompName":      
                    dict.Add("@sort", "CompName");
                    break;            
                case "CompNameDesc":  
                    dict.Add("@sort", "CompNameDesc");
                    break;            
                case "DeptName":      
                    dict.Add("@sort", "DeptName");
                    break;            
                case "DeptNameDesc":  
                    dict.Add("@sort", "DeptNameDesc");
                    break;            
                case "Position":      
                    dict.Add("@sort", "Position");
                    break;            
                case "PositionDesc":  
                    dict.Add("@sort", "PositionDesc");
                    break;            
                case "Email":         
                    dict.Add("@sort", "Email");
                    break;            
                case "EmailDesc":     
                    dict.Add("@sort", "EmailDesc");
                    break;            
                case "Active":        
                    dict.Add("@sort", "Active");
                    break;            
                case "ActiveDesc":    
                    dict.Add("@sort", "ActiveDesc");
                    break;            
                case "UserRole":      
                    dict.Add("@sort", "UserRole");
                    break;            
                case "UserRoleDesc":  
                    dict.Add("@sort", "UserRoleDesc");
                    break;
                default:
                    dict.Add("@sort", "UserName");
                    break;
            }

            List<UserIndexViewModel> results = new List<UserIndexViewModel>();

            try
            {
                DataTable dt = ATAT_Lib.ATAT_Lib.GetData("_spUserManagement", dict);

                if(dt.Rows.Count > 0)
                {
                    foreach(DataRow dr in dt.Rows)
                    {
                        results.Add(new UserIndexViewModel()
                        {
                            DocNum = int.Parse(dr["DocNum"].ToString()),
                            User_Code = dr["UserCode"].ToString(),
                            U_Name = dr["StaffName"].ToString(),
                            Nick_Name = dr["DisplayName"].ToString(),
                            Comp_Name = dr["Company"].ToString(),
                            Dept_Name = dr["Department"].ToString(),
                            Position = dr["Position"].ToString(),
                            E_Mail = dr["Email"].ToString(),
                            Active = bool.Parse(dr["Active"].ToString()),
                            UserRole = dr["UserRole"].ToString()


                        });
                    }
                }
            }
            catch(Exception ex)
            {
                return View(ex.Message);
            }

            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return View(results.ToPagedList(pageNumber, pageSize));
        }

        [HttpPost]
        public JsonResult UpdateList(string val)
        {
            int parentID = Int32.Parse(val);

            var list = from u in db.OFOTs
                       join dept in db.ODEPs on u.deptID equals dept.DocNum
                       join comp in db.OCOMs on u.compID equals comp.DocNum
                       where u.compID == parentID &&
                       u.Flevel == 2
                       select new
                       {
                           parID = u.DocNum,
                           name = dept.Dept_Name
                       };
            return Json(list.OrderBy(x => x.name), JsonRequestBehavior.AllowGet);
        }

        // GET: User/Create
        [Authorize]
        public ActionResult Create()
        {
            var currentUserRole = getUserData("role");



            List<SelectListItem> role = new List<SelectListItem>();


            switch (currentUserRole)
            {
                case "S":
                    role.Add(new SelectListItem() { Value = "S", Text = "Superuser" });
                    role.Add(new SelectListItem() { Value = "A", Text = "Administrator" });
                    role.Add(new SelectListItem() { Value = "U", Text = "User" });
                    break;
                case "A":
                    role.Add(new SelectListItem() { Value = "A", Text = "Administrator" });
                    role.Add(new SelectListItem() { Value = "U", Text = "User" });
                    break;

            }

            IEnumerable<SelectListItem> compList = from u in db.OCOMs.Where(u => u.Active == true)
                                                   select new SelectListItem
                                                   {
                                                       Text = u.Comp_Name,
                                                       Value = u.DocNum.ToString()
                                                   };

            IEnumerable<SelectListItem> deptList = from u in db.OFOTs
                                                   join dept in db.ODEPs on u.deptID equals dept.DocNum
                                                   where(u.Flevel == 2 && dept.Active == true)
                                                   select new SelectListItem
                                                   {
                                                       Text = dept.Dept_Name,
                                                       Value = u.DocNum.ToString()
                                                   };

            

            ViewBag.Company_Code = compList.OrderBy(x => x.Text);

            ViewBag.Department_Code = deptList.OrderBy( x => x.Text);




            CreateUserViewModel model = new CreateUserViewModel { Roles = role };
            model.Active = true;

            return View(model);
        }

        // POST: User/Create
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CreateUserViewModel model, string BtnSubmit)
        {
            var currentUserRole = getUserData("role");

            IEnumerable<SelectListItem> compList = from u in db.OCOMs.Where(u => u.Active == true)
                                                   select new SelectListItem
                                                   {
                                                       Text = u.Comp_Name,
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
            List<SelectListItem> role = new List<SelectListItem>();


            switch (currentUserRole)
            {
                case "S":
                    role.Add(new SelectListItem() { Value = "S", Text = "Superuser" });
                    role.Add(new SelectListItem() { Value = "A", Text = "Administrator" });
                    role.Add(new SelectListItem() { Value = "U", Text = "User" });
                    break;
                case "A":
                    role.Add(new SelectListItem() { Value = "A", Text = "Administrator" });
                    role.Add(new SelectListItem() { Value = "U", Text = "User" });
                    break;

            }

            ViewBag.Company_Code = compList.OrderBy(x => x.Text);
            model.Roles = role;
            ViewBag.Department_Code = deptList.OrderBy(x => x.Text);
            switch (BtnSubmit)
            {
                case "Create":
                    if (ModelState.IsValid)
                    {
                        ATAT_Lib.ATAT_Lib helper = new ATAT_Lib.ATAT_Lib();

                        var hashedPwd = helper.PasswordHashing(model.Password);

                        if(model.UserRole == null)
                        {
                            ViewBag.Message = "Please select valid user role!";

                            return View("Create");

                        }
                        else
                        {
                            try
                            {
                                CreateUser("CreateUser", model, hashedPwd, 1);

                                return RedirectToAction("Index");
                                //return Content(model.UserRole.ToString());
                            }
                            catch (Exception ex)
                            {
                                return View("Error", new HandleErrorInfo(ex, "User", "Create"));
                            }

                        }


                    }
                    break;
            }


            return View(model);
        }


        [HttpPost]
        public async Task<JsonResult> UsercodeValid(string User_Code)
        {
            var validateName = await db.OUSRs.FirstOrDefaultAsync(n => n.User_Code == User_Code);
            if (validateName != null)
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(true, JsonRequestBehavior.AllowGet);
            }
        }

        /*
         * 


        [HttpPost]
        public async Task<JsonResult> UpdateUsercodeValid(int DocNum, string User_Code)
        {
            var user = await db.OUSRs.FirstOrDefaultAsync((n => n.DocNum == DocNum) && (n => n.User_code == User_Code));

        }
         * 
         */


        // GET: User/update/5
        [Authorize]
        public async Task<ActionResult> Update(int? id)
        {
            var currentUserRole = getUserData("role");

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var user = await db.OUSRs.FindAsync(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            else
            {
                List<SelectListItem> role = new List<SelectListItem>();

                switch (currentUserRole)
                {
                    case "S":
                        role.Add(new SelectListItem() { Value = "S", Text = "Superuser", Selected = (user.UserRole == "S") });
                        role.Add(new SelectListItem() { Value = "A", Text = "Administrator", Selected = (user.UserRole == "A") });
                        role.Add(new SelectListItem() { Value = "U", Text = "User", Selected = (user.UserRole == "U") });
                        break;
                    case "A":
                        role.Add(new SelectListItem() { Value = "A", Text = "Administrator", Selected = (user.UserRole == "A") });
                        role.Add(new SelectListItem() { Value = "U", Text = "User", Selected = (user.UserRole == "U") });
                        break;
                        
                }

                IEnumerable<SelectListItem> compList = from u in db.OCOMs
                                                       where u.Active == true
                                                       select new SelectListItem
                                                       {
                                                           Text = u.Comp_Name,
                                                           Value = u.DocNum.ToString(),
                                                           Selected = (u.DocNum == user.Company_Code)
                                                       };

                IEnumerable<SelectListItem> deptList = from fld in db.OFOTs
                                                       join dept in db.ODEPs on fld.deptID equals dept.DocNum
                                            where fld.Flevel == 2 && dept.Active == true
                                            && (fld.Fparent == user.Company_Code)
                                                       select new SelectListItem
                                                       {
                                                           Text = dept.Dept_Name,
                                                           Value = fld.DocNum.ToString(),
                                                           Selected = (fld.DocNum == user.Department_Code)
                                                       };

                

                UpdateUserViewModel viewModel = new UpdateUserViewModel();

                viewModel.DocNum = user.DocNum;
                viewModel.User_Code = user.User_Code;
                viewModel.U_Name = user.U_Name;
                viewModel.Nick_Name = user.Nick_Name;
                viewModel.Active = user.Active;

                viewModel.Position = user.Position;
                viewModel.E_Mail = user.E_Mail;
                viewModel.Roles = role;

                ViewBag.userRole = user.UserRole;

                ViewBag.Company_Code = compList.OrderBy(x => x.Text);
                ViewBag.Department_Code = deptList.OrderBy(x => x.Text);
                

                return View(viewModel);
            }

        }

        // POST: User/Update/5
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Update(UpdateUserViewModel model, string btnSubmit)
        {
            var currentUserRole = getUserData("role");

            var user = await db.OUSRs.FindAsync(model.DocNum);



            List<SelectListItem> role = new List<SelectListItem>();
            switch (currentUserRole)
            {
                case "S":
                    role.Add(new SelectListItem() { Value = "S", Text = "Superuser", Selected = (user.UserRole == "S") });
                    role.Add(new SelectListItem() { Value = "A", Text = "Administrator", Selected = (user.UserRole == "A") });
                    role.Add(new SelectListItem() { Value = "U", Text = "User", Selected = (user.UserRole == "U") });
                    break;
                case "A":
                    role.Add(new SelectListItem() { Value = "A", Text = "Administrator", Selected = (user.UserRole == "A") });
                    role.Add(new SelectListItem() { Value = "U", Text = "User", Selected = (user.UserRole == "U") });
                    break;

            }

            IEnumerable<SelectListItem> compList = from u in db.OCOMs
                                                   where u.Active == true
                                                   select new SelectListItem
                                                   {
                                                       Text = u.Comp_Name,
                                                       Value = u.DocNum.ToString(),
                                                       Selected = (u.DocNum == user.Company_Code)
                                                   };

            IEnumerable<SelectListItem> deptList = from fld in db.OFOTs
                                                   join dept in db.ODEPs on fld.deptID equals dept.DocNum
                                                   where (fld.Flevel == 2 && dept.Active == true)
                                                   && (fld.Fparent == user.Company_Code)
                                                   select new SelectListItem
                                                   {
                                                       Text = dept.Dept_Name,
                                                       Value = fld.DocNum.ToString(),
                                                       Selected = (fld.DocNum == user.Department_Code)
                                                   };

            ViewBag.Company_Code = compList.OrderBy(x => x.Text);
            ViewBag.Department_Code = deptList.OrderBy(x => x.Text);
            model.Roles = role;
            ViewBag.userRole = user.UserRole;

            if ((model.Company_Code == 0) && (model.Department_Code != 0))
            {
                ViewBag.Messsage = "Please select valid Company option.";
                return View(model);
            }
            else
            {
                switch (btnSubmit)
                {
                    case "Update":

                        if (ModelState.IsValid)
                        {
                            try
                            {
                                UpdateUser("UpdateUser", model, int.Parse(getUserData("id")));
                                return RedirectToAction("Index");
                            }
                            catch (Exception ex)
                            {
                                return View("Error", new HandleErrorInfo(ex, "User", "Update"));
                            }
                        }
                        break;
                    case "Cancel":
                        return RedirectToAction("Index");
                }

            }

            return View(model);
        }

        // GET : User/ChangePwd/1
        [Authorize]
        public async Task<ActionResult> ChangePwd(int? id)
        {

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var user = await db.OUSRs.FindAsync(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            else
            {
                ResetPasswordViewModel viewModel = new ResetPasswordViewModel();

                viewModel.DocNum = user.DocNum;
                viewModel.User_Code = user.User_Code;

                return View(viewModel);

            }

        }

        // POST: User/ChangePwd/1
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangePwd(ResetPasswordViewModel model, int? id, string btnSubmit)
        {
            switch (btnSubmit)
            {
                case "Save":
                    if (ModelState.IsValid)
                    {
                        try
                        {
                            SavePwdToDB("ChangePwd", model);
                            
                            ViewBag.Message = "Password is updated successfully.";

                            return View("ChangePwd");
                        }
                        catch (Exception ex)
                        {
                            return View("Error", new HandleErrorInfo(ex, "User", "ResetPassword"));
                        }

                    }
                    break;      
            }
            return View(model);
        }

        // GET: User/Login
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }


        // POST: User/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginUserViewModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                foreach (var user in db.OUSRs)
                {
                    ATAT_Lib.ATAT_Lib helper = new ATAT_Lib.ATAT_Lib();
                    var hashpass = helper.PasswordHashing(model.Password);
                    if (model.User_Code == user.User_Code && hashpass == user.Password)
                    {
                        if (user.Active == true)
                        {
                            var name = user.Nick_Name;
                            var ID = user.DocNum.ToString();
                            var role = user.UserRole;
                            var comp = user.Company_Code;
                            var dept = user.Department_Code;
                            FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(1, name, DateTime.Now, DateTime.Now.AddMinutes(30), true, ID + "," + role + "," + comp + "," + dept, FormsAuthentication.FormsCookiePath);



                            string encTicket = FormsAuthentication.Encrypt(ticket);
                            var cookie = new HttpCookie(FormsAuthentication.FormsCookieName, encTicket);
                            cookie.HttpOnly = true;

                            Response.Cookies.Add(cookie);


                            return RedirectToAction("Index", "Home");

                        }
                        else
                        {
                            ViewBag.Message = "This user account is not Active. Please contact your supervisor!";
                            return View("Login");
                        }

                    }
                    else if (model.User_Code == user.User_Code && hashpass != user.Password)
                    {
                        ViewBag.Message = "You entered an incorrect user name or password, please try again.";

                        return View("Login");
                    }
                    // else if (model.User_Code != user.User_Code && hashpass != user.Password) 
                    // {
                    //     ViewBag.Message = "You entered an incorrect user name or password, please try again.";
                    //     return View("Login");
                    // }

                }

            }
            return View(model);
        }

        // POST: /User/LogOff
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Home");
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


        public string CreateUser(string fn, CreateUserViewModel model, string password, int id)
        {
            try
            {
                var p = new DynamicParameters();
                p.Add("@fn", fn);
                p.Add("@UserCode", (model.User_Code).ToUpper());
                p.Add("@Password", password);
                p.Add("@Active", model.Active);
                p.Add("@CompanyID", model.Company_Code);
                p.Add("@DepartmentID", model.Department_Code);
                p.Add("@U_Name", model.U_Name);
                p.Add("@Nick_Name", model.Nick_Name);
                p.Add("@Position", model.Position);
                p.Add("@Email", model.E_Mail);
                p.Add("@UserRole", model.UserRole.ToString());
                p.Add("@CreateID", id);

                int retval = SqlMapperUtil.InsertUpdateOrDeleteStoredProc("_spUserManagement", p, db.conn);

                return "OK";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public string UpdateUser(string fn, UpdateUserViewModel model, int id)
        {
            try
            {
                var p = new DynamicParameters();
                p.Add("@fn", fn);
                p.Add("@Active", model.Active);
                p.Add("@CompanyID", model.Company_Code);
                p.Add("@DepartmentID", model.Department_Code);
                p.Add("@U_Name", model.U_Name);
                p.Add("@Nick_Name", model.Nick_Name);
                p.Add("@UserRole", model.UserRole.ToString());
                p.Add("@Position", model.Position);
                p.Add("@Email", model.E_Mail);
                p.Add("@UpdateID", id);
                p.Add("@UserID", model.DocNum);

                int retval = SqlMapperUtil.InsertUpdateOrDeleteStoredProc("_spUserManagement", p, db.conn);

                return "OK";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        public string DeactiveUser(string fn, UpdateUserViewModel model, int id)
        {
            try
            {
                var p = new DynamicParameters();
                p.Add("@fn", fn);
                p.Add("@UpdateID", id);
                p.Add("@UserID", model.DocNum);

                int retval = SqlMapperUtil.InsertUpdateOrDeleteStoredProc("_spUserManagement", p, db.conn);

                return "OK";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public string SavePwdToDB(string fn, ResetPasswordViewModel model)
        {
            ATAT_Lib.ATAT_Lib helper = new ATAT_Lib.ATAT_Lib();
            try
            {
                var p = new DynamicParameters();
                p.Add("@fn", fn);
                p.Add("@Password", helper.PasswordHashing(model.ConfirmPassword));
                p.Add("@UserID", model.DocNum);

                int retval = SqlMapperUtil.InsertUpdateOrDeleteStoredProc("_spUserManagement", p, db.conn);
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

