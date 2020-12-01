

using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Nexmo.Api;
using Quartz;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using UserRoles.Models;
using PageSize = iTextSharp.text.PageSize;
using Font = iTextSharp.text.Font;
using Rectangle = iTextSharp.text.Rectangle;
using iTextSharp.text;

using iTextSharp.text.pdf;
using iTextSharp.text.html;

namespace UserRoles.Controllers
{
    public class EmailJobController  : Controller, IJob
    {
        // GET: EmailJob
        //public ActionResult Index()
        //{
        //    return View();
        //}


        //[Obsolete]
        [Obsolete]
        public async Task Execute(IJobExecutionContext context)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            try
            {

                //Self Collection day before
                var selfC = (from x in db.Orders
                             join i in db.Maps
                             on x.OrderID equals i.orderID
                             where i.Distance == 0
                             && x.PickUp == false
                             && x.Seen == true
                             && x.DriverID == null
                             //&& x.CollDate == DateTime.Today
                             && x.ReminderDate == DateTime.Today
                             //&& x.CollDate == DateTime.Today.AddDays(-1)
                             select x);
                foreach (var item in selfC)
                {
                    string Email = item.Email;
                    string phone = item.CustomerPhone;
                    MemoryStream memoryStream = new MemoryStream();
                    //start of css 
                    try
                    {
                        StringBuilder status = new StringBuilder("");
                        var doc = new iTextSharp.text.Document(PageSize.A4, 10, 10, 10, 10);
                        PdfWriter writer = PdfWriter.GetInstance(doc, memoryStream);
                        var titleFont = FontFactory.GetFont("Arial", 12, Font.BOLD);
                        var titleFontBlue = FontFactory.GetFont("Arial", 14, Font.NORMAL, BaseColor.BLUE);
                        var boldTableFont = FontFactory.GetFont("Arial", 8, Font.BOLD);
                        var bodyFont = FontFactory.GetFont("Arial", 8, Font.NORMAL);
                        var EmailFont = FontFactory.GetFont("Arial", 8, Font.NORMAL, BaseColor.BLUE);
                        BaseColor TabelHeaderBackGroundColor = WebColors.GetRGBColor("#EEEEEE");

                        //end of css

                        //start of header

                        Rectangle pageSize = writer.PageSize;
                        doc.Open();
                        PdfPTable header = new PdfPTable(3);
                        header.HorizontalAlignment = 0;
                        header.WidthPercentage = 100;
                        header.SetWidths(new float[] { 100f, 320f, 100f });
                        header.DefaultCell.Border = Rectangle.NO_BORDER;

                        //Image logo = Image.GetInstance(System.Web.HttpContext.Current.Server.MapPath("~/Content/images/1.jpg"));
                        //string imageURL = System.Web.HttpContext.Current.Server.MapPath("~/Content/Images/GP_BLUE.jpg") ;
                        //iTextSharp.text.Image logo = iTextSharp.text.Image.GetInstance(imageURL);

                        //logo.ScaleToFit(100, 100);
                        {
                            PdfPCell pdfCelllogo = new PdfPCell(/*logo*/);
                            pdfCelllogo.Border = Rectangle.NO_BORDER;
                            pdfCelllogo.BorderColorBottom = new BaseColor(System.Drawing.Color.Black);
                            pdfCelllogo.BorderWidthBottom = 1f;
                            header.AddCell(pdfCelllogo);
                        }
                        {
                            PdfPCell middlecell = new PdfPCell();
                            middlecell.Border = Rectangle.NO_BORDER;
                            middlecell.BorderColorBottom = new BaseColor(System.Drawing.Color.Black);
                            middlecell.BorderWidthBottom = 1f;
                            header.AddCell(middlecell);
                        }
                        {
                            PdfPTable nested = new PdfPTable(1);
                            nested.DefaultCell.Border = Rectangle.NO_BORDER;
                            PdfPCell nextPostCell1 = new PdfPCell(new Phrase("Gold Pride", titleFont));
                            nextPostCell1.Border = Rectangle.NO_BORDER;
                            nested.AddCell(nextPostCell1);
                            PdfPCell nextPostCell2 = new PdfPCell(new Phrase("90 street, Durban, SA,", bodyFont));
                            nextPostCell2.Border = Rectangle.NO_BORDER;
                            nested.AddCell(nextPostCell2);
                            PdfPCell nextPostCell3 = new PdfPCell(new Phrase("(082) 0798501", bodyFont));
                            nextPostCell3.Border = Rectangle.NO_BORDER;
                            nested.AddCell(nextPostCell3);
                            PdfPCell nextPostCell4 = new PdfPCell(new Phrase("dalphene@gmail.com", EmailFont));
                            nextPostCell4.Border = Rectangle.NO_BORDER;
                            nested.AddCell(nextPostCell4);
                            nested.AddCell("");
                            PdfPCell nesthousing = new PdfPCell(nested);
                            nesthousing.Border = Rectangle.NO_BORDER;
                            nesthousing.BorderColorBottom = new BaseColor(System.Drawing.Color.Black);
                            nesthousing.BorderWidthBottom = 1f;
                            nesthousing.Rowspan = 5;
                            nesthousing.PaddingBottom = 10f;
                            header.AddCell(nesthousing);
                        }
                        PdfPTable Invoicetable = new PdfPTable(3);
                        Invoicetable.HorizontalAlignment = 0;
                        Invoicetable.WidthPercentage = 100;
                        Invoicetable.SetWidths(new float[] { 100f, 320f, 100f });  // then set the column's __relative__ widths
                        Invoicetable.DefaultCell.Border = Rectangle.NO_BORDER;

                        {

                            PdfPTable nested = new PdfPTable(1);
                            nested.DefaultCell.Border = Rectangle.NO_BORDER;
                            PdfPCell nextPostCell1 = new PdfPCell(new Phrase("Delivery Scheduled for: " + item.CustomerName, bodyFont));
                            nextPostCell1.Border = Rectangle.NO_BORDER;
                            nested.AddCell(nextPostCell1);
                            PdfPCell nextPostCell2 = new PdfPCell(new Phrase("Customer Phone " + item.CustomerPhone, bodyFont));
                            nextPostCell2.Border = Rectangle.NO_BORDER;
                            nested.AddCell(nextPostCell2);
                            PdfPCell nextPostCell3 = new PdfPCell(new Phrase("", bodyFont));
                            nextPostCell3.Border = Rectangle.NO_BORDER;
                            nested.AddCell(nextPostCell3);
                            PdfPCell nextPostCell4 = new PdfPCell(new Phrase(" " + item.Email, EmailFont));
                            nextPostCell4.Border = Rectangle.NO_BORDER;
                            nested.AddCell(nextPostCell4);
                            nested.AddCell("");
                            PdfPCell nesthousing = new PdfPCell(nested);
                            nesthousing.Border = Rectangle.NO_BORDER;
                            nesthousing.BorderColorBottom = new BaseColor(System.Drawing.Color.Black);
                            nesthousing.BorderWidthBottom = 1f;
                            nesthousing.Rowspan = 5;
                            nesthousing.PaddingBottom = 10f;
                            Invoicetable.AddCell(nesthousing);
                        }
                        {
                            PdfPCell middlecell = new PdfPCell();
                            middlecell.Border = Rectangle.NO_BORDER;
                            middlecell.BorderColorBottom = new BaseColor(System.Drawing.Color.Black);
                            middlecell.BorderWidthBottom = 1f;
                            Invoicetable.AddCell(middlecell);
                        }

                        {
                            PdfPTable nested = new PdfPTable(1);
                            nested.DefaultCell.Border = Rectangle.NO_BORDER;
                            PdfPCell nextPostCell1 = new PdfPCell(new Phrase("Order ID: " + item.OrderID, bodyFont));
                            nextPostCell1.Border = Rectangle.NO_BORDER;
                            nested.AddCell(nextPostCell1);

                            PdfPCell nextPostCell2 = new PdfPCell(new Phrase("Date of payment: " + item.OrderDate.ToString("dd/MM/yyyy HH:mm:ss"), bodyFont));
                            nextPostCell2.Border = Rectangle.NO_BORDER;
                            nested.AddCell(nextPostCell2);

                            PdfPCell nextPostCell3 = new PdfPCell(new Phrase("Payment Type:  " + "PayFast", bodyFont));
                            nextPostCell3.Border = Rectangle.NO_BORDER;
                            nested.AddCell(nextPostCell3);

                            PdfPCell nextPostCell4 = new PdfPCell(new Phrase(" ", bodyFont));
                            nextPostCell4.Border = Rectangle.NO_BORDER;
                            nested.AddCell(nextPostCell4);


                            PdfPCell nextPostCell5 = new PdfPCell(new Phrase("", bodyFont));
                            nextPostCell5.Border = Rectangle.NO_BORDER;
                            nested.AddCell(nextPostCell5);


                            PdfPCell nextPostCell6 = new PdfPCell(new Phrase("", bodyFont));
                            nextPostCell6.Border = Rectangle.NO_BORDER;
                            nested.AddCell(nextPostCell6);

                            PdfPCell nextPostCell7 = new PdfPCell(new Phrase("", bodyFont));
                            nextPostCell7.Border = Rectangle.NO_BORDER;
                            nested.AddCell(nextPostCell7);

                            //PdfPCell nextPostCell6 = new PdfPCell(new Phrase("Collectors Address" + order.CustomerName, bodyFont));
                            //nextPostCell5.Border = Rectangle.NO_BORDER;
                            //nested.AddCell(nextPostCell6);

                            PdfPCell nextPostCell8 = new PdfPCell(new Phrase(" " /*+ "\n"*/, bodyFont));
                            nextPostCell8.Border = Rectangle.NO_BORDER;
                            nested.AddCell(nextPostCell8);

                            PdfPCell nextPostCell9 = new PdfPCell(new Phrase("", bodyFont));
                            nextPostCell9.Border = Rectangle.NO_BORDER;
                            nested.AddCell(nextPostCell9);



                            nested.AddCell("");
                            PdfPCell nesthousing = new PdfPCell(nested);
                            nesthousing.Border = Rectangle.NO_BORDER;
                            nesthousing.BorderColorBottom = new BaseColor(System.Drawing.Color.Black);
                            nesthousing.BorderWidthBottom = 1f;
                            nesthousing.Rowspan = 5;
                            nesthousing.PaddingBottom = 10f;
                            Invoicetable.AddCell(nesthousing);
                        }


                        doc.Add(header);
                        Invoicetable.PaddingTop = 10f;

                        doc.Add(Invoicetable);
                        Order order = new Order();

                        doc.Add(new Paragraph("GoodDay " + item.CustomerName + "\n" + "\n" +
                            "This is a reminder for you to collect order" + "\n" +
                            "tomorrow" + DateTime.Now.AddDays(1).ToString("dd/MM/yyyy HH:mm") + "\n" +
                            "\n" +
                            //"Please note that they need to be returned by the " + item.ExpectedReturnDate.ToString("dd/MM?yyyy") +
                            "Thank you for using GoldPride for your item needs and we hope to see you again" + "\n" +
                            "If this was not you please contact us "
                            + "\n" + "\n" + "\n" + "\n" +
                            "Have  a good day " + "\n" +
                            "Gold Pride"));

                        PdfContentByte cb = new PdfContentByte(writer);


                        BaseFont bf = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1250, true);
                        cb = new PdfContentByte(writer);
                        cb = writer.DirectContent;
                        cb.BeginText();
                        cb.SetFontAndSize(bf, 8);
                        cb.SetTextMatrix(pageSize.GetLeft(120), 20);
                        cb.ShowText("Thank you for choosing GoldPride ");
                        cb.EndText();

                        //Move the pointer and draw line to separate footer section from rest of page
                        cb.MoveTo(40, doc.PageSize.GetBottom(50));
                        cb.LineTo(doc.PageSize.Width - 40, doc.PageSize.GetBottom(50));
                        cb.Stroke();

                        writer.CloseStream = false;
                        doc.Close();

                        memoryStream.Position = 0;


                        SmtpClient client = new SmtpClient("smtp.sendgrid.net");
                        client.Port = 25;
                        client.Host = "smtp.sendgrid.net";
                        client.Timeout = 10000;
                        client.DeliveryMethod = SmtpDeliveryMethod.Network;
                        client.EnableSsl = true;
                        client.UseDefaultCredentials = false;

                        var key = Environment.GetEnvironmentVariable("apikey");

                        client.Credentials = new NetworkCredential("apikey", key/*, user, password*/);

                        System.Net.Mail.Attachment attachment;
                        attachment = new System.Net.Mail.Attachment(memoryStream, "ReminderForCollectiondbf.pdf");
                        MailMessage msz = new MailMessage(Email, Email)
                        {
                            From = new MailAddress(ConfigurationManager.AppSettings["Email"].ToString()),
                            Subject = "Order reminder for  " + item.CustomerName.ToUpper(),

                            IsBodyHtml = true,
                            Body = " Good Day : " + item.CustomerName.ToUpper() + "\n" + ", Please find attached Reminder for your order with GoldPride: " + item.OrderID,
                        };
                        msz.Attachments.Add(attachment);

                        client.Send(msz);
                    }
                    catch
                    {

                    }



                }
                //self collection day of
                var sColl = (from x in db.Orders
                             join i in db.Maps
                             on x.OrderID equals i.orderID
                             where i.Distance == 0
                             && x.PickUp == false
                             && x.Seen == true
                             && x.DriverID == null
                             && x.CollDate == DateTime.Today
                             //&& x.ReminderDate == DateTime.Today
                             select x);
                foreach (var item in sColl)
                {
                    string Email = item.Email;
                    string phone = item.CustomerPhone;
                    MemoryStream memoryStream = new MemoryStream();
                    //start of css 
                    try
                    {
                        StringBuilder status = new StringBuilder("");
                        var doc = new iTextSharp.text.Document(PageSize.A4, 10, 10, 10, 10);
                        PdfWriter writer = PdfWriter.GetInstance(doc, memoryStream);
                        var titleFont = FontFactory.GetFont("Arial", 12, Font.BOLD);
                        var titleFontBlue = FontFactory.GetFont("Arial", 14, Font.NORMAL, BaseColor.BLUE);
                        var boldTableFont = FontFactory.GetFont("Arial", 8, Font.BOLD);
                        var bodyFont = FontFactory.GetFont("Arial", 8, Font.NORMAL);
                        var EmailFont = FontFactory.GetFont("Arial", 8, Font.NORMAL, BaseColor.BLUE);
                        BaseColor TabelHeaderBackGroundColor = WebColors.GetRGBColor("#EEEEEE");

                        //end of css

                        //start of header

                        Rectangle pageSize = writer.PageSize;
                        doc.Open();
                        PdfPTable header = new PdfPTable(3);
                        header.HorizontalAlignment = 0;
                        header.WidthPercentage = 100;
                        header.SetWidths(new float[] { 100f, 320f, 100f });
                        header.DefaultCell.Border = Rectangle.NO_BORDER;

                        //Image logo = Image.GetInstance(System.Web.HttpContext.Current.Server.MapPath("~/Content/images/1.jpg"));
                        //string imageURL = System.Web.HttpContext.Current.Server.MapPath("~/Content/Images/GP_BLUE.jpg") ;
                        //iTextSharp.text.Image logo = iTextSharp.text.Image.GetInstance(imageURL);

                        //logo.ScaleToFit(100, 100);
                        {
                            PdfPCell pdfCelllogo = new PdfPCell(/*logo*/);
                            pdfCelllogo.Border = Rectangle.NO_BORDER;
                            pdfCelllogo.BorderColorBottom = new BaseColor(System.Drawing.Color.Black);
                            pdfCelllogo.BorderWidthBottom = 1f;
                            header.AddCell(pdfCelllogo);
                        }
                        {
                            PdfPCell middlecell = new PdfPCell();
                            middlecell.Border = Rectangle.NO_BORDER;
                            middlecell.BorderColorBottom = new BaseColor(System.Drawing.Color.Black);
                            middlecell.BorderWidthBottom = 1f;
                            header.AddCell(middlecell);
                        }
                        {
                            PdfPTable nested = new PdfPTable(1);
                            nested.DefaultCell.Border = Rectangle.NO_BORDER;
                            PdfPCell nextPostCell1 = new PdfPCell(new Phrase("Gold Pride", titleFont));
                            nextPostCell1.Border = Rectangle.NO_BORDER;
                            nested.AddCell(nextPostCell1);
                            PdfPCell nextPostCell2 = new PdfPCell(new Phrase("90 street, Durban, SA,", bodyFont));
                            nextPostCell2.Border = Rectangle.NO_BORDER;
                            nested.AddCell(nextPostCell2);
                            PdfPCell nextPostCell3 = new PdfPCell(new Phrase("(082) 0798501", bodyFont));
                            nextPostCell3.Border = Rectangle.NO_BORDER;
                            nested.AddCell(nextPostCell3);
                            PdfPCell nextPostCell4 = new PdfPCell(new Phrase("dalphene@gmail.com", EmailFont));
                            nextPostCell4.Border = Rectangle.NO_BORDER;
                            nested.AddCell(nextPostCell4);
                            nested.AddCell("");
                            PdfPCell nesthousing = new PdfPCell(nested);
                            nesthousing.Border = Rectangle.NO_BORDER;
                            nesthousing.BorderColorBottom = new BaseColor(System.Drawing.Color.Black);
                            nesthousing.BorderWidthBottom = 1f;
                            nesthousing.Rowspan = 5;
                            nesthousing.PaddingBottom = 10f;
                            header.AddCell(nesthousing);
                        }
                        PdfPTable Invoicetable = new PdfPTable(3);
                        Invoicetable.HorizontalAlignment = 0;
                        Invoicetable.WidthPercentage = 100;
                        Invoicetable.SetWidths(new float[] { 100f, 320f, 100f });  // then set the column's __relative__ widths
                        Invoicetable.DefaultCell.Border = Rectangle.NO_BORDER;

                        {

                            PdfPTable nested = new PdfPTable(1);
                            nested.DefaultCell.Border = Rectangle.NO_BORDER;
                            PdfPCell nextPostCell1 = new PdfPCell(new Phrase("Delivery Scheduled for: " + item.CustomerName, bodyFont));
                            nextPostCell1.Border = Rectangle.NO_BORDER;
                            nested.AddCell(nextPostCell1);
                            PdfPCell nextPostCell2 = new PdfPCell(new Phrase("Customer Phone " + item.CustomerPhone, bodyFont));
                            nextPostCell2.Border = Rectangle.NO_BORDER;
                            nested.AddCell(nextPostCell2);
                            PdfPCell nextPostCell3 = new PdfPCell(new Phrase("", bodyFont));
                            nextPostCell3.Border = Rectangle.NO_BORDER;
                            nested.AddCell(nextPostCell3);
                            PdfPCell nextPostCell4 = new PdfPCell(new Phrase(" " + item.Email, EmailFont));
                            nextPostCell4.Border = Rectangle.NO_BORDER;
                            nested.AddCell(nextPostCell4);
                            nested.AddCell("");
                            PdfPCell nesthousing = new PdfPCell(nested);
                            nesthousing.Border = Rectangle.NO_BORDER;
                            nesthousing.BorderColorBottom = new BaseColor(System.Drawing.Color.Black);
                            nesthousing.BorderWidthBottom = 1f;
                            nesthousing.Rowspan = 5;
                            nesthousing.PaddingBottom = 10f;
                            Invoicetable.AddCell(nesthousing);
                        }
                        {
                            PdfPCell middlecell = new PdfPCell();
                            middlecell.Border = Rectangle.NO_BORDER;
                            middlecell.BorderColorBottom = new BaseColor(System.Drawing.Color.Black);
                            middlecell.BorderWidthBottom = 1f;
                            Invoicetable.AddCell(middlecell);
                        }

                        {
                            PdfPTable nested = new PdfPTable(1);
                            nested.DefaultCell.Border = Rectangle.NO_BORDER;
                            PdfPCell nextPostCell1 = new PdfPCell(new Phrase("Order ID: " + item.OrderID, bodyFont));
                            nextPostCell1.Border = Rectangle.NO_BORDER;
                            nested.AddCell(nextPostCell1);

                            PdfPCell nextPostCell2 = new PdfPCell(new Phrase("Date of payment: " + item.OrderDate.ToString("dd/MM/yyyy HH:mm:ss"), bodyFont));
                            nextPostCell2.Border = Rectangle.NO_BORDER;
                            nested.AddCell(nextPostCell2);

                            PdfPCell nextPostCell3 = new PdfPCell(new Phrase("Payment Type:  " + "PayFast", bodyFont));
                            nextPostCell3.Border = Rectangle.NO_BORDER;
                            nested.AddCell(nextPostCell3);

                            PdfPCell nextPostCell4 = new PdfPCell(new Phrase(" ", bodyFont));
                            nextPostCell4.Border = Rectangle.NO_BORDER;
                            nested.AddCell(nextPostCell4);


                            PdfPCell nextPostCell5 = new PdfPCell(new Phrase("", bodyFont));
                            nextPostCell5.Border = Rectangle.NO_BORDER;
                            nested.AddCell(nextPostCell5);


                            PdfPCell nextPostCell6 = new PdfPCell(new Phrase("", bodyFont));
                            nextPostCell6.Border = Rectangle.NO_BORDER;
                            nested.AddCell(nextPostCell6);

                            PdfPCell nextPostCell7 = new PdfPCell(new Phrase("", bodyFont));
                            nextPostCell7.Border = Rectangle.NO_BORDER;
                            nested.AddCell(nextPostCell7);

                            //PdfPCell nextPostCell6 = new PdfPCell(new Phrase("Collectors Address" + order.CustomerName, bodyFont));
                            //nextPostCell5.Border = Rectangle.NO_BORDER;
                            //nested.AddCell(nextPostCell6);

                            PdfPCell nextPostCell8 = new PdfPCell(new Phrase(" " /*+ "\n"*/, bodyFont));
                            nextPostCell8.Border = Rectangle.NO_BORDER;
                            nested.AddCell(nextPostCell8);

                            PdfPCell nextPostCell9 = new PdfPCell(new Phrase("", bodyFont));
                            nextPostCell9.Border = Rectangle.NO_BORDER;
                            nested.AddCell(nextPostCell9);



                            nested.AddCell("");
                            PdfPCell nesthousing = new PdfPCell(nested);
                            nesthousing.Border = Rectangle.NO_BORDER;
                            nesthousing.BorderColorBottom = new BaseColor(System.Drawing.Color.Black);
                            nesthousing.BorderWidthBottom = 1f;
                            nesthousing.Rowspan = 5;
                            nesthousing.PaddingBottom = 10f;
                            Invoicetable.AddCell(nesthousing);
                        }


                        doc.Add(header);
                        Invoicetable.PaddingTop = 10f;

                        doc.Add(Invoicetable);

                        doc.Add(new Paragraph("GoodDay " + item.CustomerName + "\n" + "\n" +
                            "Thank you for collecting you order" + "\n" +
                            "Items Collected at" + DateTime.Now.ToString("dd/MM/yyyy HH:mm") + "\n" +
                            "\n" +
                            "Please note that they need to be returned by the " + item.ExpectedReturnDate.ToString("dd/MM?yyyy") +
                            "Thank you for using GoldPride for your item needs and we hope to see you again" + "\n" +
                            "If this was not you please contact us "
                            + "\n" + "\n" + "\n" + "\n" +
                            "Have  a good day " + "\n" +
                            "Gold Pride"));

                        PdfContentByte cb = new PdfContentByte(writer);


                        BaseFont bf = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1250, true);
                        cb = new PdfContentByte(writer);
                        cb = writer.DirectContent;
                        cb.BeginText();
                        cb.SetFontAndSize(bf, 8);
                        cb.SetTextMatrix(pageSize.GetLeft(120), 20);
                        cb.ShowText("Thank you for choosing GoldPride ");
                        cb.EndText();

                        //Move the pointer and draw line to separate footer section from rest of page
                        cb.MoveTo(40, doc.PageSize.GetBottom(50));
                        cb.LineTo(doc.PageSize.Width - 40, doc.PageSize.GetBottom(50));
                        cb.Stroke();

                        writer.CloseStream = false;
                        doc.Close();

                        memoryStream.Position = 0;


                        SmtpClient client = new SmtpClient("smtp.sendgrid.net");
                        client.Port = 25;
                        client.Host = "smtp.sendgrid.net";
                        client.Timeout = 10000;
                        client.DeliveryMethod = SmtpDeliveryMethod.Network;
                        client.EnableSsl = true;
                        client.UseDefaultCredentials = false;

                        var key = Environment.GetEnvironmentVariable("apikey");

                        client.Credentials = new NetworkCredential("apikey", key/*, user, password*/);

                        System.Net.Mail.Attachment attachment;
                        attachment = new System.Net.Mail.Attachment(memoryStream, "ReminderForCollectiondbf.pdf");
                        MailMessage msz = new MailMessage(Email, Email)
                        {
                            From = new MailAddress(ConfigurationManager.AppSettings["Email"].ToString()),
                            Subject = "Order reminder for  " + item.CustomerName.ToUpper(),

                            IsBodyHtml = true,
                            Body = " Good Day : " + item.CustomerName.ToUpper() + "\n" + ", Please find attached Reminder for your order with GoldPride: " + item.OrderID,
                        };
                        msz.Attachments.Add(attachment);

                        client.Send(msz);
                    }
                    catch
                    {

                    }


                }
                //End

