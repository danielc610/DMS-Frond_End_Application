using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.IO;
using System.Drawing.Imaging;
using System.Drawing.Printing;
using System.Web;
using System.Web.Mvc;
using iTextSharp.text.pdf;
using ZXing;
using ZXing.Common;
using ZXing.Rendering;
using DMS_FEA.Models;
using DMS_FEA.ViewModels;


namespace DMS_FEA.Controllers
{
    [Authorize]
    public class vCreateSOBarCodeSheetController : Controller
    {
        // Define variables to be used in this controller //

        //private string TempFolder = @"C:\DATA\"; //Destination of Image File and Output File //
        private string TempFolder = @"C:\inetpub\wwwroot\DMS-P01\DMS_Temp\"; //Destination of Image File and Output File //
        private string UserLink = "http://dms.asiantat.com/dms_Temp/";
        private string TempTimeStamp = ""; //TimeStamp is used as part of file name //
        private string SO_pdfFile = ""; //Output File Name //
        private string SO_Number = ""; // SO Number used to generate barcode //


        //First reponse view to user request //
        public ActionResult Index()
        {
            return View();
        }

        // User Press [Generate] Button on the View //
        [HttpPost]
        public ActionResult Generate(vmCreateSOBarCodeSheet m, string BtnSubmit)
        {
            switch (BtnSubmit)
            {
                case "Create":
                    if ((ModelState.IsValid) && (m.SO_No_End - m.SO_No_Start <= 99))
                    {
                        // Passed the Criteria Checking: defined at vmCreateSOBarCodeSheet and (End-Start) <=99 (total 100 sheets) //
                        // Define the Output PDF file name //
                        TempTimeStamp = "-" + DateTime.Now.ToString("yyyymmddhhmmss");
                        // SO_pdfFile = TempFolder + "SO" + m.SO_No_Start.ToString("D7") + "-" + m.SO_No_End.ToString("D7").Substring(3) + TempTimeStamp + ".pdf";
                        //SO_pdfFile = UserLink + "SO" + m.SO_No_Start.ToString("D7") + "-" + m.SO_No_End.ToString("D7").Substring(3) + TempTimeStamp + ".pdf";

                        //Case if revision no. is null or less than 1
                        if (m.Revision_No == null || m.Revision_No < 1)
                        {
                            SO_pdfFile = TempFolder + "SO-" + m.SO_No_Start.ToString("D7") + "-" + m.SO_No_End.ToString("D7").Substring(3) + TempTimeStamp + ".pdf";
                            UserLink = UserLink + "SO-" + m.SO_No_Start.ToString("D7") + "-" + m.SO_No_End.ToString("D7").Substring(3) + TempTimeStamp + ".pdf";
                            // Create the output File (create of barcode image in the module) //
                            // Pass the SO start number and End number into the module //
                            CreatePDF(m.SO_No_Start, m.SO_No_End, m.Revision_No);

                        }
                        // case ot revison no. is greater than and equal to 1
                        else if (m.Revision_No >= 1)
                        {
                            SO_pdfFile = TempFolder + "SO-" + m.SO_No_Start.ToString("D7") + "-" + m.SO_No_End.ToString("D7").Substring(3)+ "-R" + m.Revision_No.ToString() + TempTimeStamp + ".pdf";
                            UserLink = UserLink + "SO-" + m.SO_No_Start.ToString("D7") + "-" + m.SO_No_End.ToString("D7").Substring(3)+ "-R" + m.Revision_No.ToString() + TempTimeStamp + ".pdf";
                            // Create the output File (create of barcode image in the module) //
                            // Pass the SO start number and End number into the module //
                            CreatePDF(m.SO_No_Start, m.SO_No_End, m.Revision_No);
                        }


                        // Generate message to user & set the hyperlink to display the SO BarCode Sheet //
                        ViewBag.Message = "Barcode Sheet is created, please click the link to review or print :";
                        ViewBag.Link = UserLink;

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


        private void CreatePDF(int iStart, int iEnd, int? iRev)        // Creation of SO BarCode Sheet - PDF File //
        {
            // Define the PDF document and start to write the content //
            // Create a writer that listens to the document //
            iTextSharp.text.Document document = new iTextSharp.text.Document(iTextSharp.text.PageSize.A4);
            PdfWriter writer = PdfWriter.GetInstance(document, new FileStream(@SO_pdfFile, FileMode.OpenOrCreate));
            document.Open();

            // Generate the Content from Start to End //
            int i;
            for (i = iStart; i <= iEnd; i++)
            {
                if (iRev == null)
                {
                    // Create the BarCode Image based on the formatted SO number //
                    SO_Number = "SO-" + i.ToString("D7");
                    CreateBarCodeImage(SO_Number);

                }
                else if(iRev >= 1)
                {
                    // Create the BarCode Image based on the formatted SO number //
                    SO_Number = "SO-" + i.ToString("D7") + "-R" + iRev.ToString();
                    CreateBarCodeImage(SO_Number);
                }
                    System.Drawing.Image image = System.Drawing.Image.FromFile(TempFolder + SO_Number + TempTimeStamp + ".bmp");

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
                    iTextSharp.text.Paragraph pdfline1 = new iTextSharp.text.Paragraph(SO_Number, pdffontlines);
                    iTextSharp.text.Paragraph pdfline2 = new iTextSharp.text.Paragraph("Company : Micro-Pak Ltd", pdffontlines);
                    iTextSharp.text.Paragraph pdfline3 = new iTextSharp.text.Paragraph("This SO Document will be saved at: ", pdffontlines);
                    iTextSharp.text.Paragraph pdfline4 = new iTextSharp.text.Paragraph("M:\\MicroPak\\SO\\" + SO_Number.Substring(3, 3) + "0000\\" + SO_Number.Substring(3, 4) + "000\\", pdffontlines);

                    iTextSharp.text.Paragraph pdfline5 = new iTextSharp.text.Paragraph("Document will be filled to sub-folder LG or OP based on the scan destination selected.", pdffontlines);


                    document.NewPage();

                    // Add Header to PDF document //
                    pdfhead.Alignment = 1;
                    document.Add(pdfhead);

                    // Add BarCode Image to PDF document //
                    iTextSharp.text.Image pdfbcimg = iTextSharp.text.Image.GetInstance(image, System.Drawing.Imaging.ImageFormat.Bmp);
                    pdfbcimg.Alignment = 1;
                    document.Add(pdfbcimg);
                    image.Dispose();
                    System.IO.File.Delete(TempFolder + SO_Number + TempTimeStamp + ".bmp");


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


        private void CreateBarCodeImage(string SOnumber)   // Create BarCode Image in bitmap format//
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
                var result = BarCodeimgage.Write(SOnumber);
                Bitmap Barcodebmp = new Bitmap(result);

                using (var stream = new FileStream(TempFolder + SO_Number + TempTimeStamp + ".bmp", FileMode.OpenOrCreate, FileAccess.ReadWrite))
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
    }
}