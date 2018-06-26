using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Printing;
using System.Linq;
using System.IO;
using System.Net;
using System.Web;
using System.Web.Mvc;
using DMS_FEA.Models;
using System.Threading.Tasks;
using DMS_FEA.ViewModels;
using System.Web.Security;
using iTextSharp.text.pdf;
using ZXing;
using ZXing.Common;
using ZXing.Rendering;


namespace DMS_FEA.Controllers
{
    [Authorize]
    public class vCreateBarCodeSheetController : Controller
    {
        private DMSDBContext db = new DMSDBContext();


        public JsonResult GetFoldersData()
        {
            var data = from folders in db.OFOTs
                       select new { folders.DocNum, folders.Fparent, folders.Fname, folders.Flevel };

            return Json(data, JsonRequestBehavior.AllowGet);
        }


        public JsonResult GetFolderName(string id)
        {
            var fldID = int.Parse(id);
            var data = from folders in db.OFOTs
                       select new { folders.Fname };

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
        public JsonResult ListUpdate(string value, int lv)
        {
            int parentID = Int32.Parse(value);
            var list = from u in db.OFOTs
                       where u.Fparent == parentID &&
                       u.Flevel == lv
                       select new
                       {
                           parID = u.DocNum,
                           name = u.Fname
                       };

            return Json(list.OrderBy(x => x.name), JsonRequestBehavior.AllowGet);
        }



        private string tCompList = "";
        private string tDeptList = "";
        private string tFolderList = "";
        private string tSubFolderLv1 = "";
        private string tSubFolderLv2 = "";
        private string tCompanyHeader = "";
        private string tSelectedCompany = "";
        private string tSelectedDepartment = "";
        private string tSelectedFld = "";
        private string tSelectedSubFldLv1 = "";
        private string tSelectedSubFldLv2 = "";



        // GET: Folder/Create
        public ActionResult Index()
        {
            var userCompID = getUserData("compID");
            var userDeptID = getUserData("deptID");

            {
                IEnumerable<SelectListItem> CompList = from u in db.OFOTs
                                                       join comp in db.OCOMs on u.compID equals comp.DocNum
                                                       where (u.Flevel == 1 && comp.Active == true)
                                                       select new SelectListItem
                                                       {
                                                           Text = comp.Comp_Name,
                                                           Value = u.DocNum.ToString(),
                                                           Selected = (u.compID.ToString() == userCompID)
                                                       };

                IEnumerable<SelectListItem> DeptList = from u in db.OFOTs
                                                       join dept in db.ODEPs on u.deptID equals dept.DocNum
                                                       where (u.Flevel == 2 && dept.Active == true && u.Fparent.ToString() == userCompID)
                                                       select new SelectListItem
                                                       {
                                                           Text = dept.Dept_Name,
                                                           Value = u.DocNum.ToString(),
                                                           Selected = (u.DocNum.ToString() == userDeptID)
                                                       };
                IEnumerable<SelectListItem> FolderList = from u in db.OFOTs.Where(u => u.Flevel == 3 && u.Fparent.ToString() == userDeptID)
                                                         select new SelectListItem
                                                         {
                                                             Text = u.Fname,
                                                             Value = u.DocNum.ToString()
                                                         };
                IEnumerable<SelectListItem> SubFolderLv1 = from u in db.OFOTs.Where(u => u.Flevel == 4)
                                                           select new SelectListItem
                                                           {
                                                               Text = u.Fname,
                                                               Value = u.DocNum.ToString()
                                                           };
                IEnumerable<SelectListItem> SubFolderLv2 = from u in db.OFOTs.Where(u => u.Flevel == 5)
                                                           select new SelectListItem
                                                           {
                                                               Text = u.Fname,
                                                               Value = u.DocNum.ToString()
                                                           };

                IEnumerable<SelectListItem> BC_KeepBarCodePage = new SelectListItem[]
                {
                new SelectListItem() { Text = "Keep", Value = "K", Selected=true },
                new SelectListItem() { Text = "Remove", Value = "R"},
                };


                ViewBag.CompList = CompList.OrderBy(x => x.Text);
                ViewBag.DeptList = DeptList.OrderBy(x => x.Text);
                ViewBag.FolderList = FolderList.OrderBy(x => x.Text);
                ViewBag.SubFolderLv1 = SubFolderLv1.OrderBy(x => x.Text);
                ViewBag.SubFolderLv2 = SubFolderLv2.OrderBy(x => x.Text);
                ViewBag.BC_KeepBarCodePage = BC_KeepBarCodePage;


                return View();
            }

        }
        // User Press [Generate] Button on the View //

        //private string TempFolder = @"C:\DATA\"; //Destination of Image File and Output File //
        private string TempFolder = @"C:\inetpub\wwwroot\DMS-P01\DMS_Temp\"; //Destination of Image File and Output File //
        private string UserLink = "http://dms.asiantat.com/dms_Temp/";
        private string TempTimeStamp = ""; //TimeStamp is used as part of file name //
                                           // private string BC_KR = "K"; // Keep or Remove Barcode Sheet //
        private string BC_pdfFile = ""; //Output File Name //
        private string BC_Number = ""; // BC Number used to generate barcode //
        private string BC_FileName = ""; // BC cover file name
                                         // private string BC_string = ""; //BC string (include file name) part of barcode //
        private string BC_print = ""; //BC Print - final barcode number //
        private string docpath = ""; // document path


        [HttpPost]
        public ActionResult Generate(vmCreateBarCodeSheet m, string BtnSubmit)
        {
            var userCompID = getUserData("compID");
            var userDeptID = getUserData("deptID");

            IEnumerable<SelectListItem> CompList = from u in db.OFOTs
                                                   join comp in db.OCOMs on u.compID equals comp.DocNum
                                                   where (u.Flevel == 1 && comp.Active == true)
                                                   select new SelectListItem
                                                   {
                                                       Text = comp.Comp_Name,
                                                       Value = u.DocNum.ToString(),
                                                       Selected = (u.compID.ToString() == userCompID)
                                                   };

            IEnumerable<SelectListItem> DeptList = from u in db.OFOTs
                                                   join dept in db.ODEPs on u.deptID equals dept.DocNum
                                                   where (u.Flevel == 2 && dept.Active == true && u.Fparent.ToString() == userCompID)
                                                   select new SelectListItem
                                                   {
                                                       Text = dept.Dept_Name,
                                                       Value = u.DocNum.ToString(),
                                                       Selected = (u.DocNum.ToString() == userDeptID)
                                                   };
            IEnumerable<SelectListItem> FolderList = from u in db.OFOTs.Where(u => u.Flevel == 3 && u.Fparent.ToString() == userDeptID)
                                                     select new SelectListItem
                                                     {
                                                         Text = u.Fname,
                                                         Value = u.DocNum.ToString()
                                                     };
            IEnumerable<SelectListItem> SubFolderLv1 = from u in db.OFOTs.Where(u => u.Flevel == 4)
                                                       select new SelectListItem
                                                       {
                                                           Text = u.Fname,
                                                           Value = u.DocNum.ToString()
                                                       };
            IEnumerable<SelectListItem> SubFolderLv2 = from u in db.OFOTs.Where(u => u.Flevel == 5)
                                                       select new SelectListItem
                                                       {
                                                           Text = u.Fname,
                                                           Value = u.DocNum.ToString()
                                                       };

            IEnumerable<SelectListItem> BC_KeepBarCodePage = new SelectListItem[]
            {
                new SelectListItem() { Text = "Keep", Value = "K", Selected=true },
                new SelectListItem() { Text = "Remove", Value = "R"},
            };


            ViewBag.CompList = CompList.OrderBy(x => x.Text);
            ViewBag.DeptList = DeptList.OrderBy(x => x.Text);
            ViewBag.FolderList = FolderList.OrderBy(x => x.Text);
            ViewBag.SubFolderLv1 = SubFolderLv1.OrderBy(x => x.Text);
            ViewBag.SubFolderLv2 = SubFolderLv2.OrderBy(x => x.Text);
            ViewBag.BC_KeepBarCodePage = BC_KeepBarCodePage;


            switch (BtnSubmit)
            {

                case "Create":

                    if ((ModelState.IsValid) && ((m.BC_No_End - m.BC_No_Start <= 99) || (m.BC_No_End.ToString() == "" && m.BC_No_Start.ToString() == "") ))
                    {

                        // Get user selected values //


                        tCompList = m.CompList.ToString("D8");
                        tDeptList = m.DeptList.ToString("D8");
                        tFolderList = m.FolderList.ToString("D8");
                        tSubFolderLv1 = m.SubFolderLv1?.ToString("D8"); // add a ? after the variable because it is a nullable int
                        tSubFolderLv2 = m.SubFolderLv2?.ToString("D8"); // add a ? after the variable because it is a nullable int
                        tCompanyHeader = m.SelectedCompany;
                        tSelectedCompany = (from u in db.OFOTs where u.DocNum == m.CompList select u.Fname).FirstOrDefault().ToString();
                        tSelectedDepartment = (from u in db.OFOTs where u.DocNum == m.DeptList select u.Fname).FirstOrDefault().ToString();
                        tSelectedFld = m.SelectedFolder;
                        tSelectedSubFldLv1 = m.SelectedSubFoldeLvl1;
                        tSelectedSubFldLv2 = m.SelectedSubFoldeLvl2;

                        //return Content(tSelectedSubFldLv2);


                        //Get BC_Number //
                        // Company, Department and Folder must not be empty, that means, file can only saved under Folder level //
                        if (tSelectedSubFldLv2 != null)
                        {
                            BC_Number = m.BC_KeepBarCodePage + tSubFolderLv2 + m.BC_File_Name;
                            BC_FileName = m.BC_File_Name;
                            docpath = String.Format(@"M:\{0}\{1}\{2}\{3}\{4}\", tSelectedCompany, tSelectedDepartment, tSelectedFld, tSelectedSubFldLv1, tSelectedSubFldLv2);

                        }
                        else
                        {
                            if (tSelectedSubFldLv1 != null)
                            {
                                BC_Number = m.BC_KeepBarCodePage + tSubFolderLv1 + m.BC_File_Name;
                                BC_FileName = m.BC_File_Name;
                                docpath = String.Format(@"M:\{0}\{1}\{2}\{3}\", tSelectedCompany, tSelectedDepartment, tSelectedFld, tSelectedSubFldLv1);

                            }
                            else
                            {
                                BC_Number = m.BC_KeepBarCodePage + tFolderList + m.BC_File_Name;
                                BC_FileName = m.BC_File_Name;
                                docpath = String.Format(@"M:\{0}\{1}\{2}\", tSelectedCompany, tSelectedDepartment, tSelectedFld);
                            }
                        }

                        //                        BC_Number = "00000010";

                        //docpath = "Company= " + m.SelectedCompany + " \\Dept = " + m.SelectedDepartment + "\\Folder=" + m.SelectedFolder + "\\SubFolderLvl1=" + m.SelectedSubFoldeLvl1 + "\\SubFolderLvl2=" + m.SelectedSubFoldeLvl2 + "\\BarcodeSheet=" + m.BC_KeepBarCodePage;


                        // Passed the Criteria Checking: defined at vmCreateSOBarCodeSheet and (End-Start) <=99 (total 100 sheets) //
                        // Define the Output PDF file name //

                        TempTimeStamp = "-" + DateTime.Now.ToString("yyyymmddhhmmss");
                        if (m.BC_No_End.ToString() != "" && m.BC_No_Start.ToString() != "")
                        {
                            BC_pdfFile = BC_Number.Replace(" ","-") + "-" + m.BC_No_Start?.ToString("D7") + "-" + m.BC_No_End?.ToString("D7") + TempTimeStamp + ".pdf";
                            UserLink = UserLink + BC_pdfFile;
                            BC_pdfFile = TempFolder + BC_Number.Replace(" ", "-") + "-" + m.BC_No_Start?.ToString("D7") + "-" + m.BC_No_End?.ToString("D7") + TempTimeStamp + ".pdf";
                        }
                        else
                        {
                            BC_pdfFile = BC_Number.Replace(" ","-") +  TempTimeStamp + ".pdf";
                            UserLink = UserLink + BC_pdfFile;
                            BC_pdfFile = TempFolder + BC_Number.Replace(" ", "-") + TempTimeStamp + ".pdf";
                        }

                        //BC_string = BC_KR + BC_Number + m.BC_File_Name;

                        // Create the output File (create of barcode image in the module) //
                        // Pass the SO start number and End number into the module //
                        if (m.BC_No_Start > 0 && m.BC_No_End > 0)
                        {
                            CreatePDFSeq(m.BC_No_Start, m.BC_No_End);
                        } else
                        {
                            CreatePDF();
                        }

                        // Generate message to user & set the hyperlink to display the SO BarCode Sheet //
                        ViewBag.Message = "Barcode Sheet is created, please click the link to review or print :";
                        ViewBag.Link = UserLink;

                        //return Content("Com = " + tCompList + " -Dept =  " + tDeptList + " -Folder = "+tFolderList);
                        //  return Content(tSelectedCompany + " \\" + tSelectedDepartment + "  DocPaht=" + docpath);
                        return View("Index");
                    }
                    else if (m.BC_No_End - m.BC_No_Start < 0)
                    {
                        ViewBag.message = "End No must be greater than or equal to Start No.";
                        return View("Index");
                    }
                    else
                    {
                        
                        ViewBag.Message = "Print Range should not greater than 100, please re-enter !";
                        return View("Index");
                    }

            }
            return new EmptyResult();
        }

        private string getText(IEnumerable<SelectListItem> compList)
        {
            var text = compList.Where(x => x.Selected).FirstOrDefault().Text;
            return text;
            //throw new NotImplementedException();
        }

        private string getFolderName(int? id)
        {
            var fldName = (from u in db.OFOTs
                           where (u.DocNum == id)
                           select u.Fname).FirstOrDefault().ToString();
            return fldName;
        }

        private void CreatePDF()
        {
            //Define the PDF document and start to write the content //
            // Create a writer that listens to the document //
            iTextSharp.text.Document document = new iTextSharp.text.Document(iTextSharp.text.PageSize.A4);
            PdfWriter writer = PdfWriter.GetInstance(document, new FileStream(BC_pdfFile, FileMode.OpenOrCreate));
            document.Open();

            //Get 
            BC_print = BC_Number;
            CreateBarCodeImage(BC_print);
            System.Drawing.Image image = System.Drawing.Image.FromFile(TempFolder + BC_print + TempTimeStamp + ".bmp");
            try
            {
                BaseFont bfheader = BaseFont.CreateFont(BaseFont.TIMES_ROMAN, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
                iTextSharp.text.Font pdffontheader = new iTextSharp.text.Font(bfheader, 24);
                BaseFont bflines = BaseFont.CreateFont(BaseFont.COURIER, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
                iTextSharp.text.Font pdffontlines = new iTextSharp.text.Font(bflines, 12);
                iTextSharp.text.Paragraph pdfhead = new iTextSharp.text.Paragraph("\n\n\n\n\n\n" + " Micro-Pak DMS", pdffontheader); // (\n = new line) //
                iTextSharp.text.Paragraph pdfline0 = new iTextSharp.text.Paragraph("     ", pdffontlines); //empty line
                iTextSharp.text.Paragraph pdfline1 = new iTextSharp.text.Paragraph(BC_print, pdffontlines);
                iTextSharp.text.Paragraph pdfline2 = new iTextSharp.text.Paragraph("Company    : " + tCompanyHeader, pdffontlines);
                iTextSharp.text.Paragraph pdfline3 = new iTextSharp.text.Paragraph("This document will be saved at: ", pdffontlines);
                iTextSharp.text.Paragraph pdfline4 = new iTextSharp.text.Paragraph(docpath + BC_FileName + @"-*.pdf", pdffontlines);
                iTextSharp.text.Paragraph pdfline5 = new iTextSharp.text.Paragraph(@"* represents the file version (e.g. 001, 002) of the document.", pdffontlines);
                document.NewPage();

                // Add Header to PDF document //
                pdfhead.Alignment = 1;
                document.Add(pdfhead);

                // Add BarCode Image to PDF document //
                iTextSharp.text.Image pdfbcimg = iTextSharp.text.Image.GetInstance(image, System.Drawing.Imaging.ImageFormat.Bmp);
                pdfbcimg.Alignment = 1;
                document.Add(pdfbcimg);
                image.Dispose();
                System.IO.File.Delete(TempFolder + BC_print + TempTimeStamp + ".bmp");


                // Add pdfLine1 to pdfLine4 to PDF document //

                pdfline1.Alignment = 1;
                document.Add(pdfline1);

                // Add empty line //
                pdfline0.Alignment = 0;
                document.Add(pdfline0);

                pdfline2.IndentationLeft = 135;
                pdfline2.Alignment = 0;
                document.Add(pdfline2);

                pdfline3.IndentationLeft = 135;
                pdfline3.Alignment = 0;
                document.Add(pdfline3);

                pdfline0.Alignment = 0;
                document.Add(pdfline0);

                pdfline4.IndentationLeft = 100;
                pdfline4.IndentationRight = 100;
                pdfline4.Alignment = 0;
                document.Add(pdfline4);

                pdfline0.Alignment = 0;
                document.Add(pdfline0);

                pdfline5.IndentationLeft = 135;
                pdfline5.IndentationRight = 135;
                pdfline5.Alignment = 0;
                document.Add(pdfline5);


            }
            catch (iTextSharp.text.DocumentException)
            {
                ViewBag.Message = "BarCode Sheet Genearation Failed: Document creation error !";
            }
            catch (IOException)
            {
                ViewBag.Message = "BarCode Sheet Genearation Failed: Cannot write PDF file to destination !";
            }

            ViewBag.Message = "BarCode Sheet is Generated !";

            document.Close();
            
        }


        private void CreatePDFSeq(int? iStart, int? iEnd)        // Creation of SO BarCode Sheet - PDF File //
        {
            // Define the PDF document and start to write the content //
            // Create a writer that listens to the document //
            iTextSharp.text.Document document = new iTextSharp.text.Document(iTextSharp.text.PageSize.A4);
            PdfWriter writer = PdfWriter.GetInstance(document, new FileStream(BC_pdfFile, FileMode.OpenOrCreate));
            document.Open();

            // Generate the Content from Start to End //
            int i;
            for (i = iStart.GetValueOrDefault(); i <= iEnd.GetValueOrDefault(); i++)
            {
                // Create the BarCode Image based on the formatted  number //
                BC_print = BC_Number + "-" + i.ToString("D3");
                CreateBarCodeImage(BC_print);
                System.Drawing.Image image = System.Drawing.Image.FromFile(TempFolder + BC_print + TempTimeStamp + ".bmp");

                // Write the content to the file //
                try
                {

                    // Define Document Font type //
                    // Define each line content //
                    // and directs a PDF-stream to a file //

                    BaseFont bfheader = BaseFont.CreateFont(BaseFont.TIMES_ROMAN, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
                    iTextSharp.text.Font pdffontheader = new iTextSharp.text.Font(bfheader, 24);
                    BaseFont bflines = BaseFont.CreateFont(BaseFont.COURIER, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
                    iTextSharp.text.Font pdffontlines = new iTextSharp.text.Font(bflines, 12);
                    iTextSharp.text.Paragraph pdfhead = new iTextSharp.text.Paragraph("\n\n\n\n\n\n" + " Micro-Pak DMS", pdffontheader); // (\n = new line) //
                    iTextSharp.text.Paragraph pdfline0 = new iTextSharp.text.Paragraph("     ", pdffontlines); //empty line
                    iTextSharp.text.Paragraph pdfline1 = new iTextSharp.text.Paragraph(BC_print, pdffontlines);
                    iTextSharp.text.Paragraph pdfline2 = new iTextSharp.text.Paragraph("Company    : " + tCompanyHeader, pdffontlines);
                    iTextSharp.text.Paragraph pdfline3 = new iTextSharp.text.Paragraph("This document will be saved at: ", pdffontlines);
                    iTextSharp.text.Paragraph pdfline4 = new iTextSharp.text.Paragraph(docpath + BC_FileName + "-" + i.ToString("D3") + @"-*.pdf", pdffontlines);
                    iTextSharp.text.Paragraph pdfline5 = new iTextSharp.text.Paragraph(@"* represents the file version (e.g. 001, 002) of the document.", pdffontlines);

                    document.NewPage();

                    // Add Header to PDF document //
                    pdfhead.Alignment = 1;
                    document.Add(pdfhead);

                    // Add BarCode Image to PDF document //
                    iTextSharp.text.Image pdfbcimg = iTextSharp.text.Image.GetInstance(image, System.Drawing.Imaging.ImageFormat.Bmp);
                    pdfbcimg.Alignment = 1;
                    document.Add(pdfbcimg);
                    image.Dispose();
                    System.IO.File.Delete(TempFolder + BC_print + TempTimeStamp + ".bmp");


                    // Add pdfLine1 to pdfLine4 to PDF document //

                    pdfline1.Alignment = 1;
                    document.Add(pdfline1);

                    // Add empty line //
                    pdfline0.Alignment = 0;
                    document.Add(pdfline0);

                    pdfline2.IndentationLeft = 135;
                    pdfline2.Alignment = 0;
                    document.Add(pdfline2);

                    pdfline3.IndentationLeft = 135;
                    pdfline3.Alignment = 0;
                    document.Add(pdfline3);

                    pdfline0.Alignment = 0;
                    document.Add(pdfline0);

                    pdfline4.IndentationRight = 135;
                    pdfline4.IndentationLeft = 135;
                    pdfline4.Alignment = 0;
                    document.Add(pdfline4);

                    pdfline0.Alignment = 0;
                    document.Add(pdfline0);

                    pdfline5.IndentationLeft = 135;
                    pdfline5.IndentationRight = 135;
                    pdfline5.Alignment = 0;
                    document.Add(pdfline5);


                }
                catch (iTextSharp.text.DocumentException)
                {
                    ViewBag.Message = "BarCode Sheet Genearation Failed: Document creation error !";
                }
                catch (IOException)
                {
                    ViewBag.Message = "BarCode Sheet Genearation Failed: Cannot write PDF file to destination !";
                }



                ViewBag.Message = "BarCode Sheet is Generated !";

            }

            document.Close();
        }


        private void CreateBarCodeImage(string BCprintcode)   // Create BarCode Image in bitmap format//
        {
            try
            {
                // Define BarCode image format //
                IBarcodeWriter BarCodeimgage = new BarcodeWriter
                {
                    Format = BarcodeFormat.CODE_128,
                    Options = new EncodingOptions { PureBarcode = true, Width = 300, Height = 80, Margin = 0 }
                };

                // Generate the barcode imaage and save as bitmap file. //
                var result = BarCodeimgage.Write(BCprintcode);
                Bitmap Barcodebmp = new Bitmap(result);

                using (var stream = new FileStream(TempFolder + BCprintcode + TempTimeStamp + ".bmp", FileMode.OpenOrCreate, FileAccess.ReadWrite))
                {
                    var BarcodeAsBytes = ImageToByte(Barcodebmp);
                    stream.Write(BarcodeAsBytes, 0, BarcodeAsBytes.Length);
                }

                ViewBag.Message = "BarCode Image is created !";
            }
            catch (Exception)
            {
                ViewBag.Message = "Sorry, Bar Code Genearation Failed: Cannot write image file to destination !";
            }
        }


        public static byte[] ImageToByte(Image img)   // Part of BarCode generation - conversion // 
        {
            ImageConverter converter = new ImageConverter();
            return (byte[])converter.ConvertTo(img, typeof(byte[]));
        }


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        public static string getText(SelectList selectList)
        {
            string text = selectList.Where(x => x.Selected).FirstOrDefault().Text;
            return text;
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

        private int getID()
        {
            HttpCookie authCookie = Request.Cookies[FormsAuthentication.FormsCookieName];
            FormsAuthenticationTicket ticket = FormsAuthentication.Decrypt(authCookie.Value);
            string[] data = ticket.UserData.Split(",".ToCharArray());
            string userID = data[0];
            int ID = int.Parse(userID);

            return ID;
        }

        #endregion
    }
}