                //Self Return day before

                var selfR = (from i in db.Maps
                             join
                             x in db.Orders on i.Id equals x.OrderID
                             where i.Distance == 0
                             && x.PickUp == true
                             && x.Seen == true
                             && x.DriverID == null
                             && x.SelfReturnReminder == DateTime.Today
                             select x) ;

                foreach (var item in selfR)
                {
                    string Email = item.Email;
                    string phone = item.CustomerPhone;
                    MemoryStream memoryStream = new MemoryStream();
                    //start of css 
                    try
                    {
                        StringBuilder status = new StringBuilder("");
                        var doc = new iTextSharp.text.Document(PageSize.A4, 10, 10, 10, 10);
                        PdfWriter writer = PdfWriter.GetInstance(doc, memoryStream);
                        var titleFont = FontFactory.GetFont("Arial", 12, Font.BOLD);
                        var titleFontBlue = FontFactory.GetFont("Arial", 14, Font.NORMAL, BaseColor.BLUE);
                        var boldTableFont = FontFactory.GetFont("Arial", 8, Font.BOLD);
                        var bodyFont = FontFactory.GetFont("Arial", 8, Font.NORMAL);
                        var EmailFont = FontFactory.GetFont("Arial", 8, Font.NORMAL, BaseColor.BLUE);
                        BaseColor TabelHeaderBackGroundColor = WebColors.GetRGBColor("#EEEEEE");

                        //end of css

                        //start of header

                        Rectangle pageSize = writer.PageSize;
                        doc.Open();
                        PdfPTable header = new PdfPTable(3);
                        header.HorizontalAlignment = 0;
                        header.WidthPercentage = 100;
                        header.SetWidths(new float[] { 100f, 320f, 100f });
                        header.DefaultCell.Border = Rectangle.NO_BORDER;

                        //Image logo = Image.GetInstance(System.Web.HttpContext.Current.Server.MapPath("~/Content/images/1.jpg"));
                        //string imageURL = System.Web.HttpContext.Current.Server.MapPath("~/Content/Images/GP_BLUE.jpg") ;
                        //iTextSharp.text.Image logo = iTextSharp.text.Image.GetInstance(imageURL);

                        //logo.ScaleToFit(100, 100);
                        {
                            PdfPCell pdfCelllogo = new PdfPCell(/*logo*/);
                            pdfCelllogo.Border = Rectangle.NO_BORDER;
                            pdfCelllogo.BorderColorBottom = new BaseColor(System.Drawing.Color.Black);
                            pdfCelllogo.BorderWidthBottom = 1f;
                            header.AddCell(pdfCelllogo);
                        }
                        {
                            PdfPCell middlecell = new PdfPCell();
                            middlecell.Border = Rectangle.NO_BORDER;
                            middlecell.BorderColorBottom = new BaseColor(System.Drawing.Color.Black);
                            middlecell.BorderWidthBottom = 1f;
                            header.AddCell(middlecell);
                        }
                        {
                            PdfPTable nested = new PdfPTable(1);
                            nested.DefaultCell.Border = Rectangle.NO_BORDER;
                            PdfPCell nextPostCell1 = new PdfPCell(new Phrase("Gold Pride", titleFont));
                            nextPostCell1.Border = Rectangle.NO_BORDER;
                            nested.AddCell(nextPostCell1);
                            PdfPCell nextPostCell2 = new PdfPCell(new Phrase("90 street, Durban, SA,", bodyFont));
                            nextPostCell2.Border = Rectangle.NO_BORDER;
                            nested.AddCell(nextPostCell2);
                            PdfPCell nextPostCell3 = new PdfPCell(new Phrase("(082) 0798501", bodyFont));
                            nextPostCell3.Border = Rectangle.NO_BORDER;
                            nested.AddCell(nextPostCell3);
                            PdfPCell nextPostCell4 = new PdfPCell(new Phrase("dalphene@gmail.com", EmailFont));
                            nextPostCell4.Border = Rectangle.NO_BORDER;
                            nested.AddCell(nextPostCell4);
                            nested.AddCell("");
                            PdfPCell nesthousing = new PdfPCell(nested);
                            nesthousing.Border = Rectangle.NO_BORDER;
                            nesthousing.BorderColorBottom = new BaseColor(System.Drawing.Color.Black);
                            nesthousing.BorderWidthBottom = 1f;
                            nesthousing.Rowspan = 5;
                            nesthousing.PaddingBottom = 10f;
                            header.AddCell(nesthousing);
                        }
                        PdfPTable Invoicetable = new PdfPTable(3);
                        Invoicetable.HorizontalAlignment = 0;
                        Invoicetable.WidthPercentage = 100;
                        Invoicetable.SetWidths(new float[] { 100f, 320f, 100f });  // then set the column's __relative__ widths
                        Invoicetable.DefaultCell.Border = Rectangle.NO_BORDER;

                        {

                            PdfPTable nested = new PdfPTable(1);
                            nested.DefaultCell.Border = Rectangle.NO_BORDER;
                            PdfPCell nextPostCell1 = new PdfPCell(new Phrase("Delivery Scheduled for: " + item.CustomerName, bodyFont));
                            nextPostCell1.Border = Rectangle.NO_BORDER;
                            nested.AddCell(nextPostCell1);
                            PdfPCell nextPostCell2 = new PdfPCell(new Phrase("Customer Phone " + item.CustomerPhone, bodyFont));
                            nextPostCell2.Border = Rectangle.NO_BORDER;
                            nested.AddCell(nextPostCell2);
                            PdfPCell nextPostCell3 = new PdfPCell(new Phrase("", bodyFont));
                            nextPostCell3.Border = Rectangle.NO_BORDER;
                            nested.AddCell(nextPostCell3);
                            PdfPCell nextPostCell4 = new PdfPCell(new Phrase(" " + item.Email, EmailFont));
                            nextPostCell4.Border = Rectangle.NO_BORDER;
                            nested.AddCell(nextPostCell4);
                            nested.AddCell("");
                            PdfPCell nesthousing = new PdfPCell(nested);
                            nesthousing.Border = Rectangle.NO_BORDER;
                            nesthousing.BorderColorBottom = new BaseColor(System.Drawing.Color.Black);
                            nesthousing.BorderWidthBottom = 1f;
                            nesthousing.Rowspan = 5;
                            nesthousing.PaddingBottom = 10f;
                            Invoicetable.AddCell(nesthousing);
                        }
                        {
                            PdfPCell middlecell = new PdfPCell();
                            middlecell.Border = Rectangle.NO_BORDER;
                            middlecell.BorderColorBottom = new BaseColor(System.Drawing.Color.Black);
                            middlecell.BorderWidthBottom = 1f;
                            Invoicetable.AddCell(middlecell);
                        }

                        {
                            PdfPTable nested = new PdfPTable(1);
                            nested.DefaultCell.Border = Rectangle.NO_BORDER;
                            PdfPCell nextPostCell1 = new PdfPCell(new Phrase("Order ID: " + item.OrderID, bodyFont));
                            nextPostCell1.Border = Rectangle.NO_BORDER;
                            nested.AddCell(nextPostCell1);

                            PdfPCell nextPostCell2 = new PdfPCell(new Phrase("Date of payment: " + item.OrderDate.ToString("dd/MM/yyyy HH:mm:ss"), bodyFont));
                            nextPostCell2.Border = Rectangle.NO_BORDER;
                            nested.AddCell(nextPostCell2);

                            PdfPCell nextPostCell3 = new PdfPCell(new Phrase("Payment Type:  " + "PayFast", bodyFont));
                            nextPostCell3.Border = Rectangle.NO_BORDER;
                            nested.AddCell(nextPostCell3);

                            PdfPCell nextPostCell4 = new PdfPCell(new Phrase(" ", bodyFont));
                            nextPostCell4.Border = Rectangle.NO_BORDER;
                            nested.AddCell(nextPostCell4);


                            PdfPCell nextPostCell5 = new PdfPCell(new Phrase("", bodyFont));
                            nextPostCell5.Border = Rectangle.NO_BORDER;
                            nested.AddCell(nextPostCell5);


                            PdfPCell nextPostCell6 = new PdfPCell(new Phrase("", bodyFont));
                            nextPostCell6.Border = Rectangle.NO_BORDER;
                            nested.AddCell(nextPostCell6);

                            PdfPCell nextPostCell7 = new PdfPCell(new Phrase("", bodyFont));
                            nextPostCell7.Border = Rectangle.NO_BORDER;
                            nested.AddCell(nextPostCell7);

                            //PdfPCell nextPostCell6 = new PdfPCell(new Phrase("Collectors Address" + order.CustomerName, bodyFont));
                            //nextPostCell5.Border = Rectangle.NO_BORDER;
                            //nested.AddCell(nextPostCell6);

                            PdfPCell nextPostCell8 = new PdfPCell(new Phrase(" " /*+ "\n"*/, bodyFont));
                            nextPostCell8.Border = Rectangle.NO_BORDER;
                            nested.AddCell(nextPostCell8);

                            PdfPCell nextPostCell9 = new PdfPCell(new Phrase("", bodyFont));
                            nextPostCell9.Border = Rectangle.NO_BORDER;
                            nested.AddCell(nextPostCell9);



                            nested.AddCell("");
                            PdfPCell nesthousing = new PdfPCell(nested);
                            nesthousing.Border = Rectangle.NO_BORDER;
                            nesthousing.BorderColorBottom = new BaseColor(System.Drawing.Color.Black);
                            nesthousing.BorderWidthBottom = 1f;
                            nesthousing.Rowspan = 5;
                            nesthousing.PaddingBottom = 10f;
                            Invoicetable.AddCell(nesthousing);
                        }


                        doc.Add(header);
                        Invoicetable.PaddingTop = 10f;

                        doc.Add(Invoicetable);

                        doc.Add(new Paragraph("GoodDay " + item.CustomerName + "\n" + "\n" +
                            "This is a reminder that you need to retrurn your items:" + "\n" +
                            "Tomorrow " + DateTime.Now.AddDays(1).ToString("dd/MM/yyyy HH:mm") + "\n" +
                            "\n" +
                            //"Please note that they need to be returned by the " + item.ExpectedReturnDate.ToString("dd/MM?yyyy") +
                            "Thank you for using GoldPride for your item needs and we hope to see you again" + "\n" +
                            "If this was not you please contact us "
                            + "\n" + "\n" + "\n" + "\n" +
                            "Have  a good day " + "\n" +
                            "Gold Pride"));

                        PdfContentByte cb = new PdfContentByte(writer);


                        BaseFont bf = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1250, true);
                        cb = new PdfContentByte(writer);
                        cb = writer.DirectContent;
                        cb.BeginText();
                        cb.SetFontAndSize(bf, 8);
                        cb.SetTextMatrix(pageSize.GetLeft(120), 20);
                        cb.ShowText("Thank you for choosing GoldPride ");
                        cb.EndText();

                        //Move the pointer and draw line to separate footer section from rest of page
                        cb.MoveTo(40, doc.PageSize.GetBottom(50));
                        cb.LineTo(doc.PageSize.Width - 40, doc.PageSize.GetBottom(50));
                        cb.Stroke();

                        writer.CloseStream = false;
                        doc.Close();

                        memoryStream.Position = 0;


                        SmtpClient client = new SmtpClient("smtp.sendgrid.net");
                        client.Port = 25;
                        client.Host = "smtp.sendgrid.net";
                        client.Timeout = 10000;
                        client.DeliveryMethod = SmtpDeliveryMethod.Network;
                        client.EnableSsl = true;
                        client.UseDefaultCredentials = false;

                        var key = Environment.GetEnvironmentVariable("apikey");

                        client.Credentials = new NetworkCredential("apikey", key/*, user, password*/);

                        System.Net.Mail.Attachment attachment;
                        attachment = new System.Net.Mail.Attachment(memoryStream, "ReminderForReturn.pdf");
                        MailMessage msz = new MailMessage(Email, Email)
                        {
                            From = new MailAddress(ConfigurationManager.AppSettings["Email"].ToString()),
                            Subject = "Order reminder for  " + item.CustomerName.ToUpper(),

                            IsBodyHtml = true,
                            Body = " Good Day : " + item.CustomerName.ToUpper() + "\n" + ", Please find attached Reminder for your order with GoldPride: " + item.OrderID,
                        };
                        msz.Attachments.Add(attachment);

                        client.Send(msz);
                    }
                    catch
                    {

                    }

                }
                //self return today
                var selfRe = (from i in db.Maps
                             join
                             x in db.Orders on i.Id equals x.OrderID
                             where i.Distance == 0
                             && x.PickUp == true
                             && x.Seen == true
                             && x.DriverID == null
                             && x.ExpectedReturnDate == DateTime.Today
                             select x);

                foreach (var item in selfRe)
                {
                    string Email = item.Email;
                    string phone = item.CustomerPhone;
                    MemoryStream memoryStream = new MemoryStream();
                    //start of css 
                    try
                    {
                        StringBuilder status = new StringBuilder("");
                        var doc = new iTextSharp.text.Document(PageSize.A4, 10, 10, 10, 10);
                        PdfWriter writer = PdfWriter.GetInstance(doc, memoryStream);
                        var titleFont = FontFactory.GetFont("Arial", 12, Font.BOLD);
                        var titleFontBlue = FontFactory.GetFont("Arial", 14, Font.NORMAL, BaseColor.BLUE);
                        var boldTableFont = FontFactory.GetFont("Arial", 8, Font.BOLD);
                        var bodyFont = FontFactory.GetFont("Arial", 8, Font.NORMAL);
                        var EmailFont = FontFactory.GetFont("Arial", 8, Font.NORMAL, BaseColor.BLUE);
                        BaseColor TabelHeaderBackGroundColor = WebColors.GetRGBColor("#EEEEEE");

                        //end of css

                        //start of header

                        Rectangle pageSize = writer.PageSize;
                        doc.Open();
                        PdfPTable header = new PdfPTable(3);
                        header.HorizontalAlignment = 0;
                        header.WidthPercentage = 100;
                        header.SetWidths(new float[] { 100f, 320f, 100f });
                        header.DefaultCell.Border = Rectangle.NO_BORDER;

                        //Image logo = Image.GetInstance(System.Web.HttpContext.Current.Server.MapPath("~/Content/images/1.jpg"));
                        //string imageURL = System.Web.HttpContext.Current.Server.MapPath("~/Content/Images/GP_BLUE.jpg") ;
                        //iTextSharp.text.Image logo = iTextSharp.text.Image.GetInstance(imageURL);

                        //logo.ScaleToFit(100, 100);
                        {
                            PdfPCell pdfCelllogo = new PdfPCell(/*logo*/);
                            pdfCelllogo.Border = Rectangle.NO_BORDER;
                            pdfCelllogo.BorderColorBottom = new BaseColor(System.Drawing.Color.Black);
                            pdfCelllogo.BorderWidthBottom = 1f;
                            header.AddCell(pdfCelllogo);
                        }
                        {
                            PdfPCell middlecell = new PdfPCell();
                            middlecell.Border = Rectangle.NO_BORDER;
                            middlecell.BorderColorBottom = new BaseColor(System.Drawing.Color.Black);
                            middlecell.BorderWidthBottom = 1f;
                            header.AddCell(middlecell);
                        }
                        {
                            PdfPTable nested = new PdfPTable(1);
                            nested.DefaultCell.Border = Rectangle.NO_BORDER;
                            PdfPCell nextPostCell1 = new PdfPCell(new Phrase("Gold Pride", titleFont));
                            nextPostCell1.Border = Rectangle.NO_BORDER;
                            nested.AddCell(nextPostCell1);
                            PdfPCell nextPostCell2 = new PdfPCell(new Phrase("90 street, Durban, SA,", bodyFont));
                            nextPostCell2.Border = Rectangle.NO_BORDER;
                            nested.AddCell(nextPostCell2);
                            PdfPCell nextPostCell3 = new PdfPCell(new Phrase("(082) 0798501", bodyFont));
                            nextPostCell3.Border = Rectangle.NO_BORDER;
                            nested.AddCell(nextPostCell3);
                            PdfPCell nextPostCell4 = new PdfPCell(new Phrase("dalphene@gmail.com", EmailFont));
                            nextPostCell4.Border = Rectangle.NO_BORDER;
                            nested.AddCell(nextPostCell4);
                            nested.AddCell("");
                            PdfPCell nesthousing = new PdfPCell(nested);
                            nesthousing.Border = Rectangle.NO_BORDER;
                            nesthousing.BorderColorBottom = new BaseColor(System.Drawing.Color.Black);
                            nesthousing.BorderWidthBottom = 1f;
                            nesthousing.Rowspan = 5;
                            nesthousing.PaddingBottom = 10f;
                            header.AddCell(nesthousing);
                        }
                        PdfPTable Invoicetable = new PdfPTable(3);
                        Invoicetable.HorizontalAlignment = 0;
                        Invoicetable.WidthPercentage = 100;
                        Invoicetable.SetWidths(new float[] { 100f, 320f, 100f });  // then set the column's __relative__ widths
                        Invoicetable.DefaultCell.Border = Rectangle.NO_BORDER;

                        {

                            PdfPTable nested = new PdfPTable(1);
                            nested.DefaultCell.Border = Rectangle.NO_BORDER;
                            PdfPCell nextPostCell1 = new PdfPCell(new Phrase("Delivery Scheduled for: " + item.CustomerName, bodyFont));
                            nextPostCell1.Border = Rectangle.NO_BORDER;
                            nested.AddCell(nextPostCell1);
                            PdfPCell nextPostCell2 = new PdfPCell(new Phrase("Customer Phone " + item.CustomerPhone, bodyFont));
                            nextPostCell2.Border = Rectangle.NO_BORDER;
                            nested.AddCell(nextPostCell2);
                            PdfPCell nextPostCell3 = new PdfPCell(new Phrase("", bodyFont));
                            nextPostCell3.Border = Rectangle.NO_BORDER;
                            nested.AddCell(nextPostCell3);
                            PdfPCell nextPostCell4 = new PdfPCell(new Phrase(" " + item.Email, EmailFont));
                            nextPostCell4.Border = Rectangle.NO_BORDER;
                            nested.AddCell(nextPostCell4);
                            nested.AddCell("");
                            PdfPCell nesthousing = new PdfPCell(nested);
                            nesthousing.Border = Rectangle.NO_BORDER;
                            nesthousing.BorderColorBottom = new BaseColor(System.Drawing.Color.Black);
                            nesthousing.BorderWidthBottom = 1f;
                            nesthousing.Rowspan = 5;
                            nesthousing.PaddingBottom = 10f;
                            Invoicetable.AddCell(nesthousing);
                        }
                        {
                            PdfPCell middlecell = new PdfPCell();
                            middlecell.Border = Rectangle.NO_BORDER;
                            middlecell.BorderColorBottom = new BaseColor(System.Drawing.Color.Black);
                            middlecell.BorderWidthBottom = 1f;
                            Invoicetable.AddCell(middlecell);
                        }

                        {
                            PdfPTable nested = new PdfPTable(1);
                            nested.DefaultCell.Border = Rectangle.NO_BORDER;
                            PdfPCell nextPostCell1 = new PdfPCell(new Phrase("Order ID: " + item.OrderID, bodyFont));
                            nextPostCell1.Border = Rectangle.NO_BORDER;
                            nested.AddCell(nextPostCell1);

                            PdfPCell nextPostCell2 = new PdfPCell(new Phrase("Date of payment: " + item.OrderDate.ToString("dd/MM/yyyy HH:mm:ss"), bodyFont));
                            nextPostCell2.Border = Rectangle.NO_BORDER;
                            nested.AddCell(nextPostCell2);

                            PdfPCell nextPostCell3 = new PdfPCell(new Phrase("Payment Type:  " + "PayFast", bodyFont));
                            nextPostCell3.Border = Rectangle.NO_BORDER;
                            nested.AddCell(nextPostCell3);

                            PdfPCell nextPostCell4 = new PdfPCell(new Phrase(" ", bodyFont));
                            nextPostCell4.Border = Rectangle.NO_BORDER;
                            nested.AddCell(nextPostCell4);


                            PdfPCell nextPostCell5 = new PdfPCell(new Phrase("", bodyFont));
                            nextPostCell5.Border = Rectangle.NO_BORDER;
                            nested.AddCell(nextPostCell5);


                            PdfPCell nextPostCell6 = new PdfPCell(new Phrase("", bodyFont));
                            nextPostCell6.Border = Rectangle.NO_BORDER;
                            nested.AddCell(nextPostCell6);

                            PdfPCell nextPostCell7 = new PdfPCell(new Phrase("", bodyFont));
                            nextPostCell7.Border = Rectangle.NO_BORDER;
                            nested.AddCell(nextPostCell7);

                            //PdfPCell nextPostCell6 = new PdfPCell(new Phrase("Collectors Address" + order.CustomerName, bodyFont));
                            //nextPostCell5.Border = Rectangle.NO_BORDER;
                            //nested.AddCell(nextPostCell6);

                            PdfPCell nextPostCell8 = new PdfPCell(new Phrase(" " /*+ "\n"*/, bodyFont));
                            nextPostCell8.Border = Rectangle.NO_BORDER;
                            nested.AddCell(nextPostCell8);

                            PdfPCell nextPostCell9 = new PdfPCell(new Phrase("", bodyFont));
                            nextPostCell9.Border = Rectangle.NO_BORDER;
                            nested.AddCell(nextPostCell9);



                            nested.AddCell("");
                            PdfPCell nesthousing = new PdfPCell(nested);
                            nesthousing.Border = Rectangle.NO_BORDER;
                            nesthousing.BorderColorBottom = new BaseColor(System.Drawing.Color.Black);
                            nesthousing.BorderWidthBottom = 1f;
                            nesthousing.Rowspan = 5;
                            nesthousing.PaddingBottom = 10f;
                            Invoicetable.AddCell(nesthousing);
                        }


                        doc.Add(header);
                        Invoicetable.PaddingTop = 10f;

                        doc.Add(Invoicetable);

                        doc.Add(new Paragraph("GoodDay " + item.CustomerName + "\n" + "\n" +
                            "Thank you for returning your items for order" + "\n" +
                            "Items Returned at" + DateTime.Now.ToString("dd/MM/yyyy HH:mm") + "\n" +
                            "\n" +
                            //"Please note that they need to be returned by the " + item.ExpectedReturnDate.ToString("dd/MM?yyyy") +
                            "Thank you for using GoldPride for your item needs and we hope to see you again" + "\n" +
                            "If this was not you please contact us "
                            + "\n" + "\n" + "\n" + "\n" +
                            "Have  a good day " + "\n" +
                            "Gold Pride"));

                        PdfContentByte cb = new PdfContentByte(writer);


                        BaseFont bf = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1250, true);
                        cb = new PdfContentByte(writer);
                        cb = writer.DirectContent;
                        cb.BeginText();
                        cb.SetFontAndSize(bf, 8);
                        cb.SetTextMatrix(pageSize.GetLeft(120), 20);
                        cb.ShowText("Thank you for choosing GoldPride ");
                        cb.EndText();

                        //Move the pointer and draw line to separate footer section from rest of page
                        cb.MoveTo(40, doc.PageSize.GetBottom(50));
                        cb.LineTo(doc.PageSize.Width - 40, doc.PageSize.GetBottom(50));
                        cb.Stroke();

                        writer.CloseStream = false;
                        doc.Close();

                        memoryStream.Position = 0;


                        SmtpClient client = new SmtpClient("smtp.sendgrid.net");
                        client.Port = 25;
                        client.Host = "smtp.sendgrid.net";
                        client.Timeout = 10000;
                        client.DeliveryMethod = SmtpDeliveryMethod.Network;
                        client.EnableSsl = true;
                        client.UseDefaultCredentials = false;

                        var key = Environment.GetEnvironmentVariable("apikey");

                        client.Credentials = new NetworkCredential("apikey", key/*, user, password*/);

                        System.Net.Mail.Attachment attachment;
                        attachment = new System.Net.Mail.Attachment(memoryStream, "ReminderForReturnToday.pdf");
                        MailMessage msz = new MailMessage(Email, Email)
                        {
                            From = new MailAddress(ConfigurationManager.AppSettings["Email"].ToString()),
                            Subject = "Order reminder for  " + item.CustomerName.ToUpper(),

                            IsBodyHtml = true,
                            Body = " Good Day : " + item.CustomerName.ToUpper() + "\n" + ", Please find attached Reminder for your order with GoldPride: " + item.OrderID,
                        };
                        msz.Attachments.Add(attachment);

                        client.Send(msz);
                    }
                    catch
                    {

                    }

                }
                //end


                //Deliver day before

                var info = (from i in db.Maps
                            join
                             x in db.Orders on i.orderID equals x.OrderID
                            where i.Distance > 0 && x.Collected == false && x.Delivered == false
                            && x.ReminderDate == DateTime.Today && x.DriverID != null
                            && x.Seen == true
                            select x);

                foreach (var item in info)
                {
                    string Email = item.Email;
                    string phone = item.CustomerPhone;
                    MemoryStream memoryStream = new MemoryStream();
                    //start of css 
                    try
                    {
                        StringBuilder status = new StringBuilder("");
                        var doc = new iTextSharp.text.Document(PageSize.A4, 10, 10, 10, 10);
                        PdfWriter writer = PdfWriter.GetInstance(doc, memoryStream);
                        var titleFont = FontFactory.GetFont("Arial", 12, Font.BOLD);
                        var titleFontBlue = FontFactory.GetFont("Arial", 14, Font.NORMAL, BaseColor.BLUE);
                        var boldTableFont = FontFactory.GetFont("Arial", 8, Font.BOLD);
                        var bodyFont = FontFactory.GetFont("Arial", 8, Font.NORMAL);
                        var EmailFont = FontFactory.GetFont("Arial", 8, Font.NORMAL, BaseColor.BLUE);
                        BaseColor TabelHeaderBackGroundColor = WebColors.GetRGBColor("#EEEEEE");

                        //end of css

                        //start of header

                        Rectangle pageSize = writer.PageSize;
                        doc.Open();
                        PdfPTable header = new PdfPTable(3);
                        header.HorizontalAlignment = 0;
                        header.WidthPercentage = 100;
                        header.SetWidths(new float[] { 100f, 320f, 100f });
                        header.DefaultCell.Border = Rectangle.NO_BORDER;

                        //Image logo = Image.GetInstance(System.Web.HttpContext.Current.Server.MapPath("~/Content/images/1.jpg"));
                        //string imageURL = System.Web.HttpContext.Current.Server.MapPath("~/Content/Images/GP_BLUE.jpg") ;
                        //iTextSharp.text.Image logo = iTextSharp.text.Image.GetInstance(imageURL);

                        //logo.ScaleToFit(100, 100);
                        {
                            PdfPCell pdfCelllogo = new PdfPCell(/*logo*/);
                            pdfCelllogo.Border = Rectangle.NO_BORDER;
                            pdfCelllogo.BorderColorBottom = new BaseColor(System.Drawing.Color.Black);
                            pdfCelllogo.BorderWidthBottom = 1f;
                            header.AddCell(pdfCelllogo);
                        }
                        {
                            PdfPCell middlecell = new PdfPCell();
                            middlecell.Border = Rectangle.NO_BORDER;
                            middlecell.BorderColorBottom = new BaseColor(System.Drawing.Color.Black);
                            middlecell.BorderWidthBottom = 1f;
                            header.AddCell(middlecell);
                        }
                        {
                            PdfPTable nested = new PdfPTable(1);
                            nested.DefaultCell.Border = Rectangle.NO_BORDER;
                            PdfPCell nextPostCell1 = new PdfPCell(new Phrase("Gold Pride", titleFont));
                            nextPostCell1.Border = Rectangle.NO_BORDER;
                            nested.AddCell(nextPostCell1);
                            PdfPCell nextPostCell2 = new PdfPCell(new Phrase("90 street, Durban, SA,", bodyFont));
                            nextPostCell2.Border = Rectangle.NO_BORDER;
                            nested.AddCell(nextPostCell2);
                            PdfPCell nextPostCell3 = new PdfPCell(new Phrase("(082) 0798501", bodyFont));
                            nextPostCell3.Border = Rectangle.NO_BORDER;
                            nested.AddCell(nextPostCell3);
                            PdfPCell nextPostCell4 = new PdfPCell(new Phrase("dalphene@gmail.com", EmailFont));
                            nextPostCell4.Border = Rectangle.NO_BORDER;
                            nested.AddCell(nextPostCell4);
                            nested.AddCell("");
                            PdfPCell nesthousing = new PdfPCell(nested);
                            nesthousing.Border = Rectangle.NO_BORDER;
                            nesthousing.BorderColorBottom = new BaseColor(System.Drawing.Color.Black);
                            nesthousing.BorderWidthBottom = 1f;
                            nesthousing.Rowspan = 5;
                            nesthousing.PaddingBottom = 10f;
                            header.AddCell(nesthousing);
                        }
                        PdfPTable Invoicetable = new PdfPTable(3);
                        Invoicetable.HorizontalAlignment = 0;
                        Invoicetable.WidthPercentage = 100;
                        Invoicetable.SetWidths(new float[] { 100f, 320f, 100f });  // then set the column's __relative__ widths
                        Invoicetable.DefaultCell.Border = Rectangle.NO_BORDER;

                        {

                            PdfPTable nested = new PdfPTable(1);
                            nested.DefaultCell.Border = Rectangle.NO_BORDER;
                            PdfPCell nextPostCell1 = new PdfPCell(new Phrase("Delivery Scheduled for: " + item.CustomerName, bodyFont));
                            nextPostCell1.Border = Rectangle.NO_BORDER;
                            nested.AddCell(nextPostCell1);
                            PdfPCell nextPostCell2 = new PdfPCell(new Phrase("Customer Phone " + item.CustomerPhone, bodyFont));
                            nextPostCell2.Border = Rectangle.NO_BORDER;
                            nested.AddCell(nextPostCell2);
                            PdfPCell nextPostCell3 = new PdfPCell(new Phrase("", bodyFont));
                            nextPostCell3.Border = Rectangle.NO_BORDER;
                            nested.AddCell(nextPostCell3);
                            PdfPCell nextPostCell4 = new PdfPCell(new Phrase(" " + item.Email, EmailFont));
                            nextPostCell4.Border = Rectangle.NO_BORDER;
                            nested.AddCell(nextPostCell4);
                            nested.AddCell("");
                            PdfPCell nesthousing = new PdfPCell(nested);
                            nesthousing.Border = Rectangle.NO_BORDER;
                            nesthousing.BorderColorBottom = new BaseColor(System.Drawing.Color.Black);
                            nesthousing.BorderWidthBottom = 1f;
                            nesthousing.Rowspan = 5;
                            nesthousing.PaddingBottom = 10f;
                            Invoicetable.AddCell(nesthousing);
                        }
                        {
                            PdfPCell middlecell = new PdfPCell();
                            middlecell.Border = Rectangle.NO_BORDER;
                            middlecell.BorderColorBottom = new BaseColor(System.Drawing.Color.Black);
                            middlecell.BorderWidthBottom = 1f;
                            Invoicetable.AddCell(middlecell);
                        }

                        {
                            PdfPTable nested = new PdfPTable(1);
                            nested.DefaultCell.Border = Rectangle.NO_BORDER;
                            PdfPCell nextPostCell1 = new PdfPCell(new Phrase("Order ID: " + item.OrderID, bodyFont));
                            nextPostCell1.Border = Rectangle.NO_BORDER;
                            nested.AddCell(nextPostCell1);

                            PdfPCell nextPostCell2 = new PdfPCell(new Phrase("Date of payment: " + item.OrderDate.ToString("dd/MM/yyyy HH:mm:ss"), bodyFont));
                            nextPostCell2.Border = Rectangle.NO_BORDER;
                            nested.AddCell(nextPostCell2);

                            PdfPCell nextPostCell3 = new PdfPCell(new Phrase("Payment Type:  " + "PayFast", bodyFont));
                            nextPostCell3.Border = Rectangle.NO_BORDER;
                            nested.AddCell(nextPostCell3);

                            PdfPCell nextPostCell4 = new PdfPCell(new Phrase(" ", bodyFont));
                            nextPostCell4.Border = Rectangle.NO_BORDER;
                            nested.AddCell(nextPostCell4);


                            PdfPCell nextPostCell5 = new PdfPCell(new Phrase("", bodyFont));
                            nextPostCell5.Border = Rectangle.NO_BORDER;
                            nested.AddCell(nextPostCell5);


                            PdfPCell nextPostCell6 = new PdfPCell(new Phrase("", bodyFont));
                            nextPostCell6.Border = Rectangle.NO_BORDER;
                            nested.AddCell(nextPostCell6);

                            PdfPCell nextPostCell7 = new PdfPCell(new Phrase("", bodyFont));
                            nextPostCell7.Border = Rectangle.NO_BORDER;
                            nested.AddCell(nextPostCell7);

                            //PdfPCell nextPostCell6 = new PdfPCell(new Phrase("Collectors Address" + order.CustomerName, bodyFont));
                            //nextPostCell5.Border = Rectangle.NO_BORDER;
                            //nested.AddCell(nextPostCell6);

                            PdfPCell nextPostCell8 = new PdfPCell(new Phrase(" " /*+ "\n"*/, bodyFont));
                            nextPostCell8.Border = Rectangle.NO_BORDER;
                            nested.AddCell(nextPostCell8);

                            PdfPCell nextPostCell9 = new PdfPCell(new Phrase("", bodyFont));
                            nextPostCell9.Border = Rectangle.NO_BORDER;
                            nested.AddCell(nextPostCell9);



                            nested.AddCell("");
                            PdfPCell nesthousing = new PdfPCell(nested);
                            nesthousing.Border = Rectangle.NO_BORDER;
                            nesthousing.BorderColorBottom = new BaseColor(System.Drawing.Color.Black);
                            nesthousing.BorderWidthBottom = 1f;
                            nesthousing.Rowspan = 5;
                            nesthousing.PaddingBottom = 10f;
                            Invoicetable.AddCell(nesthousing);
                        }


                        doc.Add(header);
                        Invoicetable.PaddingTop = 10f;

                        doc.Add(Invoicetable);

                        doc.Add(new Paragraph("GoodDay " + item.CustomerName + "\n" + "\n" +
                            "This is a reminder that you have opted for delivery" + "\n" +
                            "Delivery of Items will be tomorrow" + DateTime.Now.AddDays(1).ToString("dd/MM/yyyy HH:mm") + "\n" +
                            "\n" +
                            //"Please note that they need to be returned by the " + item.ExpectedReturnDate.ToString("dd/MM?yyyy") +
                            "Thank you for using GoldPride for your item needs and we hope to see you again" + "\n" +
                            "If this was not you please contact us "
                            + "\n" + "\n" + "\n" + "\n" +
                            "Have  a good day " + "\n" +
                            "Gold Pride"));

                        PdfContentByte cb = new PdfContentByte(writer);


                        BaseFont bf = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1250, true);
                        cb = new PdfContentByte(writer);
                        cb = writer.DirectContent;
                        cb.BeginText();
                        cb.SetFontAndSize(bf, 8);
                        cb.SetTextMatrix(pageSize.GetLeft(120), 20);
                        cb.ShowText("Thank you for choosing GoldPride ");
                        cb.EndText();

                        //Move the pointer and draw line to separate footer section from rest of page
                        cb.MoveTo(40, doc.PageSize.GetBottom(50));
                        cb.LineTo(doc.PageSize.Width - 40, doc.PageSize.GetBottom(50));
                        cb.Stroke();

                        writer.CloseStream = false;
                        doc.Close();

                        memoryStream.Position = 0;


                        SmtpClient client = new SmtpClient("smtp.sendgrid.net");
                        client.Port = 25;
                        client.Host = "smtp.sendgrid.net";
                        client.Timeout = 10000;
                        client.DeliveryMethod = SmtpDeliveryMethod.Network;
                        client.EnableSsl = true;
                        client.UseDefaultCredentials = false;

                        var key = Environment.GetEnvironmentVariable("apikey");

                        client.Credentials = new NetworkCredential("apikey", key/*, user, password*/);

                        System.Net.Mail.Attachment attachment;
                        attachment = new System.Net.Mail.Attachment(memoryStream, "ReminderForDelivery.pdf");
                        MailMessage msz = new MailMessage(Email, Email)
                        {
                            From = new MailAddress(ConfigurationManager.AppSettings["Email"].ToString()),
                            Subject = "Order reminder for  " + item.CustomerName.ToUpper(),

                            IsBodyHtml = true,
                            Body = " Good Day : " + item.CustomerName.ToUpper() + "\n" + ", Please find attached Reminder for your order with GoldPride: " + item.OrderID,
                        };
                        msz.Attachments.Add(attachment);

                        client.Send(msz);
                    }
                    catch
                    {

                    }
                }
                // del today
                var infos = (from i in db.Maps
                            join
                             x in db.Orders on i.orderID equals x.OrderID
                             where i.Distance > 0 && x.Collected == false && x.Delivered == false
                            && x.CollDate == DateTime.Today && x.DriverID != null
                            && x.Seen == true
                            select x);

                foreach (var item in infos)
                {
                    string Email = item.Email;
                    string phone = item.CustomerPhone;
                    MemoryStream memoryStream = new MemoryStream();
                    //start of css 
                    try
                    {
                        StringBuilder status = new StringBuilder("");
                        var doc = new iTextSharp.text.Document(PageSize.A4, 10, 10, 10, 10);
                        PdfWriter writer = PdfWriter.GetInstance(doc, memoryStream);
                        var titleFont = FontFactory.GetFont("Arial", 12, Font.BOLD);
                        var titleFontBlue = FontFactory.GetFont("Arial", 14, Font.NORMAL, BaseColor.BLUE);
                        var boldTableFont = FontFactory.GetFont("Arial", 8, Font.BOLD);
                        var bodyFont = FontFactory.GetFont("Arial", 8, Font.NORMAL);
                        var EmailFont = FontFactory.GetFont("Arial", 8, Font.NORMAL, BaseColor.BLUE);
                        BaseColor TabelHeaderBackGroundColor = WebColors.GetRGBColor("#EEEEEE");

                        //end of css

                        //start of header

                        Rectangle pageSize = writer.PageSize;
                        doc.Open();
                        PdfPTable header = new PdfPTable(3);
                        header.HorizontalAlignment = 0;
                        header.WidthPercentage = 100;
                        header.SetWidths(new float[] { 100f, 320f, 100f });
                        header.DefaultCell.Border = Rectangle.NO_BORDER;

                        //Image logo = Image.GetInstance(System.Web.HttpContext.Current.Server.MapPath("~/Content/images/1.jpg"));
                        //string imageURL = System.Web.HttpContext.Current.Server.MapPath("~/Content/Images/GP_BLUE.jpg") ;
                        //iTextSharp.text.Image logo = iTextSharp.text.Image.GetInstance(imageURL);

                        //logo.ScaleToFit(100, 100);
                        {
                            PdfPCell pdfCelllogo = new PdfPCell(/*logo*/);
                            pdfCelllogo.Border = Rectangle.NO_BORDER;
                            pdfCelllogo.BorderColorBottom = new BaseColor(System.Drawing.Color.Black);
                            pdfCelllogo.BorderWidthBottom = 1f;
                            header.AddCell(pdfCelllogo);
                        }
                        {
                            PdfPCell middlecell = new PdfPCell();
                            middlecell.Border = Rectangle.NO_BORDER;
                            middlecell.BorderColorBottom = new BaseColor(System.Drawing.Color.Black);
                            middlecell.BorderWidthBottom = 1f;
                            header.AddCell(middlecell);
                        }
                        {
                            PdfPTable nested = new PdfPTable(1);
                            nested.DefaultCell.Border = Rectangle.NO_BORDER;
                            PdfPCell nextPostCell1 = new PdfPCell(new Phrase("Gold Pride", titleFont));
                            nextPostCell1.Border = Rectangle.NO_BORDER;
                            nested.AddCell(nextPostCell1);
                            PdfPCell nextPostCell2 = new PdfPCell(new Phrase("90 street, Durban, SA,", bodyFont));
                            nextPostCell2.Border = Rectangle.NO_BORDER;
                            nested.AddCell(nextPostCell2);
                            PdfPCell nextPostCell3 = new PdfPCell(new Phrase("(082) 0798501", bodyFont));
                            nextPostCell3.Border = Rectangle.NO_BORDER;
                            nested.AddCell(nextPostCell3);
                            PdfPCell nextPostCell4 = new PdfPCell(new Phrase("dalphene@gmail.com", EmailFont));
                            nextPostCell4.Border = Rectangle.NO_BORDER;
                            nested.AddCell(nextPostCell4);
                            nested.AddCell("");
                            PdfPCell nesthousing = new PdfPCell(nested);
                            nesthousing.Border = Rectangle.NO_BORDER;
                            nesthousing.BorderColorBottom = new BaseColor(System.Drawing.Color.Black);
                            nesthousing.BorderWidthBottom = 1f;
                            nesthousing.Rowspan = 5;
                            nesthousing.PaddingBottom = 10f;
                            header.AddCell(nesthousing);
                        }
                        PdfPTable Invoicetable = new PdfPTable(3);
                        Invoicetable.HorizontalAlignment = 0;
                        Invoicetable.WidthPercentage = 100;
                        Invoicetable.SetWidths(new float[] { 100f, 320f, 100f });  // then set the column's __relative__ widths
                        Invoicetable.DefaultCell.Border = Rectangle.NO_BORDER;

                        {

                            PdfPTable nested = new PdfPTable(1);
                            nested.DefaultCell.Border = Rectangle.NO_BORDER;
                            PdfPCell nextPostCell1 = new PdfPCell(new Phrase("Delivery Scheduled for: " + item.CustomerName, bodyFont));
                            nextPostCell1.Border = Rectangle.NO_BORDER;
                            nested.AddCell(nextPostCell1);
                            PdfPCell nextPostCell2 = new PdfPCell(new Phrase("Customer Phone " + item.CustomerPhone, bodyFont));
                            nextPostCell2.Border = Rectangle.NO_BORDER;
                            nested.AddCell(nextPostCell2);
                            PdfPCell nextPostCell3 = new PdfPCell(new Phrase("", bodyFont));
                            nextPostCell3.Border = Rectangle.NO_BORDER;
                            nested.AddCell(nextPostCell3);
                            PdfPCell nextPostCell4 = new PdfPCell(new Phrase(" " + item.Email, EmailFont));
                            nextPostCell4.Border = Rectangle.NO_BORDER;
                            nested.AddCell(nextPostCell4);
                            nested.AddCell("");
                            PdfPCell nesthousing = new PdfPCell(nested);
                            nesthousing.Border = Rectangle.NO_BORDER;
                            nesthousing.BorderColorBottom = new BaseColor(System.Drawing.Color.Black);
                            nesthousing.BorderWidthBottom = 1f;
                            nesthousing.Rowspan = 5;
                            nesthousing.PaddingBottom = 10f;
                            Invoicetable.AddCell(nesthousing);
                        }
                        {
                            PdfPCell middlecell = new PdfPCell();
                            middlecell.Border = Rectangle.NO_BORDER;
                            middlecell.BorderColorBottom = new BaseColor(System.Drawing.Color.Black);
                            middlecell.BorderWidthBottom = 1f;
                            Invoicetable.AddCell(middlecell);
                        }

                        {
                            PdfPTable nested = new PdfPTable(1);
                            nested.DefaultCell.Border = Rectangle.NO_BORDER;
                            PdfPCell nextPostCell1 = new PdfPCell(new Phrase("Order ID: " + item.OrderID, bodyFont));
                            nextPostCell1.Border = Rectangle.NO_BORDER;
                            nested.AddCell(nextPostCell1);

                            PdfPCell nextPostCell2 = new PdfPCell(new Phrase("Date of payment: " + item.OrderDate.ToString("dd/MM/yyyy HH:mm:ss"), bodyFont));
                            nextPostCell2.Border = Rectangle.NO_BORDER;
                            nested.AddCell(nextPostCell2);

                            PdfPCell nextPostCell3 = new PdfPCell(new Phrase("Payment Type:  " + "PayFast", bodyFont));
                            nextPostCell3.Border = Rectangle.NO_BORDER;
                            nested.AddCell(nextPostCell3);

                            PdfPCell nextPostCell4 = new PdfPCell(new Phrase(" ", bodyFont));
                            nextPostCell4.Border = Rectangle.NO_BORDER;
                            nested.AddCell(nextPostCell4);


                            PdfPCell nextPostCell5 = new PdfPCell(new Phrase("", bodyFont));
                            nextPostCell5.Border = Rectangle.NO_BORDER;
                            nested.AddCell(nextPostCell5);


                            PdfPCell nextPostCell6 = new PdfPCell(new Phrase("", bodyFont));
                            nextPostCell6.Border = Rectangle.NO_BORDER;
                            nested.AddCell(nextPostCell6);

                            PdfPCell nextPostCell7 = new PdfPCell(new Phrase("", bodyFont));
                            nextPostCell7.Border = Rectangle.NO_BORDER;
                            nested.AddCell(nextPostCell7);

                            //PdfPCell nextPostCell6 = new PdfPCell(new Phrase("Collectors Address" + order.CustomerName, bodyFont));
                            //nextPostCell5.Border = Rectangle.NO_BORDER;
                            //nested.AddCell(nextPostCell6);

                            PdfPCell nextPostCell8 = new PdfPCell(new Phrase(" " /*+ "\n"*/, bodyFont));
                            nextPostCell8.Border = Rectangle.NO_BORDER;
                            nested.AddCell(nextPostCell8);

                            PdfPCell nextPostCell9 = new PdfPCell(new Phrase("", bodyFont));
                            nextPostCell9.Border = Rectangle.NO_BORDER;
                            nested.AddCell(nextPostCell9);



                            nested.AddCell("");
                            PdfPCell nesthousing = new PdfPCell(nested);
                            nesthousing.Border = Rectangle.NO_BORDER;
                            nesthousing.BorderColorBottom = new BaseColor(System.Drawing.Color.Black);
                            nesthousing.BorderWidthBottom = 1f;
                            nesthousing.Rowspan = 5;
                            nesthousing.PaddingBottom = 10f;
                            Invoicetable.AddCell(nesthousing);
                        }


                        doc.Add(header);
                        Invoicetable.PaddingTop = 10f;

                        doc.Add(Invoicetable);

                        doc.Add(new Paragraph("GoodDay " + item.CustomerName + "\n" + "\n" +
                            "Your Items have been delivered successfully" + "\n" +
                            "Items Delivered at" + DateTime.Now.ToString("dd/MM/yyyy HH:mm") + "\n" +
                            "\n" +
                            "Please note that they will be collected at " + item.ExpectedReturnDate.ToString("dd/MM?yyyy") +
                            "Thank you for using GoldPride for your item needs and we hope to see you again" + "\n" +
                            "If this was not you please contact us "
                            + "\n" + "\n" + "\n" + "\n" +
                            "Have  a good day " + "\n" +
                            "Gold Pride"));

                        PdfContentByte cb = new PdfContentByte(writer);


                        BaseFont bf = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1250, true);
                        cb = new PdfContentByte(writer);
                        cb = writer.DirectContent;
                        cb.BeginText();
                        cb.SetFontAndSize(bf, 8);
                        cb.SetTextMatrix(pageSize.GetLeft(120), 20);
                        cb.ShowText("Thank you for choosing GoldPride ");
                        cb.EndText();

                        //Move the pointer and draw line to separate footer section from rest of page
                        cb.MoveTo(40, doc.PageSize.GetBottom(50));
                        cb.LineTo(doc.PageSize.Width - 40, doc.PageSize.GetBottom(50));
                        cb.Stroke();

                        writer.CloseStream = false;
                        doc.Close();

                        memoryStream.Position = 0;


                        SmtpClient client = new SmtpClient("smtp.sendgrid.net");
                        client.Port = 25;
                        client.Host = "smtp.sendgrid.net";
                        client.Timeout = 10000;
                        client.DeliveryMethod = SmtpDeliveryMethod.Network;
                        client.EnableSsl = true;
                        client.UseDefaultCredentials = false;

                        var key = Environment.GetEnvironmentVariable("apikey");

                        client.Credentials = new NetworkCredential("apikey", key/*, user, password*/);

                        System.Net.Mail.Attachment attachment;
                        attachment = new System.Net.Mail.Attachment(memoryStream, "ReminderForDeliveryToday.pdf");
                        MailMessage msz = new MailMessage(Email, Email)
                        {
                            From = new MailAddress(ConfigurationManager.AppSettings["Email"].ToString()),
                            Subject = "Order reminder for  " + item.CustomerName.ToUpper(),

                            IsBodyHtml = true,
                            Body = " Good Day : " + item.CustomerName.ToUpper() + "\n" + ", Please find attached Reminder for your order with GoldPride: " + item.OrderID,
                        };
                        msz.Attachments.Add(attachment);

                        client.Send(msz);
                    }
                    catch
                    {

                    }
                }


                //End

                //Return day before
                var Return = (from i in db.Maps
                              join
                              x in db.Orders on i.Id equals x.OrderID
                              where i.Distance > 0 && x.Delivered == true && x.Collected == false
                              && x.SelfReturnReminder == DateTime.Today && x.DriverID != null
                              && x.Seen == true
                              select x);

                foreach (var item in Return)
                {
                    string Email = item.Email;
                    string phone = item.CustomerPhone;
                    MemoryStream memoryStream = new MemoryStream();
                    //start of css 
                    try
                    {
                        StringBuilder status = new StringBuilder("");
                        var doc = new iTextSharp.text.Document(PageSize.A4, 10, 10, 10, 10);
                        PdfWriter writer = PdfWriter.GetInstance(doc, memoryStream);
                        var titleFont = FontFactory.GetFont("Arial", 12, Font.BOLD);
                        var titleFontBlue = FontFactory.GetFont("Arial", 14, Font.NORMAL, BaseColor.BLUE);
                        var boldTableFont = FontFactory.GetFont("Arial", 8, Font.BOLD);
                        var bodyFont = FontFactory.GetFont("Arial", 8, Font.NORMAL);
                        var EmailFont = FontFactory.GetFont("Arial", 8, Font.NORMAL, BaseColor.BLUE);
                        BaseColor TabelHeaderBackGroundColor = WebColors.GetRGBColor("#EEEEEE");

                        //end of css

                        //start of header

                        Rectangle pageSize = writer.PageSize;
                        doc.Open();
                        PdfPTable header = new PdfPTable(3);
                        header.HorizontalAlignment = 0;
                        header.WidthPercentage = 100;
                        header.SetWidths(new float[] { 100f, 320f, 100f });
                        header.DefaultCell.Border = Rectangle.NO_BORDER;

                        //Image logo = Image.GetInstance(System.Web.HttpContext.Current.Server.MapPath("~/Content/images/1.jpg"));
                        //string imageURL = System.Web.HttpContext.Current.Server.MapPath("~/Content/Images/GP_BLUE.jpg") ;
                        //iTextSharp.text.Image logo = iTextSharp.text.Image.GetInstance(imageURL);

                        //logo.ScaleToFit(100, 100);
                        {
                            PdfPCell pdfCelllogo = new PdfPCell(/*logo*/);
                            pdfCelllogo.Border = Rectangle.NO_BORDER;
                            pdfCelllogo.BorderColorBottom = new BaseColor(System.Drawing.Color.Black);
                            pdfCelllogo.BorderWidthBottom = 1f;
                            header.AddCell(pdfCelllogo);
                        }
                        {
                            PdfPCell middlecell = new PdfPCell();
                            middlecell.Border = Rectangle.NO_BORDER;
                            middlecell.BorderColorBottom = new BaseColor(System.Drawing.Color.Black);
                            middlecell.BorderWidthBottom = 1f;
                            header.AddCell(middlecell);
                        }
                        {
                            PdfPTable nested = new PdfPTable(1);
                            nested.DefaultCell.Border = Rectangle.NO_BORDER;
                            PdfPCell nextPostCell1 = new PdfPCell(new Phrase("Gold Pride", titleFont));
                            nextPostCell1.Border = Rectangle.NO_BORDER;
                            nested.AddCell(nextPostCell1);
                            PdfPCell nextPostCell2 = new PdfPCell(new Phrase("90 street, Durban, SA,", bodyFont));
                            nextPostCell2.Border = Rectangle.NO_BORDER;
                            nested.AddCell(nextPostCell2);
                            PdfPCell nextPostCell3 = new PdfPCell(new Phrase("(082) 0798501", bodyFont));
                            nextPostCell3.Border = Rectangle.NO_BORDER;
                            nested.AddCell(nextPostCell3);
                            PdfPCell nextPostCell4 = new PdfPCell(new Phrase("dalphene@gmail.com", EmailFont));
                            nextPostCell4.Border = Rectangle.NO_BORDER;
                            nested.AddCell(nextPostCell4);
                            nested.AddCell("");
                            PdfPCell nesthousing = new PdfPCell(nested);
                            nesthousing.Border = Rectangle.NO_BORDER;
                            nesthousing.BorderColorBottom = new BaseColor(System.Drawing.Color.Black);
                            nesthousing.BorderWidthBottom = 1f;
                            nesthousing.Rowspan = 5;
                            nesthousing.PaddingBottom = 10f;
                            header.AddCell(nesthousing);
                        }
                        PdfPTable Invoicetable = new PdfPTable(3);
                        Invoicetable.HorizontalAlignment = 0;
                        Invoicetable.WidthPercentage = 100;
                        Invoicetable.SetWidths(new float[] { 100f, 320f, 100f });  // then set the column's __relative__ widths
                        Invoicetable.DefaultCell.Border = Rectangle.NO_BORDER;

                        {

                            PdfPTable nested = new PdfPTable(1);
                            nested.DefaultCell.Border = Rectangle.NO_BORDER;
                            PdfPCell nextPostCell1 = new PdfPCell(new Phrase("Delivery Scheduled for: " + item.CustomerName, bodyFont));
                            nextPostCell1.Border = Rectangle.NO_BORDER;
                            nested.AddCell(nextPostCell1);
                            PdfPCell nextPostCell2 = new PdfPCell(new Phrase("Customer Phone " + item.CustomerPhone, bodyFont));
                            nextPostCell2.Border = Rectangle.NO_BORDER;
                            nested.AddCell(nextPostCell2);
                            PdfPCell nextPostCell3 = new PdfPCell(new Phrase("", bodyFont));
                            nextPostCell3.Border = Rectangle.NO_BORDER;
                            nested.AddCell(nextPostCell3);
                            PdfPCell nextPostCell4 = new PdfPCell(new Phrase(" " + item.Email, EmailFont));
                            nextPostCell4.Border = Rectangle.NO_BORDER;
                            nested.AddCell(nextPostCell4);
                            nested.AddCell("");
                            PdfPCell nesthousing = new PdfPCell(nested);
                            nesthousing.Border = Rectangle.NO_BORDER;
                            nesthousing.BorderColorBottom = new BaseColor(System.Drawing.Color.Black);
                            nesthousing.BorderWidthBottom = 1f;
                            nesthousing.Rowspan = 5;
                            nesthousing.PaddingBottom = 10f;
                            Invoicetable.AddCell(nesthousing);
                        }
                        {
                            PdfPCell middlecell = new PdfPCell();
                            middlecell.Border = Rectangle.NO_BORDER;
                            middlecell.BorderColorBottom = new BaseColor(System.Drawing.Color.Black);
                            middlecell.BorderWidthBottom = 1f;
                            Invoicetable.AddCell(middlecell);
                        }

                        {
                            PdfPTable nested = new PdfPTable(1);
                            nested.DefaultCell.Border = Rectangle.NO_BORDER;
                            PdfPCell nextPostCell1 = new PdfPCell(new Phrase("Order ID: " + item.OrderID, bodyFont));
                            nextPostCell1.Border = Rectangle.NO_BORDER;
                            nested.AddCell(nextPostCell1);

                            PdfPCell nextPostCell2 = new PdfPCell(new Phrase("Date of payment: " + item.OrderDate.ToString("dd/MM/yyyy HH:mm:ss"), bodyFont));
                            nextPostCell2.Border = Rectangle.NO_BORDER;
                            nested.AddCell(nextPostCell2);

                            PdfPCell nextPostCell3 = new PdfPCell(new Phrase("Payment Type:  " + "PayFast", bodyFont));
                            nextPostCell3.Border = Rectangle.NO_BORDER;
                            nested.AddCell(nextPostCell3);

                            PdfPCell nextPostCell4 = new PdfPCell(new Phrase(" ", bodyFont));
                            nextPostCell4.Border = Rectangle.NO_BORDER;
                            nested.AddCell(nextPostCell4);


                            PdfPCell nextPostCell5 = new PdfPCell(new Phrase("", bodyFont));
                            nextPostCell5.Border = Rectangle.NO_BORDER;
                            nested.AddCell(nextPostCell5);


                            PdfPCell nextPostCell6 = new PdfPCell(new Phrase("", bodyFont));
                            nextPostCell6.Border = Rectangle.NO_BORDER;
                            nested.AddCell(nextPostCell6);

                            PdfPCell nextPostCell7 = new PdfPCell(new Phrase("", bodyFont));
                            nextPostCell7.Border = Rectangle.NO_BORDER;
                            nested.AddCell(nextPostCell7);

                            //PdfPCell nextPostCell6 = new PdfPCell(new Phrase("Collectors Address" + order.CustomerName, bodyFont));
                            //nextPostCell5.Border = Rectangle.NO_BORDER;
                            //nested.AddCell(nextPostCell6);

                            PdfPCell nextPostCell8 = new PdfPCell(new Phrase(" " /*+ "\n"*/, bodyFont));
                            nextPostCell8.Border = Rectangle.NO_BORDER;
                            nested.AddCell(nextPostCell8);

                            PdfPCell nextPostCell9 = new PdfPCell(new Phrase("", bodyFont));
                            nextPostCell9.Border = Rectangle.NO_BORDER;
                            nested.AddCell(nextPostCell9);



                            nested.AddCell("");
                            PdfPCell nesthousing = new PdfPCell(nested);
                            nesthousing.Border = Rectangle.NO_BORDER;
                            nesthousing.BorderColorBottom = new BaseColor(System.Drawing.Color.Black);
                            nesthousing.BorderWidthBottom = 1f;
                            nesthousing.Rowspan = 5;
                            nesthousing.PaddingBottom = 10f;
                            Invoicetable.AddCell(nesthousing);
                        }


                        doc.Add(header);
                        Invoicetable.PaddingTop = 10f;

                        doc.Add(Invoicetable);

                        doc.Add(new Paragraph("GoodDay " + item.CustomerName + "\n" + "\n" +
                            "This is a reminder that your items will be collected tommorrow" + "\n" +
                            "Items Collected date" + DateTime.Now.AddDays(1).ToString("dd/MM/yyyy HH:mm") + "\n" +
                            "\n" +
                            //"Please note that they need to be returned by the " + item.ExpectedReturnDate.ToString("dd/MM?yyyy") +
                            "Thank you for using GoldPride for your item needs and we hope to see you again" + "\n" +
                            "If this was not you please contact us "
                            + "\n" + "\n" + "\n" + "\n" +
                            "Have  a good day " + "\n" +
                            "Gold Pride"));

                        PdfContentByte cb = new PdfContentByte(writer);


                        BaseFont bf = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1250, true);
                        cb = new PdfContentByte(writer);
                        cb = writer.DirectContent;
                        cb.BeginText();
                        cb.SetFontAndSize(bf, 8);
                        cb.SetTextMatrix(pageSize.GetLeft(120), 20);
                        cb.ShowText("Thank you for choosing GoldPride ");
                        cb.EndText();

                        //Move the pointer and draw line to separate footer section from rest of page
                        cb.MoveTo(40, doc.PageSize.GetBottom(50));
                        cb.LineTo(doc.PageSize.Width - 40, doc.PageSize.GetBottom(50));
                        cb.Stroke();

                        writer.CloseStream = false;
                        doc.Close();

                        memoryStream.Position = 0;


                        SmtpClient client = new SmtpClient("smtp.sendgrid.net");
                        client.Port = 25;
                        client.Host = "smtp.sendgrid.net";
                        client.Timeout = 10000;
                        client.DeliveryMethod = SmtpDeliveryMethod.Network;
                        client.EnableSsl = true;
                        client.UseDefaultCredentials = false;

                        var key = Environment.GetEnvironmentVariable("apikey");

                        client.Credentials = new NetworkCredential("apikey", key/*, user, password*/);

                        System.Net.Mail.Attachment attachment;
                        attachment = new System.Net.Mail.Attachment(memoryStream, "ReminderForDriverCollection.pdf");
                        MailMessage msz = new MailMessage(Email, Email)
                        {
                            From = new MailAddress(ConfigurationManager.AppSettings["Email"].ToString()),
                            Subject = "Order reminder for  " + item.CustomerName.ToUpper(),

                            IsBodyHtml = true,
                            Body = " Good Day : " + item.CustomerName.ToUpper() + "\n" + ", Please find attached Reminder for your order with GoldPride: " + item.OrderID,
                        };
                        msz.Attachments.Add(attachment);

                        client.Send(msz);
                    }
                    catch
                    {

                    }
                   


                    }
                //retrn day of
                var Returns = (from i in db.Maps
                               join
                               x in db.Orders on i.Id equals x.OrderID
                               where i.Distance > 0 && x.Delivered == true && x.Collected == false
                               && x.ExpectedReturnDate == DateTime.Today && x.DriverID != null
                               && x.Seen == true
                               select x);

                foreach (var item in Returns)
                {
                    string Email = item.Email;
                    string phone = item.CustomerPhone;
                    MemoryStream memoryStream = new MemoryStream();
                    //start of css 
                    try
                    {
                        StringBuilder status = new StringBuilder("");
                        var doc = new iTextSharp.text.Document(PageSize.A4, 10, 10, 10, 10);
                        PdfWriter writer = PdfWriter.GetInstance(doc, memoryStream);
                        var titleFont = FontFactory.GetFont("Arial", 12, Font.BOLD);
                        var titleFontBlue = FontFactory.GetFont("Arial", 14, Font.NORMAL, BaseColor.BLUE);
                        var boldTableFont = FontFactory.GetFont("Arial", 8, Font.BOLD);
                        var bodyFont = FontFactory.GetFont("Arial", 8, Font.NORMAL);
                        var EmailFont = FontFactory.GetFont("Arial", 8, Font.NORMAL, BaseColor.BLUE);
                        BaseColor TabelHeaderBackGroundColor = WebColors.GetRGBColor("#EEEEEE");

                        //end of css

                        //start of header

                        Rectangle pageSize = writer.PageSize;
                        doc.Open();
                        PdfPTable header = new PdfPTable(3);
                        header.HorizontalAlignment = 0;
                        header.WidthPercentage = 100;
                        header.SetWidths(new float[] { 100f, 320f, 100f });
                        header.DefaultCell.Border = Rectangle.NO_BORDER;

                        //Image logo = Image.GetInstance(System.Web.HttpContext.Current.Server.MapPath("~/Content/images/1.jpg"));
                        //string imageURL = System.Web.HttpContext.Current.Server.MapPath("~/Content/Images/GP_BLUE.jpg") ;
                        //iTextSharp.text.Image logo = iTextSharp.text.Image.GetInstance(imageURL);

                        //logo.ScaleToFit(100, 100);
                        {
                            PdfPCell pdfCelllogo = new PdfPCell(/*logo*/);
                            pdfCelllogo.Border = Rectangle.NO_BORDER;
                            pdfCelllogo.BorderColorBottom = new BaseColor(System.Drawing.Color.Black);
                            pdfCelllogo.BorderWidthBottom = 1f;
                            header.AddCell(pdfCelllogo);
                        }
                        {
                            PdfPCell middlecell = new PdfPCell();
                            middlecell.Border = Rectangle.NO_BORDER;
                            middlecell.BorderColorBottom = new BaseColor(System.Drawing.Color.Black);
                            middlecell.BorderWidthBottom = 1f;
                            header.AddCell(middlecell);
                        }
                        {
                            PdfPTable nested = new PdfPTable(1);
                            nested.DefaultCell.Border = Rectangle.NO_BORDER;
                            PdfPCell nextPostCell1 = new PdfPCell(new Phrase("Gold Pride", titleFont));
                            nextPostCell1.Border = Rectangle.NO_BORDER;
                            nested.AddCell(nextPostCell1);
                            PdfPCell nextPostCell2 = new PdfPCell(new Phrase("90 street, Durban, SA,", bodyFont));
                            nextPostCell2.Border = Rectangle.NO_BORDER;
                            nested.AddCell(nextPostCell2);
                            PdfPCell nextPostCell3 = new PdfPCell(new Phrase("(082) 0798501", bodyFont));
                            nextPostCell3.Border = Rectangle.NO_BORDER;
                            nested.AddCell(nextPostCell3);
                            PdfPCell nextPostCell4 = new PdfPCell(new Phrase("dalphene@gmail.com", EmailFont));
                            nextPostCell4.Border = Rectangle.NO_BORDER;
                            nested.AddCell(nextPostCell4);
                            nested.AddCell("");
                            PdfPCell nesthousing = new PdfPCell(nested);
                            nesthousing.Border = Rectangle.NO_BORDER;
                            nesthousing.BorderColorBottom = new BaseColor(System.Drawing.Color.Black);
                            nesthousing.BorderWidthBottom = 1f;
                            nesthousing.Rowspan = 5;
                            nesthousing.PaddingBottom = 10f;
                            header.AddCell(nesthousing);
                        }
                        PdfPTable Invoicetable = new PdfPTable(3);
                        Invoicetable.HorizontalAlignment = 0;
                        Invoicetable.WidthPercentage = 100;
                        Invoicetable.SetWidths(new float[] { 100f, 320f, 100f });  // then set the column's __relative__ widths
                        Invoicetable.DefaultCell.Border = Rectangle.NO_BORDER;

                        {

                            PdfPTable nested = new PdfPTable(1);
                            nested.DefaultCell.Border = Rectangle.NO_BORDER;
                            PdfPCell nextPostCell1 = new PdfPCell(new Phrase("Delivery Scheduled for: " + item.CustomerName, bodyFont));
                            nextPostCell1.Border = Rectangle.NO_BORDER;
                            nested.AddCell(nextPostCell1);
                            PdfPCell nextPostCell2 = new PdfPCell(new Phrase("Customer Phone " + item.CustomerPhone, bodyFont));
                            nextPostCell2.Border = Rectangle.NO_BORDER;
                            nested.AddCell(nextPostCell2);
                            PdfPCell nextPostCell3 = new PdfPCell(new Phrase("", bodyFont));
                            nextPostCell3.Border = Rectangle.NO_BORDER;
                            nested.AddCell(nextPostCell3);
                            PdfPCell nextPostCell4 = new PdfPCell(new Phrase(" " + item.Email, EmailFont));
                            nextPostCell4.Border = Rectangle.NO_BORDER;
                            nested.AddCell(nextPostCell4);
                            nested.AddCell("");
                            PdfPCell nesthousing = new PdfPCell(nested);
                            nesthousing.Border = Rectangle.NO_BORDER;
                            nesthousing.BorderColorBottom = new BaseColor(System.Drawing.Color.Black);
                            nesthousing.BorderWidthBottom = 1f;
                            nesthousing.Rowspan = 5;
                            nesthousing.PaddingBottom = 10f;
                            Invoicetable.AddCell(nesthousing);
                        }
                        {
                            PdfPCell middlecell = new PdfPCell();
                            middlecell.Border = Rectangle.NO_BORDER;
                            middlecell.BorderColorBottom = new BaseColor(System.Drawing.Color.Black);
                            middlecell.BorderWidthBottom = 1f;
                            Invoicetable.AddCell(middlecell);
                        }

                        {
                            PdfPTable nested = new PdfPTable(1);
                            nested.DefaultCell.Border = Rectangle.NO_BORDER;
                            PdfPCell nextPostCell1 = new PdfPCell(new Phrase("Order ID: " + item.OrderID, bodyFont));
                            nextPostCell1.Border = Rectangle.NO_BORDER;
                            nested.AddCell(nextPostCell1);

                            PdfPCell nextPostCell2 = new PdfPCell(new Phrase("Date of payment: " + item.OrderDate.ToString("dd/MM/yyyy HH:mm:ss"), bodyFont));
                            nextPostCell2.Border = Rectangle.NO_BORDER;
                            nested.AddCell(nextPostCell2);

                            PdfPCell nextPostCell3 = new PdfPCell(new Phrase("Payment Type:  " + "PayFast", bodyFont));
                            nextPostCell3.Border = Rectangle.NO_BORDER;
                            nested.AddCell(nextPostCell3);

                            PdfPCell nextPostCell4 = new PdfPCell(new Phrase(" ", bodyFont));
                            nextPostCell4.Border = Rectangle.NO_BORDER;
                            nested.AddCell(nextPostCell4);


                            PdfPCell nextPostCell5 = new PdfPCell(new Phrase("", bodyFont));
                            nextPostCell5.Border = Rectangle.NO_BORDER;
                            nested.AddCell(nextPostCell5);


                            PdfPCell nextPostCell6 = new PdfPCell(new Phrase("", bodyFont));
                            nextPostCell6.Border = Rectangle.NO_BORDER;
                            nested.AddCell(nextPostCell6);

                            PdfPCell nextPostCell7 = new PdfPCell(new Phrase("", bodyFont));
                            nextPostCell7.Border = Rectangle.NO_BORDER;
                            nested.AddCell(nextPostCell7);

                            //PdfPCell nextPostCell6 = new PdfPCell(new Phrase("Collectors Address" + order.CustomerName, bodyFont));
                            //nextPostCell5.Border = Rectangle.NO_BORDER;
                            //nested.AddCell(nextPostCell6);

                            PdfPCell nextPostCell8 = new PdfPCell(new Phrase(" " /*+ "\n"*/, bodyFont));
                            nextPostCell8.Border = Rectangle.NO_BORDER;
                            nested.AddCell(nextPostCell8);

                            PdfPCell nextPostCell9 = new PdfPCell(new Phrase("", bodyFont));
                            nextPostCell9.Border = Rectangle.NO_BORDER;
                            nested.AddCell(nextPostCell9);



                            nested.AddCell("");
                            PdfPCell nesthousing = new PdfPCell(nested);
                            nesthousing.Border = Rectangle.NO_BORDER;
                            nesthousing.BorderColorBottom = new BaseColor(System.Drawing.Color.Black);
                            nesthousing.BorderWidthBottom = 1f;
                            nesthousing.Rowspan = 5;
                            nesthousing.PaddingBottom = 10f;
                            Invoicetable.AddCell(nesthousing);
                        }


                        doc.Add(header);
                        Invoicetable.PaddingTop = 10f;

                        doc.Add(Invoicetable);

                        doc.Add(new Paragraph("GoodDay " + item.CustomerName + "\n" + "\n" +
                            "Your Items have been successfully collected" + "\n" +
                            "Items Collected at" + DateTime.Now.ToString("dd/MM/yyyy HH:mm") + "\n" +
                            "\n" +
                            //"Please note that they need to be returned by the " + item.ExpectedReturnDate.ToString("dd/MM?yyyy") +
                            "Thank you for using GoldPride for your item needs and we hope to see you again" + "\n" +
                            "If this was not you please contact us "
                            + "\n" + "\n" + "\n" + "\n" +
                            "Have  a good day " + "\n" +
                            "Gold Pride"));

                        PdfContentByte cb = new PdfContentByte(writer);


                        BaseFont bf = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1250, true);
                        cb = new PdfContentByte(writer);
                        cb = writer.DirectContent;
                        cb.BeginText();
                        cb.SetFontAndSize(bf, 8);
                        cb.SetTextMatrix(pageSize.GetLeft(120), 20);
                        cb.ShowText("Thank you for choosing GoldPride ");
                        cb.EndText();

                        //Move the pointer and draw line to separate footer section from rest of page
                        cb.MoveTo(40, doc.PageSize.GetBottom(50));
                        cb.LineTo(doc.PageSize.Width - 40, doc.PageSize.GetBottom(50));
                        cb.Stroke();

                        writer.CloseStream = false;
                        doc.Close();

                        memoryStream.Position = 0;


                        SmtpClient client = new SmtpClient("smtp.sendgrid.net");
                        client.Port = 25;
                        client.Host = "smtp.sendgrid.net";
                        client.Timeout = 10000;
                        client.DeliveryMethod = SmtpDeliveryMethod.Network;
                        client.EnableSsl = true;
                        client.UseDefaultCredentials = false;

                        var key = Environment.GetEnvironmentVariable("apikey");

                        client.Credentials = new NetworkCredential("apikey", key/*, user, password*/);

                        System.Net.Mail.Attachment attachment;
                        attachment = new System.Net.Mail.Attachment(memoryStream, "ReminderForcollectionToday.pdf");
                        MailMessage msz = new MailMessage(Email, Email)
                        {
                            From = new MailAddress(ConfigurationManager.AppSettings["Email"].ToString()),
                            Subject = "Order reminder for  " + item.CustomerName.ToUpper(),

                            IsBodyHtml = true,
                            Body = " Good Day : " + item.CustomerName.ToUpper() + "\n" + ", Please find attached Reminder for your order with GoldPride: " + item.OrderID,
                        };
                        msz.Attachments.Add(attachment);

                        client.Send(msz);
                    }
                    catch
                    {

                    }
                }
                //end


                //start of packages reminder
                //Day before
                var pack = (from i in db.BookEvents
                            where i.ReminderDayBefore == DateTime.Today
                            select i);
                foreach (var item in pack)
                {
                    string Email = item.CEmail;
                    
                    MemoryStream memoryStream = new MemoryStream();
                    //start of css 
                    try
                    {
                        StringBuilder status = new StringBuilder("");
                        var doc = new iTextSharp.text.Document(PageSize.A4, 10, 10, 10, 10);
                        PdfWriter writer = PdfWriter.GetInstance(doc, memoryStream);
                        var titleFont = FontFactory.GetFont("Arial", 12, Font.BOLD);
                        var titleFontBlue = FontFactory.GetFont("Arial", 14, Font.NORMAL, BaseColor.BLUE);
                        var boldTableFont = FontFactory.GetFont("Arial", 8, Font.BOLD);
                        var bodyFont = FontFactory.GetFont("Arial", 8, Font.NORMAL);
                        var EmailFont = FontFactory.GetFont("Arial", 8, Font.NORMAL, BaseColor.BLUE);
                        BaseColor TabelHeaderBackGroundColor = WebColors.GetRGBColor("#EEEEEE");

                        //end of css

                        //start of header

                        Rectangle pageSize = writer.PageSize;
                        doc.Open();
                        PdfPTable header = new PdfPTable(3);
                        header.HorizontalAlignment = 0;
                        header.WidthPercentage = 100;
                        header.SetWidths(new float[] { 100f, 320f, 100f });
                        header.DefaultCell.Border = Rectangle.NO_BORDER;

                        //Image logo = Image.GetInstance(System.Web.HttpContext.Current.Server.MapPath("~/Content/images/1.jpg"));
                        //string imageURL = System.Web.HttpContext.Current.Server.MapPath("~/Content/Images/GP_BLUE.jpg") ;
                        //iTextSharp.text.Image logo = iTextSharp.text.Image.GetInstance(imageURL);

                        //logo.ScaleToFit(100, 100);
                        {
                            PdfPCell pdfCelllogo = new PdfPCell(/*logo*/);
                            pdfCelllogo.Border = Rectangle.NO_BORDER;
                            pdfCelllogo.BorderColorBottom = new BaseColor(System.Drawing.Color.Black);
                            pdfCelllogo.BorderWidthBottom = 1f;
                            header.AddCell(pdfCelllogo);
                        }
                        {
                            PdfPCell middlecell = new PdfPCell();
                            middlecell.Border = Rectangle.NO_BORDER;
                            middlecell.BorderColorBottom = new BaseColor(System.Drawing.Color.Black);
                            middlecell.BorderWidthBottom = 1f;
                            header.AddCell(middlecell);
                        }
                        {
                            PdfPTable nested = new PdfPTable(1);
                            nested.DefaultCell.Border = Rectangle.NO_BORDER;
                            PdfPCell nextPostCell1 = new PdfPCell(new Phrase("Gold Pride", titleFont));
                            nextPostCell1.Border = Rectangle.NO_BORDER;
                            nested.AddCell(nextPostCell1);
                            PdfPCell nextPostCell2 = new PdfPCell(new Phrase("90 street, Durban, SA,", bodyFont));
                            nextPostCell2.Border = Rectangle.NO_BORDER;
                            nested.AddCell(nextPostCell2);
                            PdfPCell nextPostCell3 = new PdfPCell(new Phrase("(082) 0798501", bodyFont));
                            nextPostCell3.Border = Rectangle.NO_BORDER;
                            nested.AddCell(nextPostCell3);
                            PdfPCell nextPostCell4 = new PdfPCell(new Phrase("dalphene@gmail.com", EmailFont));
                            nextPostCell4.Border = Rectangle.NO_BORDER;
                            nested.AddCell(nextPostCell4);
                            nested.AddCell("");
                            PdfPCell nesthousing = new PdfPCell(nested);
                            nesthousing.Border = Rectangle.NO_BORDER;
                            nesthousing.BorderColorBottom = new BaseColor(System.Drawing.Color.Black);
                            nesthousing.BorderWidthBottom = 1f;
                            nesthousing.Rowspan = 5;
                            nesthousing.PaddingBottom = 10f;
                            header.AddCell(nesthousing);
                        }
                        PdfPTable Invoicetable = new PdfPTable(3);
                        Invoicetable.HorizontalAlignment = 0;
                        Invoicetable.WidthPercentage = 100;
                        Invoicetable.SetWidths(new float[] { 100f, 320f, 100f });  // then set the column's __relative__ widths
                        Invoicetable.DefaultCell.Border = Rectangle.NO_BORDER;

                        {

                            PdfPTable nested = new PdfPTable(1);
                            nested.DefaultCell.Border = Rectangle.NO_BORDER;
                            PdfPCell nextPostCell1 = new PdfPCell(new Phrase("Delivery Scheduled for: " + item.FName, bodyFont));
                            nextPostCell1.Border = Rectangle.NO_BORDER;
                            nested.AddCell(nextPostCell1);
                            PdfPCell nextPostCell2 = new PdfPCell(new Phrase("Customer Phone " + item.Cell, bodyFont));
                            nextPostCell2.Border = Rectangle.NO_BORDER;
                            nested.AddCell(nextPostCell2);
                            PdfPCell nextPostCell3 = new PdfPCell(new Phrase("", bodyFont));
                            nextPostCell3.Border = Rectangle.NO_BORDER;
                            nested.AddCell(nextPostCell3);
                            PdfPCell nextPostCell4 = new PdfPCell(new Phrase(" " + item.CEmail, EmailFont));
                            nextPostCell4.Border = Rectangle.NO_BORDER;
                            nested.AddCell(nextPostCell4);
                            nested.AddCell("");
                            PdfPCell nesthousing = new PdfPCell(nested);
                            nesthousing.Border = Rectangle.NO_BORDER;
                            nesthousing.BorderColorBottom = new BaseColor(System.Drawing.Color.Black);
                            nesthousing.BorderWidthBottom = 1f;
                            nesthousing.Rowspan = 5;
                            nesthousing.PaddingBottom = 10f;
                            Invoicetable.AddCell(nesthousing);
                        }
                        {
                            PdfPCell middlecell = new PdfPCell();
                            middlecell.Border = Rectangle.NO_BORDER;
                            middlecell.BorderColorBottom = new BaseColor(System.Drawing.Color.Black);
                            middlecell.BorderWidthBottom = 1f;
                            Invoicetable.AddCell(middlecell);
                        }

                        {
                            PdfPTable nested = new PdfPTable(1);
                            nested.DefaultCell.Border = Rectangle.NO_BORDER;
                            PdfPCell nextPostCell1 = new PdfPCell(new Phrase("Order ID: " + item.ID, bodyFont));
                            nextPostCell1.Border = Rectangle.NO_BORDER;
                            nested.AddCell(nextPostCell1);

                            PdfPCell nextPostCell2 = new PdfPCell(new Phrase("Date of payment: " + item.DateOfOrder, bodyFont));
                            nextPostCell2.Border = Rectangle.NO_BORDER;
                            nested.AddCell(nextPostCell2);

                            PdfPCell nextPostCell3 = new PdfPCell(new Phrase("Payment Type:  " + "PayFast", bodyFont));
                            nextPostCell3.Border = Rectangle.NO_BORDER;
                            nested.AddCell(nextPostCell3);

                            PdfPCell nextPostCell4 = new PdfPCell(new Phrase(" ", bodyFont));
                            nextPostCell4.Border = Rectangle.NO_BORDER;
                            nested.AddCell(nextPostCell4);


                            PdfPCell nextPostCell5 = new PdfPCell(new Phrase("", bodyFont));
                            nextPostCell5.Border = Rectangle.NO_BORDER;
                            nested.AddCell(nextPostCell5);


                            PdfPCell nextPostCell6 = new PdfPCell(new Phrase("", bodyFont));
                            nextPostCell6.Border = Rectangle.NO_BORDER;
                            nested.AddCell(nextPostCell6);

                            PdfPCell nextPostCell7 = new PdfPCell(new Phrase("", bodyFont));
                            nextPostCell7.Border = Rectangle.NO_BORDER;
                            nested.AddCell(nextPostCell7);

                            //PdfPCell nextPostCell6 = new PdfPCell(new Phrase("Collectors Address" + order.CustomerName, bodyFont));
                            //nextPostCell5.Border = Rectangle.NO_BORDER;
                            //nested.AddCell(nextPostCell6);

                            PdfPCell nextPostCell8 = new PdfPCell(new Phrase(" " /*+ "\n"*/, bodyFont));
                            nextPostCell8.Border = Rectangle.NO_BORDER;
                            nested.AddCell(nextPostCell8);

                            PdfPCell nextPostCell9 = new PdfPCell(new Phrase("", bodyFont));
                            nextPostCell9.Border = Rectangle.NO_BORDER;
                            nested.AddCell(nextPostCell9);



                            nested.AddCell("");
                            PdfPCell nesthousing = new PdfPCell(nested);
                            nesthousing.Border = Rectangle.NO_BORDER;
                            nesthousing.BorderColorBottom = new BaseColor(System.Drawing.Color.Black);
                            nesthousing.BorderWidthBottom = 1f;
                            nesthousing.Rowspan = 5;
                            nesthousing.PaddingBottom = 10f;
                            Invoicetable.AddCell(nesthousing);
                        }


                        doc.Add(header);
                        Invoicetable.PaddingTop = 10f;

                        doc.Add(Invoicetable);

                        doc.Add(new Paragraph("GoodDay " + item.FName + "\n" + "\n" +
                            "This is a reminder that you have an event tommorrow" + "\n" +
                            "Event Date" + DateTime.Now.AddDays(1).ToString("dd/MM/yyyy HH:mm") + "\n" +
                            "\n" +
                            //"Please note that they need to be returned by the " +
                            "Thank you for using GoldPride for your item needs and we hope to see you again" + "\n" +
                            "If this was not you please contact us "
                            + "\n" + "\n" + "\n" + "\n" +
                            "Have  a good day " + "\n" +
                            "Gold Pride"));

                        PdfContentByte cb = new PdfContentByte(writer);


                        BaseFont bf = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1250, true);
                        cb = new PdfContentByte(writer);
                        cb = writer.DirectContent;
                        cb.BeginText();
                        cb.SetFontAndSize(bf, 8);
                        cb.SetTextMatrix(pageSize.GetLeft(120), 20);
                        cb.ShowText("Thank you for choosing GoldPride ");
                        cb.EndText();

                        //Move the pointer and draw line to separate footer section from rest of page
                        cb.MoveTo(40, doc.PageSize.GetBottom(50));
                        cb.LineTo(doc.PageSize.Width - 40, doc.PageSize.GetBottom(50));
                        cb.Stroke();

                        writer.CloseStream = false;
                        doc.Close();

                        memoryStream.Position = 0;


                        SmtpClient client = new SmtpClient("smtp.sendgrid.net");
                        client.Port = 25;
                        client.Host = "smtp.sendgrid.net";
                        client.Timeout = 10000;
                        client.DeliveryMethod = SmtpDeliveryMethod.Network;
                        client.EnableSsl = true;
                        client.UseDefaultCredentials = false;

                        var key = Environment.GetEnvironmentVariable("apikey");

                        client.Credentials = new NetworkCredential("apikey", key/*, user, password*/);

                        System.Net.Mail.Attachment attachment;
                        attachment = new System.Net.Mail.Attachment(memoryStream, "PackageDayBefore.pdf");
                        MailMessage msz = new MailMessage(Email, Email)
                        {
                            From = new MailAddress(ConfigurationManager.AppSettings["Email"].ToString()),
                            Subject = "Order reminder for  " + item.FName.ToUpper(),

                            IsBodyHtml = true,
                            Body = " Good Day : " + item.FName.ToUpper() + "\n" + ", Please find attached Reminder for your order with GoldPride: " + item.ID,
                        };
                        msz.Attachments.Add(attachment);

                        client.Send(msz);
                    }
                    catch
                    {

                    }
                }
                //Day of
                var packs = (from i in db.BookEvents
                            where i.Reminder == DateTime.Today
                            select i);
                foreach (var item in packs)
                {
                    string Email = item.CEmail;

                    MemoryStream memoryStream = new MemoryStream();
                    //start of css 
                    try
                    {
                        StringBuilder status = new StringBuilder("");
                        var doc = new iTextSharp.text.Document(PageSize.A4, 10, 10, 10, 10);
                        PdfWriter writer = PdfWriter.GetInstance(doc, memoryStream);
                        var titleFont = FontFactory.GetFont("Arial", 12, Font.BOLD);
                        var titleFontBlue = FontFactory.GetFont("Arial", 14, Font.NORMAL, BaseColor.BLUE);
                        var boldTableFont = FontFactory.GetFont("Arial", 8, Font.BOLD);
                        var bodyFont = FontFactory.GetFont("Arial", 8, Font.NORMAL);
                        var EmailFont = FontFactory.GetFont("Arial", 8, Font.NORMAL, BaseColor.BLUE);
                        BaseColor TabelHeaderBackGroundColor = WebColors.GetRGBColor("#EEEEEE");

                        //end of css

                        //start of header

                        Rectangle pageSize = writer.PageSize;
                        doc.Open();
                        PdfPTable header = new PdfPTable(3);
                        header.HorizontalAlignment = 0;
                        header.WidthPercentage = 100;
                        header.SetWidths(new float[] { 100f, 320f, 100f });
                        header.DefaultCell.Border = Rectangle.NO_BORDER;

                        //Image logo = Image.GetInstance(System.Web.HttpContext.Current.Server.MapPath("~/Content/images/1.jpg"));
                        //string imageURL = System.Web.HttpContext.Current.Server.MapPath("~/Content/Images/GP_BLUE.jpg") ;
                        //iTextSharp.text.Image logo = iTextSharp.text.Image.GetInstance(imageURL);

                        //logo.ScaleToFit(100, 100);
                        {
                            PdfPCell pdfCelllogo = new PdfPCell(/*logo*/);
                            pdfCelllogo.Border = Rectangle.NO_BORDER;
                            pdfCelllogo.BorderColorBottom = new BaseColor(System.Drawing.Color.Black);
                            pdfCelllogo.BorderWidthBottom = 1f;
                            header.AddCell(pdfCelllogo);
                        }
                        {
                            PdfPCell middlecell = new PdfPCell();
                            middlecell.Border = Rectangle.NO_BORDER;
                            middlecell.BorderColorBottom = new BaseColor(System.Drawing.Color.Black);
                            middlecell.BorderWidthBottom = 1f;
                            header.AddCell(middlecell);
                        }
                        {
                            PdfPTable nested = new PdfPTable(1);
                            nested.DefaultCell.Border = Rectangle.NO_BORDER;
                            PdfPCell nextPostCell1 = new PdfPCell(new Phrase("Gold Pride", titleFont));
                            nextPostCell1.Border = Rectangle.NO_BORDER;
                            nested.AddCell(nextPostCell1);
                            PdfPCell nextPostCell2 = new PdfPCell(new Phrase("90 street, Durban, SA,", bodyFont));
                            nextPostCell2.Border = Rectangle.NO_BORDER;
                            nested.AddCell(nextPostCell2);
                            PdfPCell nextPostCell3 = new PdfPCell(new Phrase("(082) 0798501", bodyFont));
                            nextPostCell3.Border = Rectangle.NO_BORDER;
                            nested.AddCell(nextPostCell3);
                            PdfPCell nextPostCell4 = new PdfPCell(new Phrase("dalphene@gmail.com", EmailFont));
                            nextPostCell4.Border = Rectangle.NO_BORDER;
                            nested.AddCell(nextPostCell4);
                            nested.AddCell("");
                            PdfPCell nesthousing = new PdfPCell(nested);
                            nesthousing.Border = Rectangle.NO_BORDER;
                            nesthousing.BorderColorBottom = new BaseColor(System.Drawing.Color.Black);
                            nesthousing.BorderWidthBottom = 1f;
                            nesthousing.Rowspan = 5;
                            nesthousing.PaddingBottom = 10f;
                            header.AddCell(nesthousing);
                        }
                        PdfPTable Invoicetable = new PdfPTable(3);
                        Invoicetable.HorizontalAlignment = 0;
                        Invoicetable.WidthPercentage = 100;
                        Invoicetable.SetWidths(new float[] { 100f, 320f, 100f });  // then set the column's __relative__ widths
                        Invoicetable.DefaultCell.Border = Rectangle.NO_BORDER;

                        {

                            PdfPTable nested = new PdfPTable(1);
                            nested.DefaultCell.Border = Rectangle.NO_BORDER;
                            PdfPCell nextPostCell1 = new PdfPCell(new Phrase("Delivery Scheduled for: " + item.FName, bodyFont));
                            nextPostCell1.Border = Rectangle.NO_BORDER;
                            nested.AddCell(nextPostCell1);
                            PdfPCell nextPostCell2 = new PdfPCell(new Phrase("Customer Phone " + item.Cell, bodyFont));
                            nextPostCell2.Border = Rectangle.NO_BORDER;
                            nested.AddCell(nextPostCell2);
                            PdfPCell nextPostCell3 = new PdfPCell(new Phrase("", bodyFont));
                            nextPostCell3.Border = Rectangle.NO_BORDER;
                            nested.AddCell(nextPostCell3);
                            PdfPCell nextPostCell4 = new PdfPCell(new Phrase(" " + item.CEmail, EmailFont));
                            nextPostCell4.Border = Rectangle.NO_BORDER;
                            nested.AddCell(nextPostCell4);
                            nested.AddCell("");
                            PdfPCell nesthousing = new PdfPCell(nested);
                            nesthousing.Border = Rectangle.NO_BORDER;
                            nesthousing.BorderColorBottom = new BaseColor(System.Drawing.Color.Black);
                            nesthousing.BorderWidthBottom = 1f;
                            nesthousing.Rowspan = 5;
                            nesthousing.PaddingBottom = 10f;
                            Invoicetable.AddCell(nesthousing);
                        }
                        {
                            PdfPCell middlecell = new PdfPCell();
                            middlecell.Border = Rectangle.NO_BORDER;
                            middlecell.BorderColorBottom = new BaseColor(System.Drawing.Color.Black);
                            middlecell.BorderWidthBottom = 1f;
                            Invoicetable.AddCell(middlecell);
                        }

                        {
                            PdfPTable nested = new PdfPTable(1);
                            nested.DefaultCell.Border = Rectangle.NO_BORDER;
                            PdfPCell nextPostCell1 = new PdfPCell(new Phrase("Order ID: " + item.ID, bodyFont));
                            nextPostCell1.Border = Rectangle.NO_BORDER;
                            nested.AddCell(nextPostCell1);

                            PdfPCell nextPostCell2 = new PdfPCell(new Phrase("Date of payment: " + item.DateOfOrder, bodyFont));
                            nextPostCell2.Border = Rectangle.NO_BORDER;
                            nested.AddCell(nextPostCell2);

                            PdfPCell nextPostCell3 = new PdfPCell(new Phrase("Payment Type:  " + "PayFast", bodyFont));
                            nextPostCell3.Border = Rectangle.NO_BORDER;
                            nested.AddCell(nextPostCell3);

                            PdfPCell nextPostCell4 = new PdfPCell(new Phrase(" ", bodyFont));
                            nextPostCell4.Border = Rectangle.NO_BORDER;
                            nested.AddCell(nextPostCell4);


                            PdfPCell nextPostCell5 = new PdfPCell(new Phrase("", bodyFont));
                            nextPostCell5.Border = Rectangle.NO_BORDER;
                            nested.AddCell(nextPostCell5);


                            PdfPCell nextPostCell6 = new PdfPCell(new Phrase("", bodyFont));
                            nextPostCell6.Border = Rectangle.NO_BORDER;
                            nested.AddCell(nextPostCell6);

                            PdfPCell nextPostCell7 = new PdfPCell(new Phrase("", bodyFont));
                            nextPostCell7.Border = Rectangle.NO_BORDER;
                            nested.AddCell(nextPostCell7);

                            //PdfPCell nextPostCell6 = new PdfPCell(new Phrase("Collectors Address" + order.CustomerName, bodyFont));
                            //nextPostCell5.Border = Rectangle.NO_BORDER;
                            //nested.AddCell(nextPostCell6);

                            PdfPCell nextPostCell8 = new PdfPCell(new Phrase(" " /*+ "\n"*/, bodyFont));
                            nextPostCell8.Border = Rectangle.NO_BORDER;
                            nested.AddCell(nextPostCell8);

                            PdfPCell nextPostCell9 = new PdfPCell(new Phrase("", bodyFont));
                            nextPostCell9.Border = Rectangle.NO_BORDER;
                            nested.AddCell(nextPostCell9);



                            nested.AddCell("");
                            PdfPCell nesthousing = new PdfPCell(nested);
                            nesthousing.Border = Rectangle.NO_BORDER;
                            nesthousing.BorderColorBottom = new BaseColor(System.Drawing.Color.Black);
                            nesthousing.BorderWidthBottom = 1f;
                            nesthousing.Rowspan = 5;
                            nesthousing.PaddingBottom = 10f;
                            Invoicetable.AddCell(nesthousing);
                        }


                        doc.Add(header);
                        Invoicetable.PaddingTop = 10f;

                        doc.Add(Invoicetable);

                        doc.Add(new Paragraph("GoodDay " + item.FName + "\n" + "\n" +
                            "Today is the day of your event" + "\n" +
                            "Date " + DateTime.Now.ToString("dd/MM/yyyy HH:mm") + "\n" +
                            "\n" +
                            //"Please note that they need to be returned by the " +
                            "Thank you for using GoldPride for your item needs and we hope to see you again" + "\n" +
                            "If this was not you please contact us "
                            + "\n" + "\n" + "\n" + "\n" +
                            "Have  a good day " + "\n" +
                            "Gold Pride"));

                        PdfContentByte cb = new PdfContentByte(writer);


                        BaseFont bf = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1250, true);
                        cb = new PdfContentByte(writer);
                        cb = writer.DirectContent;
                        cb.BeginText();
                        cb.SetFontAndSize(bf, 8);
                        cb.SetTextMatrix(pageSize.GetLeft(120), 20);
                        cb.ShowText("Thank you for choosing GoldPride ");
                        cb.EndText();

                        //Move the pointer and draw line to separate footer section from rest of page
                        cb.MoveTo(40, doc.PageSize.GetBottom(50));
                        cb.LineTo(doc.PageSize.Width - 40, doc.PageSize.GetBottom(50));
                        cb.Stroke();

                        writer.CloseStream = false;
                        doc.Close();

                        memoryStream.Position = 0;


                        SmtpClient client = new SmtpClient("smtp.sendgrid.net");
                        client.Port = 25;
                        client.Host = "smtp.sendgrid.net";
                        client.Timeout = 10000;
                        client.DeliveryMethod = SmtpDeliveryMethod.Network;
                        client.EnableSsl = true;
                        client.UseDefaultCredentials = false;

                        var key = Environment.GetEnvironmentVariable("apikey");

                        client.Credentials = new NetworkCredential("apikey", key/*, user, password*/);

                        System.Net.Mail.Attachment attachment;
                        attachment = new System.Net.Mail.Attachment(memoryStream, "PackageToday.pdf");
                        MailMessage msz = new MailMessage(Email, Email)
                        {
                            From = new MailAddress(ConfigurationManager.AppSettings["Email"].ToString()),
                            Subject = "Order reminder for  " + item.FName.ToUpper(),

                            IsBodyHtml = true,
                            Body = " Good Day : " + item.FName.ToUpper() + "\n" + ", Please find attached Reminder for your order with GoldPride: " + item.ID,
                        };
                        msz.Attachments.Add(attachment);

                        client.Send(msz);
                    }
                    catch
                    {

                    }
                }
                //meeting day before
                var meet = (from i in db.Events
                             where i.MReminder == DateTime.Today
                             select i);
                foreach (var item in meet)
                {
                    string Email = item.Email;

                    MemoryStream memoryStream = new MemoryStream();
                    //start of css 
                    try
                    {
                        StringBuilder status = new StringBuilder("");
                        var doc = new iTextSharp.text.Document(PageSize.A4, 10, 10, 10, 10);
                        PdfWriter writer = PdfWriter.GetInstance(doc, memoryStream);
                        var titleFont = FontFactory.GetFont("Arial", 12, Font.BOLD);
                        var titleFontBlue = FontFactory.GetFont("Arial", 14, Font.NORMAL, BaseColor.BLUE);
                        var boldTableFont = FontFactory.GetFont("Arial", 8, Font.BOLD);
                        var bodyFont = FontFactory.GetFont("Arial", 8, Font.NORMAL);
                        var EmailFont = FontFactory.GetFont("Arial", 8, Font.NORMAL, BaseColor.BLUE);
                        BaseColor TabelHeaderBackGroundColor = WebColors.GetRGBColor("#EEEEEE");

                        //end of css

                        //start of header

                        Rectangle pageSize = writer.PageSize;
                        doc.Open();
                        PdfPTable header = new PdfPTable(3);
                        header.HorizontalAlignment = 0;
                        header.WidthPercentage = 100;
                        header.SetWidths(new float[] { 100f, 320f, 100f });
                        header.DefaultCell.Border = Rectangle.NO_BORDER;

                        //Image logo = Image.GetInstance(System.Web.HttpContext.Current.Server.MapPath("~/Content/images/1.jpg"));
                        //string imageURL = System.Web.HttpContext.Current.Server.MapPath("~/Content/Images/GP_BLUE.jpg") ;
                        //iTextSharp.text.Image logo = iTextSharp.text.Image.GetInstance(imageURL);

                        //logo.ScaleToFit(100, 100);
                        {
                            PdfPCell pdfCelllogo = new PdfPCell(/*logo*/);
                            pdfCelllogo.Border = Rectangle.NO_BORDER;
                            pdfCelllogo.BorderColorBottom = new BaseColor(System.Drawing.Color.Black);
                            pdfCelllogo.BorderWidthBottom = 1f;
                            header.AddCell(pdfCelllogo);
                        }
                        {
                            PdfPCell middlecell = new PdfPCell();
                            middlecell.Border = Rectangle.NO_BORDER;
                            middlecell.BorderColorBottom = new BaseColor(System.Drawing.Color.Black);
                            middlecell.BorderWidthBottom = 1f;
                            header.AddCell(middlecell);
                        }
                        {
                            PdfPTable nested = new PdfPTable(1);
                            nested.DefaultCell.Border = Rectangle.NO_BORDER;
                            PdfPCell nextPostCell1 = new PdfPCell(new Phrase("Gold Pride", titleFont));
                            nextPostCell1.Border = Rectangle.NO_BORDER;
                            nested.AddCell(nextPostCell1);
                            PdfPCell nextPostCell2 = new PdfPCell(new Phrase("90 street, Durban, SA,", bodyFont));
                            nextPostCell2.Border = Rectangle.NO_BORDER;
                            nested.AddCell(nextPostCell2);
                            PdfPCell nextPostCell3 = new PdfPCell(new Phrase("(082) 0798501", bodyFont));
                            nextPostCell3.Border = Rectangle.NO_BORDER;
                            nested.AddCell(nextPostCell3);
                            PdfPCell nextPostCell4 = new PdfPCell(new Phrase("dalphene@gmail.com", EmailFont));
                            nextPostCell4.Border = Rectangle.NO_BORDER;
                            nested.AddCell(nextPostCell4);
                            nested.AddCell("");
                            PdfPCell nesthousing = new PdfPCell(nested);
                            nesthousing.Border = Rectangle.NO_BORDER;
                            nesthousing.BorderColorBottom = new BaseColor(System.Drawing.Color.Black);
                            nesthousing.BorderWidthBottom = 1f;
                            nesthousing.Rowspan = 5;
                            nesthousing.PaddingBottom = 10f;
                            header.AddCell(nesthousing);
                        }
                        PdfPTable Invoicetable = new PdfPTable(3);
                        Invoicetable.HorizontalAlignment = 0;
                        Invoicetable.WidthPercentage = 100;
                        Invoicetable.SetWidths(new float[] { 100f, 320f, 100f });  // then set the column's __relative__ widths
                        Invoicetable.DefaultCell.Border = Rectangle.NO_BORDER;

                        {

                            PdfPTable nested = new PdfPTable(1);
                            nested.DefaultCell.Border = Rectangle.NO_BORDER;
                            PdfPCell nextPostCell1 = new PdfPCell(new Phrase("Delivery Scheduled for: " + item.Fname, bodyFont));
                            nextPostCell1.Border = Rectangle.NO_BORDER;
                            nested.AddCell(nextPostCell1);
                            PdfPCell nextPostCell2 = new PdfPCell(new Phrase("Customer Phone " + item.contactNum, bodyFont));
                            nextPostCell2.Border = Rectangle.NO_BORDER;
                            nested.AddCell(nextPostCell2);
                            PdfPCell nextPostCell3 = new PdfPCell(new Phrase("", bodyFont));
                            nextPostCell3.Border = Rectangle.NO_BORDER;
                            nested.AddCell(nextPostCell3);
                            PdfPCell nextPostCell4 = new PdfPCell(new Phrase(" " + item.Email, EmailFont));
                            nextPostCell4.Border = Rectangle.NO_BORDER;
                            nested.AddCell(nextPostCell4);
                            nested.AddCell("");
                            PdfPCell nesthousing = new PdfPCell(nested);
                            nesthousing.Border = Rectangle.NO_BORDER;
                            nesthousing.BorderColorBottom = new BaseColor(System.Drawing.Color.Black);
                            nesthousing.BorderWidthBottom = 1f;
                            nesthousing.Rowspan = 5;
                            nesthousing.PaddingBottom = 10f;
                            Invoicetable.AddCell(nesthousing);
                        }
                        {
                            PdfPCell middlecell = new PdfPCell();
                            middlecell.Border = Rectangle.NO_BORDER;
                            middlecell.BorderColorBottom = new BaseColor(System.Drawing.Color.Black);
                            middlecell.BorderWidthBottom = 1f;
                            Invoicetable.AddCell(middlecell);
                        }

                        {
                            PdfPTable nested = new PdfPTable(1);
                            nested.DefaultCell.Border = Rectangle.NO_BORDER;
                            PdfPCell nextPostCell1 = new PdfPCell(new Phrase("Event ID: " + item.EventId, bodyFont));
                            nextPostCell1.Border = Rectangle.NO_BORDER;
                            nested.AddCell(nextPostCell1);

                            PdfPCell nextPostCell2 = new PdfPCell(new Phrase("Date of Meeting: " + item.Reminder, bodyFont));
                            nextPostCell2.Border = Rectangle.NO_BORDER;
                            nested.AddCell(nextPostCell2);

                            PdfPCell nextPostCell3 = new PdfPCell(new Phrase("Meeting Type:  " + item.EventType, bodyFont));
                            nextPostCell3.Border = Rectangle.NO_BORDER;
                            nested.AddCell(nextPostCell3);

                            PdfPCell nextPostCell4 = new PdfPCell(new Phrase(" ", bodyFont));
                            nextPostCell4.Border = Rectangle.NO_BORDER;
                            nested.AddCell(nextPostCell4);


                            PdfPCell nextPostCell5 = new PdfPCell(new Phrase("", bodyFont));
                            nextPostCell5.Border = Rectangle.NO_BORDER;
                            nested.AddCell(nextPostCell5);


                            PdfPCell nextPostCell6 = new PdfPCell(new Phrase("", bodyFont));
                            nextPostCell6.Border = Rectangle.NO_BORDER;
                            nested.AddCell(nextPostCell6);

                            PdfPCell nextPostCell7 = new PdfPCell(new Phrase("", bodyFont));
                            nextPostCell7.Border = Rectangle.NO_BORDER;
                            nested.AddCell(nextPostCell7);

                            //PdfPCell nextPostCell6 = new PdfPCell(new Phrase("Collectors Address" + order.CustomerName, bodyFont));
                            //nextPostCell5.Border = Rectangle.NO_BORDER;
                            //nested.AddCell(nextPostCell6);

                            PdfPCell nextPostCell8 = new PdfPCell(new Phrase(" " /*+ "\n"*/, bodyFont));
                            nextPostCell8.Border = Rectangle.NO_BORDER;
                            nested.AddCell(nextPostCell8);

                            PdfPCell nextPostCell9 = new PdfPCell(new Phrase("", bodyFont));
                            nextPostCell9.Border = Rectangle.NO_BORDER;
                            nested.AddCell(nextPostCell9);



                            nested.AddCell("");
                            PdfPCell nesthousing = new PdfPCell(nested);
                            nesthousing.Border = Rectangle.NO_BORDER;
                            nesthousing.BorderColorBottom = new BaseColor(System.Drawing.Color.Black);
                            nesthousing.BorderWidthBottom = 1f;
                            nesthousing.Rowspan = 5;
                            nesthousing.PaddingBottom = 10f;
                            Invoicetable.AddCell(nesthousing);
                        }


                        doc.Add(header);
                        Invoicetable.PaddingTop = 10f;

                        doc.Add(Invoicetable);

                        doc.Add(new Paragraph("GoodDay " + item.Fname + "\n" + "\n" +
                            "This is a reminder that you have an appointment tomorrow" + "\n" +
                            "Date" + DateTime.Now.AddDays(1).ToString("dd/MM/yyyy HH:mm") + "\n" +
                            "\n" +
                            //"Please note that they need to be returned by the " +
                            "Thank you for using GoldPride for your item needs and we hope to see you again" + "\n" +
                            "If this was not you please contact us "
                            + "\n" + "\n" + "\n" + "\n" +
                            "Have  a good day " + "\n" +
                            "Gold Pride"));

                        PdfContentByte cb = new PdfContentByte(writer);


                        BaseFont bf = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1250, true);
                        cb = new PdfContentByte(writer);
                        cb = writer.DirectContent;
                        cb.BeginText();
                        cb.SetFontAndSize(bf, 8);
                        cb.SetTextMatrix(pageSize.GetLeft(120), 20);
                        cb.ShowText("Thank you for choosing GoldPride ");
                        cb.EndText();

                        //Move the pointer and draw line to separate footer section from rest of page
                        cb.MoveTo(40, doc.PageSize.GetBottom(50));
                        cb.LineTo(doc.PageSize.Width - 40, doc.PageSize.GetBottom(50));
                        cb.Stroke();

                        writer.CloseStream = false;
                        doc.Close();

                        memoryStream.Position = 0;


                        SmtpClient client = new SmtpClient("smtp.sendgrid.net");
                        client.Port = 25;
                        client.Host = "smtp.sendgrid.net";
                        client.Timeout = 10000;
                        client.DeliveryMethod = SmtpDeliveryMethod.Network;
                        client.EnableSsl = true;
                        client.UseDefaultCredentials = false;

                        var key = Environment.GetEnvironmentVariable("apikey");

                        client.Credentials = new NetworkCredential("apikey", key/*, user, password*/);

                        System.Net.Mail.Attachment attachment;
                        attachment = new System.Net.Mail.Attachment(memoryStream, "MeetingDBF.pdf");
                        MailMessage msz = new MailMessage(Email, Email)
                        {
                            From = new MailAddress(ConfigurationManager.AppSettings["Email"].ToString()),
                            Subject = "Order reminder for  " + item.Fname.ToUpper(),

                            IsBodyHtml = true,
                            Body = " Good Day : " + item.Fname.ToUpper() + "\n" + ", Please find attached Reminder for your order with GoldPride: " + item.EventId,
                        };
                        msz.Attachments.Add(attachment);

                        client.Send(msz);
                    }
                    catch
                    {

                    }
                }
                var meeting = (from i in db.Events
                            where i.Reminder == DateTime.Today
                            select i);
                foreach (var item in meeting)
                {
                    string Email = item.Email;

                    MemoryStream memoryStream = new MemoryStream();
                    //start of css 
                    try
                    {
                        StringBuilder status = new StringBuilder("");
                        var doc = new iTextSharp.text.Document(PageSize.A4, 10, 10, 10, 10);
                        PdfWriter writer = PdfWriter.GetInstance(doc, memoryStream);
                        var titleFont = FontFactory.GetFont("Arial", 12, Font.BOLD);
                        var titleFontBlue = FontFactory.GetFont("Arial", 14, Font.NORMAL, BaseColor.BLUE);
                        var boldTableFont = FontFactory.GetFont("Arial", 8, Font.BOLD);
                        var bodyFont = FontFactory.GetFont("Arial", 8, Font.NORMAL);
                        var EmailFont = FontFactory.GetFont("Arial", 8, Font.NORMAL, BaseColor.BLUE);
                        BaseColor TabelHeaderBackGroundColor = WebColors.GetRGBColor("#EEEEEE");

                        //end of css

                        //start of header

                        Rectangle pageSize = writer.PageSize;
                        doc.Open();
                        PdfPTable header = new PdfPTable(3);
                        header.HorizontalAlignment = 0;
                        header.WidthPercentage = 100;
                        header.SetWidths(new float[] { 100f, 320f, 100f });
                        header.DefaultCell.Border = Rectangle.NO_BORDER;

                        //Image logo = Image.GetInstance(System.Web.HttpContext.Current.Server.MapPath("~/Content/images/1.jpg"));
                        //string imageURL = System.Web.HttpContext.Current.Server.MapPath("~/Content/Images/GP_BLUE.jpg") ;
                        //iTextSharp.text.Image logo = iTextSharp.text.Image.GetInstance(imageURL);

                        //logo.ScaleToFit(100, 100);
                        {
                            PdfPCell pdfCelllogo = new PdfPCell(/*logo*/);
                            pdfCelllogo.Border = Rectangle.NO_BORDER;
                            pdfCelllogo.BorderColorBottom = new BaseColor(System.Drawing.Color.Black);
                            pdfCelllogo.BorderWidthBottom = 1f;
                            header.AddCell(pdfCelllogo);
                        }
                        {
                            PdfPCell middlecell = new PdfPCell();
                            middlecell.Border = Rectangle.NO_BORDER;
                            middlecell.BorderColorBottom = new BaseColor(System.Drawing.Color.Black);
                            middlecell.BorderWidthBottom = 1f;
                            header.AddCell(middlecell);
                        }
                        {
                            PdfPTable nested = new PdfPTable(1);
                            nested.DefaultCell.Border = Rectangle.NO_BORDER;
                            PdfPCell nextPostCell1 = new PdfPCell(new Phrase("Gold Pride", titleFont));
                            nextPostCell1.Border = Rectangle.NO_BORDER;
                            nested.AddCell(nextPostCell1);
                            PdfPCell nextPostCell2 = new PdfPCell(new Phrase("90 street, Durban, SA,", bodyFont));
                            nextPostCell2.Border = Rectangle.NO_BORDER;
                            nested.AddCell(nextPostCell2);
                            PdfPCell nextPostCell3 = new PdfPCell(new Phrase("(082) 0798501", bodyFont));
                            nextPostCell3.Border = Rectangle.NO_BORDER;
                            nested.AddCell(nextPostCell3);
                            PdfPCell nextPostCell4 = new PdfPCell(new Phrase("dalphene@gmail.com", EmailFont));
                            nextPostCell4.Border = Rectangle.NO_BORDER;
                            nested.AddCell(nextPostCell4);
                            nested.AddCell("");
                            PdfPCell nesthousing = new PdfPCell(nested);
                            nesthousing.Border = Rectangle.NO_BORDER;
                            nesthousing.BorderColorBottom = new BaseColor(System.Drawing.Color.Black);
                            nesthousing.BorderWidthBottom = 1f;
                            nesthousing.Rowspan = 5;
                            nesthousing.PaddingBottom = 10f;
                            header.AddCell(nesthousing);
                        }
                        PdfPTable Invoicetable = new PdfPTable(3);
                        Invoicetable.HorizontalAlignment = 0;
                        Invoicetable.WidthPercentage = 100;
                        Invoicetable.SetWidths(new float[] { 100f, 320f, 100f });  // then set the column's __relative__ widths
                        Invoicetable.DefaultCell.Border = Rectangle.NO_BORDER;

                        {

                            PdfPTable nested = new PdfPTable(1);
                            nested.DefaultCell.Border = Rectangle.NO_BORDER;
                            PdfPCell nextPostCell1 = new PdfPCell(new Phrase("Delivery Scheduled for: " + item.Fname, bodyFont));
                            nextPostCell1.Border = Rectangle.NO_BORDER;
                            nested.AddCell(nextPostCell1);
                            PdfPCell nextPostCell2 = new PdfPCell(new Phrase("Customer Phone " + item.contactNum, bodyFont));
                            nextPostCell2.Border = Rectangle.NO_BORDER;
                            nested.AddCell(nextPostCell2);
                            PdfPCell nextPostCell3 = new PdfPCell(new Phrase("", bodyFont));
                            nextPostCell3.Border = Rectangle.NO_BORDER;
                            nested.AddCell(nextPostCell3);
                            PdfPCell nextPostCell4 = new PdfPCell(new Phrase(" " + item.Email, EmailFont));
                            nextPostCell4.Border = Rectangle.NO_BORDER;
                            nested.AddCell(nextPostCell4);
                            nested.AddCell("");
                            PdfPCell nesthousing = new PdfPCell(nested);
                            nesthousing.Border = Rectangle.NO_BORDER;
                            nesthousing.BorderColorBottom = new BaseColor(System.Drawing.Color.Black);
                            nesthousing.BorderWidthBottom = 1f;
                            nesthousing.Rowspan = 5;
                            nesthousing.PaddingBottom = 10f;
                            Invoicetable.AddCell(nesthousing);
                        }
                        {
                            PdfPCell middlecell = new PdfPCell();
                            middlecell.Border = Rectangle.NO_BORDER;
                            middlecell.BorderColorBottom = new BaseColor(System.Drawing.Color.Black);
                            middlecell.BorderWidthBottom = 1f;
                            Invoicetable.AddCell(middlecell);
                        }

                        {
                            PdfPTable nested = new PdfPTable(1);
                            nested.DefaultCell.Border = Rectangle.NO_BORDER;
                            PdfPCell nextPostCell1 = new PdfPCell(new Phrase("Event ID: " + item.EventId, bodyFont));
                            nextPostCell1.Border = Rectangle.NO_BORDER;
                            nested.AddCell(nextPostCell1);

                            PdfPCell nextPostCell2 = new PdfPCell(new Phrase("Date of Meeting: " + item.Reminder, bodyFont));
                            nextPostCell2.Border = Rectangle.NO_BORDER;
                            nested.AddCell(nextPostCell2);

                            PdfPCell nextPostCell3 = new PdfPCell(new Phrase("Meeting Type:  " + item.EventType, bodyFont));
                            nextPostCell3.Border = Rectangle.NO_BORDER;
                            nested.AddCell(nextPostCell3);

                            PdfPCell nextPostCell4 = new PdfPCell(new Phrase(" ", bodyFont));
                            nextPostCell4.Border = Rectangle.NO_BORDER;
                            nested.AddCell(nextPostCell4);


                            PdfPCell nextPostCell5 = new PdfPCell(new Phrase("", bodyFont));
                            nextPostCell5.Border = Rectangle.NO_BORDER;
                            nested.AddCell(nextPostCell5);


                            PdfPCell nextPostCell6 = new PdfPCell(new Phrase("", bodyFont));
                            nextPostCell6.Border = Rectangle.NO_BORDER;
                            nested.AddCell(nextPostCell6);

                            PdfPCell nextPostCell7 = new PdfPCell(new Phrase("", bodyFont));
                            nextPostCell7.Border = Rectangle.NO_BORDER;
                            nested.AddCell(nextPostCell7);

                            //PdfPCell nextPostCell6 = new PdfPCell(new Phrase("Collectors Address" + order.CustomerName, bodyFont));
                            //nextPostCell5.Border = Rectangle.NO_BORDER;
                            //nested.AddCell(nextPostCell6);

                            PdfPCell nextPostCell8 = new PdfPCell(new Phrase(" " /*+ "\n"*/, bodyFont));
                            nextPostCell8.Border = Rectangle.NO_BORDER;
                            nested.AddCell(nextPostCell8);

                            PdfPCell nextPostCell9 = new PdfPCell(new Phrase("", bodyFont));
                            nextPostCell9.Border = Rectangle.NO_BORDER;
                            nested.AddCell(nextPostCell9);



                            nested.AddCell("");
                            PdfPCell nesthousing = new PdfPCell(nested);
                            nesthousing.Border = Rectangle.NO_BORDER;
                            nesthousing.BorderColorBottom = new BaseColor(System.Drawing.Color.Black);
                            nesthousing.BorderWidthBottom = 1f;
                            nesthousing.Rowspan = 5;
                            nesthousing.PaddingBottom = 10f;
                            Invoicetable.AddCell(nesthousing);
                        }


                        doc.Add(header);
                        Invoicetable.PaddingTop = 10f;

                        doc.Add(Invoicetable);

                        doc.Add(new Paragraph("GoodDay " + item.Fname + "\n" + "\n" +
                            "This is a reminder that you have an appoinment today" + "\n" +
                            "Date" + DateTime.Now.ToString("dd/MM/yyyy HH:mm") +item.Start.ToShortTimeString()+ "\n" +
                            "\n" +
                            "Please note that they need to be returned by the " +
                            "Thank you for using GoldPride for your item needs and we hope to see you again" + "\n" +
                            "If this was not you please contact us "
                            + "\n" + "\n" + "\n" + "\n" +
                            "Have  a good day " + "\n" +
                            "Gold Pride"));

                        PdfContentByte cb = new PdfContentByte(writer);


                        BaseFont bf = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1250, true);
                        cb = new PdfContentByte(writer);
                        cb = writer.DirectContent;
                        cb.BeginText();
                        cb.SetFontAndSize(bf, 8);
                        cb.SetTextMatrix(pageSize.GetLeft(120), 20);
                        cb.ShowText("Thank you for choosing GoldPride ");
                        cb.EndText();

                        //Move the pointer and draw line to separate footer section from rest of page
                        cb.MoveTo(40, doc.PageSize.GetBottom(50));
                        cb.LineTo(doc.PageSize.Width - 40, doc.PageSize.GetBottom(50));
                        cb.Stroke();

                        writer.CloseStream = false;
                        doc.Close();

                        memoryStream.Position = 0;


                        SmtpClient client = new SmtpClient("smtp.sendgrid.net");
                        client.Port = 25;
                        client.Host = "smtp.sendgrid.net";
                        client.Timeout = 10000;
                        client.DeliveryMethod = SmtpDeliveryMethod.Network;
                        client.EnableSsl = true;
                        client.UseDefaultCredentials = false;

                        var key = Environment.GetEnvironmentVariable("apikey");

                        client.Credentials = new NetworkCredential("apikey", key/*, user, password*/);

                        System.Net.Mail.Attachment attachment;
                        attachment = new System.Net.Mail.Attachment(memoryStream, "Meeting.pdf");
                        MailMessage msz = new MailMessage(Email, Email)
                        {
                            From = new MailAddress(ConfigurationManager.AppSettings["Email"].ToString()),
                            Subject = "Meeting reminder for  " + item.Fname.ToUpper(),

                            IsBodyHtml = true,
                            Body = " Good Day : " + item.Fname.ToUpper() + "\n" + ", Please find attached Reminder for your order with GoldPride: " + item.EventId,
                        };
                        msz.Attachments.Add(attachment);
                        msz.Body = "Please take a moment of your time to review our services" + "https://2020grp26.azurewebsites.net/Reviews/Create";

                        client.Send(msz);
                    }
                    catch
                    {

                    }
                }


            }
            catch
            {

            }

        }
        
    }
}
