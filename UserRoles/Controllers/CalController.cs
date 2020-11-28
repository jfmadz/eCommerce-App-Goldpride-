using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EventBook.Models;
using UserRoles.Models;
using UserRoles;
using UserRoles.Controllers;
using System.Configuration;
using System.Net.Mail;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.IO;
using System.Text;
using iText.Layout;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.html;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Net;

namespace UserRoles.Controllers
{
    [Authorize]
    public class CalController : Controller
    {
        public ApplicationDbContext db = new ApplicationDbContext();

        // GET: Cal
        [HttpGet]
        public ActionResult Client()
        {
            //Slots slots = new Slots();
            //ViewBag.SlotId = new SelectList(db.Slots, "SlotId", "StartTime");
            //var a = (from i in db.Slots where i.StartTime==slots.StartTime select slots.EndTime.Single().ToString());
            //ViewBag.EndTime = a;
            ////ViewBag.SlotId = new SelectList(db.Slots, "SlotId", "EndTime");

            return View();
        }
        public ActionResult Index()
        {
            return View();

        }

        // GET: Details/Create
        public ActionResult Create()
        {
            return View();
        }

        //new code
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Obsolete]
        public ActionResult Client([Bind(Include = "EventId,EventType,Fname,Start,End,contactNum,Email,MReminder,Reminder")] Event eve)
        {
            var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
            ApplicationUser currentUser = UserManager.FindById(User.Identity.GetUserId());
            string ID = currentUser.Id;
            string Email = currentUser.Email;
            string Name = currentUser.Name;
            string SName = currentUser.Surname;

            if (!db.Events.Any(b => b.Start >= eve.Start && b.Start <= eve.End ||
            (b.End <= eve.End && b.End >= eve.Start) ||
            (b.Start <= eve.Start && b.End >= eve.End)) // no booking in a booking
            && eve.Start.Date > DateTime.Today  //no previous dates
            && eve.Start.Date != DateTime.Today // no booking today

            && eve.Start.DayOfWeek != DayOfWeek.Saturday && eve.Start.DayOfWeek != DayOfWeek.Sunday)

            {
                try
                {
                    //test

                    if (ModelState.IsValid)
                    {
                        eve.MReminder = eve.Start.Date.AddDays(-1);
                        eve.Reminder = eve.Start.Date;
                        db.Events.Add(eve);
                        db.SaveChanges();

                        try
                        {

                            MemoryStream memoryStream = new MemoryStream();
                            //start of css 
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

                            iTextSharp.text.Image logo = iTextSharp.text.Image.GetInstance(HttpContext.Server.MapPath("~/Content/Images/GP_BLUE.jpg"));
                            //var logo = iTextSharp.text.Image.GetInstance(Server.MapPath("~/Content/Images/GP_BLUE.jpg"));
                            logo.ScaleToFit(100, 100);
                            {
                                PdfPCell pdfCelllogo = new PdfPCell(logo);
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
                                PdfPCell nextPostCell1 = new PdfPCell(new Phrase("Meeting booked for:", bodyFont));
                                nextPostCell1.Border = Rectangle.NO_BORDER;
                                nested.AddCell(nextPostCell1);
                                PdfPCell nextPostCell2 = new PdfPCell(new Phrase(" " + Name, bodyFont));
                                nextPostCell2.Border = Rectangle.NO_BORDER;
                                nested.AddCell(nextPostCell2);
                                PdfPCell nextPostCell3 = new PdfPCell(new Phrase("", bodyFont));
                                nextPostCell3.Border = Rectangle.NO_BORDER;
                                nested.AddCell(nextPostCell3);
                                PdfPCell nextPostCell4 = new PdfPCell(new Phrase(" " + Email, EmailFont));
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
                                PdfPCell nextPostCell1 = new PdfPCell(new Phrase("Meeting ID: " + eve.EventId, bodyFont));
                                nextPostCell1.Border = Rectangle.NO_BORDER;
                                nested.AddCell(nextPostCell1);

                                PdfPCell nextPostCell2 = new PdfPCell(new Phrase("Event Type: " +eve.EventType, bodyFont));
                                nextPostCell2.Border = Rectangle.NO_BORDER;
                                nested.AddCell(nextPostCell2);

                                PdfPCell nextPostCell3 = new PdfPCell(new Phrase("Start time of the meeting: " +eve.Start.ToString("dd/MM/yyyy HH:mm:ss"), bodyFont));
                                nextPostCell3.Border = Rectangle.NO_BORDER;
                                nested.AddCell(nextPostCell3);

                                PdfPCell nextPostCell4 = new PdfPCell(new Phrase("End time of the meeting: " + eve.End.ToString("dd/MM/yyyy HH:mm:ss"), bodyFont));
                                nextPostCell4.Border = Rectangle.NO_BORDER;
                                nested.AddCell(nextPostCell4);


                                PdfPCell nextPostCell5 = new PdfPCell(new Phrase("", bodyFont));
                                nextPostCell5.Border = Rectangle.NO_BORDER;
                                nested.AddCell(nextPostCell5);


                                PdfPCell nextPostCell6 = new PdfPCell(new Phrase("" , bodyFont));
                                nextPostCell6.Border = Rectangle.NO_BORDER;
                                nested.AddCell(nextPostCell6);

                                PdfPCell nextPostCell7 = new PdfPCell(new Phrase("" , bodyFont));
                                nextPostCell7.Border = Rectangle.NO_BORDER;
                                nested.AddCell(nextPostCell7);

                                //PdfPCell nextPostCell6 = new PdfPCell(new Phrase("Collectors Address" + order.CustomerName, bodyFont));
                                //nextPostCell5.Border = Rectangle.NO_BORDER;
                                //nested.AddCell(nextPostCell6);

                                PdfPCell nextPostCell8 = new PdfPCell(new Phrase(" " /*+ "\n"*/, bodyFont));
                                nextPostCell8.Border = Rectangle.NO_BORDER;
                                nested.AddCell(nextPostCell8);

                                PdfPCell nextPostCell9 = new PdfPCell(new Phrase("" , bodyFont));
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

                            doc.Add(new Paragraph("GoodDay " +Name +"\n" +"\n" +
                                "We would like to inform you that we have received your meetining booking with us "+ "\n" +
                                "The details of the meeting are as follows: " +"\n" +
                                "Start: " +eve.Start +"\n"+
                                "End: "+ eve.End +"\n"+
                                "Event Type " + eve.EventType+ "\n" + "\n" + "\n"+
                                "Thank you for taking the time to schedule a meeting with us and we look forward to seeing you on the: " + eve.Start +"\n"
                                +"\n"+ "\n" + "\n" + "\n" +
                                "Have  a good day "+ "\n"+
                                "GoldPride"));

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
                            attachment = new System.Net.Mail.Attachment(memoryStream, "meetingConfirmation.pdf");
                            MailMessage msz = new MailMessage(Email, Email)
                            {
                                From = new MailAddress(ConfigurationManager.AppSettings["Email"].ToString()),
                                Subject = "Details of meeting for " + Name.ToUpper(),

                                IsBodyHtml = true,
                                Body = " Good Day : " + Name.ToUpper() + "\n"+ ", Please find attached for your meeting with GoldPride: " ,
                            };
                            msz.Attachments.Add(attachment);

                            client.Send(msz);
                            

                            ModelState.Clear();
                            ViewBag.Message = "Thank you for Contacting us ";
                        }
                        catch (Exception ex)
                        {
                            ModelState.Clear();
                            ViewBag.Message = $" Sorry we are facing Problem here {ex.Message}";
                        }

                        //Admin Email
                        try
                        {
                            ViewBag.Message = "Booking Successful";

                            var apiKey = Environment.GetEnvironmentVariable("SENDGRID_API_KEY");
                            var client = new SendGridClient(apiKey);
                            SmtpClient smtp = new SmtpClient("smtp.sendgrid.net", Convert.ToInt32(587));
                            smtp.Host = "smtp.sendgrid.net.com";
                            smtp.Port = 587;
                            //System.Net.NetworkCredential credentials = new System.Net.NetworkCredential("noreply.codetek@gmail.com", "SendGrid_Key");

                            MailMessage ms = new MailMessage();
                            //Email which you are getting 
                            //from contact us page 
                            //var to = new EmailAddress(details.Email);
                            var From = new EmailAddress(ConfigurationManager.AppSettings["Email"].ToString());
                          
                            var to = new EmailAddress(ConfigurationManager.AppSettings["Email"].ToString());
                            var Subject = "New Gold Pride Appointment";
                            //var Body = "<br /><br />";
                            var plainTextContent = "This is your Appointment Details ";
                            var htmlContent = "Hello Admin" + "<br/><br/>" + " New Appointment Set For : " +

                             eve.Start.ToLongDateString() + " At " + eve.Start.ToShortTimeString() + " And Finishes At :" + eve.End.ToShortTimeString();

                            // var IsBodyHtml = true;
                            var msg = MailHelper.CreateSingleEmail(
                               From,
                               to,
                                Subject,
                                plainTextContent,
                                htmlContent
                                );
                            var response = client.SendEmailAsync(msg);

                            ModelState.Clear();
                            ViewBag.Message = "Thank you for Contacting us ";
                        }
                        catch (Exception ex)
                        {
                            ModelState.Clear();
                            ViewBag.Message = $" Sorry we are facing Problem here {ex.Message}";

                            //}
                            ////Admin Email end

                        }
                        //code end
                        //test end
                    }
                }
                catch (Exception ex)
                {
                    ViewBag.Message = $" Appointment Time Unavailable {ex.Message}";
                }
            }
            else
            {
                ViewBag.Message = "Appointment Time Unavailable";
            }

            //ViewBag.SlotId = new SelectList(db.Slots, "SlotId", "StartTime", eve.SlotId);


            return View(eve);

        }



        [HttpGet]
        public JsonResult GetEvents()
        {
            using (ApplicationDbContext dc = new ApplicationDbContext())
            {
                var events = dc.Events.ToList();
                return new JsonResult { Data = events, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
        }

        [HttpPost]
        public JsonResult SaveEv(Event e)
        {


            var status = false;
            using (ApplicationDbContext dc = new ApplicationDbContext())
            {
                if (e.EventId > 0)
                {
                    var v = dc.Events.Where(a => a.EventId == e.EventId).FirstOrDefault();
                    if (v != null)
                    {
                        v.EventType = e.EventType;

                        v.Start = e.Start;
                        v.End = e.End;
                        v.Fname = e.Fname;
                        v.contactNum = e.contactNum;
                        v.Email = e.Email;

                    }

                }
                else
                {
                    dc.Events.Add(e);

                }

                dc.SaveChanges();

                status = true;

            }


            return new JsonResult { Data = new { status } };
        }

        [HttpPost]
        public JsonResult DeleteEv(int eventID)
        {
            var status = false;
            using (ApplicationDbContext dc = new ApplicationDbContext())
            {
                var v = dc.Events.Where(a => a.EventId == eventID).FirstOrDefault();
                if (v != null)
                {
                    dc.Events.Remove(v);
                    dc.SaveChanges();
                    status = true;
                }
            }
            return new JsonResult { Data = new { status = status } };
        }

    }
}