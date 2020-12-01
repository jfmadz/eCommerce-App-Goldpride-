using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using PayPal.Api;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.Expressions;
using System.Data.SqlClient;
using System.Configuration;
using UserRoles.Models;

using PdfSharp;
using iTextSharp.text.pdf;
using iTextSharp.text;
using iTextSharp.text.html.simpleparser;
using Order = UserRoles.Models.Order;
using Microsoft.Owin.Security.Notifications;
using System.Web.Security;
using Microsoft.Ajax.Utilities;
using System.Data.Entity.ModelConfiguration.Conventions;
using PagedList;
using PageSize = iTextSharp.text.PageSize;
using SendGrid;
using System.Net.Mail;
using iTextSharp.tool.xml;
using QRCoder;
using System.Drawing;
using Font = iTextSharp.text.Font;
using iTextSharp.text.html;
using Rectangle = iTextSharp.text.Rectangle;
using System.Text;
using System.Threading.Tasks;

namespace UserRoles.Controllers
{
    [Authorize]
    public class OrderController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();


        // GET: Order

        // Packages order methods
        public ActionResult CusEvents(int? searchString, int? page)
        {
            var list = from u in db.BookEvents
                       where u.Date > DateTime.Now && u.CEmail == User.Identity.Name
                       orderby
                        u.ID ascending
                       select u;
            if (searchString != null)
            {
                list = (IOrderedQueryable<BookEvent>)list.Where(s => s.ID == searchString);
            }
            return View(list.ToList().ToPagedList(page ?? 1, 15));
        }
        public ActionResult UpComingEvents(int? searchString, int?page)
        {
            var list = from u in db.BookEvents
                       where u.Date > DateTime.Now
                       orderby
                        u.ID ascending
                       select u;
            if (searchString != null)
            {
                list = (IOrderedQueryable<BookEvent>)list.Where(s => s.ID == searchString);
            }
            return View(list.ToList().ToPagedList(page ?? 1, 15));
        }
        public ActionResult EventDetails(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var order = db.BookEvents.Find(id);
            if (order == null)
            {
                return HttpNotFound();
            }


            return View(order);
        }
        public ActionResult EventEdit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BookEvent bookEvent = db.BookEvents.Find(id);
            if (bookEvent == null)
            {
                return HttpNotFound();
            }
            ViewBag.LocationID = new SelectList(db.Locations, "LocationID", "hallname", bookEvent.locationID);
            return View(bookEvent);
        }

        // POST: Decors/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Obsolete]
        public ActionResult EventEdit([Bind(Include = "ID,FName,CEmail,LName,Cell,Date,Quantity,locationID,Setup")] BookEvent bookEvent)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (bookEvent.Setup == true)
                    {
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
                                PdfPCell nextPostCell1 = new PdfPCell(new Phrase("Delivery Scheduled for: " + bookEvent.FName, bodyFont));
                                nextPostCell1.Border = Rectangle.NO_BORDER;
                                nested.AddCell(nextPostCell1);
                                PdfPCell nextPostCell2 = new PdfPCell(new Phrase("Customer Phone " + bookEvent.Cell, bodyFont));
                                nextPostCell2.Border = Rectangle.NO_BORDER;
                                nested.AddCell(nextPostCell2);
                                PdfPCell nextPostCell3 = new PdfPCell(new Phrase("", bodyFont));
                                nextPostCell3.Border = Rectangle.NO_BORDER;
                                nested.AddCell(nextPostCell3);
                                PdfPCell nextPostCell4 = new PdfPCell(new Phrase(" " + bookEvent.CEmail, EmailFont));
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
                                PdfPCell nextPostCell1 = new PdfPCell(new Phrase("Order ID: " + bookEvent.ID, bodyFont));
                                nextPostCell1.Border = Rectangle.NO_BORDER;
                                nested.AddCell(nextPostCell1);

                                PdfPCell nextPostCell2 = new PdfPCell(new Phrase("Date of Event: " + bookEvent.Date.ToString("dd/MM/yyyy HH:mm:ss"), bodyFont));
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

                            doc.Add(new Paragraph("GoodDay " + bookEvent.FName + "\n" + "\n" +
                                "We would like to inform you that we have completed the setup for you event happening on the: " + bookEvent.Date + "\n" +
                                
                                "\n" +
                                "Thank you for using GoldPride for your item needs and we hope to see you again" + "\n" +
                                "If there are any issues please do contact us"
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
                            attachment = new System.Net.Mail.Attachment(memoryStream, "Setup.pdf");
                            MailMessage msz = new MailMessage(bookEvent.CEmail, bookEvent.CEmail)
                            {
                                From = new MailAddress(ConfigurationManager.AppSettings["Email"].ToString()),
                                Subject = "Event Setup for:  " + bookEvent.FName.ToUpper(),

                                IsBodyHtml = true,
                                Body = " Good Day : " + bookEvent.FName.ToUpper() + "\n" + ", Please find attached for your booking with GoldPride: " + bookEvent.ID,
                            };
                            msz.Attachments.Add(attachment);

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
                db.Entry(bookEvent).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("UpComingEvents");
            }
            ViewBag.LocationID = new SelectList(db.Locations, "LocationID", "hallName", bookEvent.locationID);
            return View(bookEvent);
        }

        [HttpGet]

        public ActionResult ActionQrCode()

        {

            return View();

        }

        //Items Hire Order Methods

        //public ActionResult Collectnew(int? searchString, int? page)
        //{
        //    var a = (from i in db.Maps
        //             join
        //             x in db.Orders on i.orderID equals x.OrderID
        //             where i.Distance == 0 && x.PickUp == false && x.Seen == false && x.DriverID == null
        //             orderby
        //            x.OrderID ascending
        //             select x);

        //    if (searchString != null)
        //    {
        //        a = (IOrderedQueryable<Order>)a.Where(s => s.OrderID == searchString);
        //    }

        //    return View(a.ToList().ToPagedList(page ?? 1, 15));


        //}
        //public ActionResult DeliveryNew(int? searchString, int? page)
        //{
        //    var a = (from i in db.Maps
        //             join
        //             x in db.Orders on i.orderID equals x.OrderID
        //             where i.Distance > 0 && x.PickUp == false && x.Seen == false && x.DriverID == null
        //             orderby
        //            x.OrderID ascending
        //             select x);

        //    if (searchString != null)
        //    {
        //        a = (IOrderedQueryable<Order>)a.Where(s => s.OrderID == searchString);
        //    }

        //    return View(a.ToList().ToPagedList(page ?? 1, 15));


        //}

        public ActionResult CusIndex(int? searchString, int? page)
        {
            var list = from u in db.Orders
                       where u.Email == User.Identity.Name orderby
                       u.OrderID descending
                       select u;
            if (searchString != null)
            {
                list = (IOrderedQueryable<Order>)list.Where(s => s.OrderID == searchString);
            }
            return View(list.ToList().ToPagedList(page ?? 1, 15));
        }

        public ActionResult CusOngoing(int? searchString, int? page)
        {
            var list = from u in db.Orders
                       where u.Email == User.Identity.Name &&
                      
                      ( u.OutForCol==false && u.Collected==false) &&
                      (  u.Returned==false)
                       orderby 
                       u.OrderID descending
                       select u;
            if (searchString != null)
            {
                list = (IOrderedQueryable<Order>)list.Where(s => s.OrderID == searchString);
            }
            return View(list.ToList().ToPagedList(page ?? 1, 15));
        }

        public ActionResult Seen(int? searchString)
        {
            var a = (from i in db.Orders
                     where i.Seen == true 
                     && i.PickUp==false && i.Delivered==false orderby
                     i.OrderID descending
                     select i);
            if (searchString != null)
            {
                a = (IOrderedQueryable<Order>)a.Where(s => s.OrderID == searchString);
            }

            return View(a.ToList());
        }

        public ActionResult CusToPickUp(int? searchString)
        {
            var a = (from i in db.Maps
                    join
                    x in db.Orders on i.orderID equals x.OrderID
                    where i.Distance == 0 && x.PickUp==false && x.Seen == true && x.DriverID == null
                     orderby
                    x.OrderID ascending
                    select x);

            if (searchString != null)
            {
                a = (IOrderedQueryable<Order>)a.Where(s => s.OrderID == searchString);
            }

            return View(a.ToList());
        }

        public ActionResult CusToReturn(int? searchString)
        {
            var a = (from i in db.Maps
                     join
                     x in db.Orders on i.orderID equals x.OrderID
                     where i.Distance == 0 && x.PickUp == true &&
                     x.Returned==false && x.Seen == true && x.DriverID == null
                     orderby
                     x.OrderID descending
                     select x);

            if (searchString != null)
            {
                a = (IOrderedQueryable<Order>)a.Where(s => s.OrderID == searchString);
            }

            return View(a.ToList());
        }

        public ActionResult DriverToDeliver(int? searchString)
        {
        //    var a = (from x in db.Orders

        //             where  x.Collected==false && x.Delivered==false
        //             /*&& x.CollDate == DateTime.Today*/ && x.DriverID != null
        //             orderby
        //                x.OrderID descending
        //             select x);

        var a = (from i in db.Maps
                     join
                      x in db.Orders on i.orderID equals x.OrderID
                 where i.Distance > 0 && x.Collected == false && x.Delivered == false
                     && x.CollDate == DateTime.Today && x.DriverID != null
                     orderby
                        x.OrderID descending
                     select x);

            if (searchString != null)
            {
                a = (IOrderedQueryable<Order>)a.Where(s => s.OrderID == searchString);
            }

            return View(a.ToList());
        }

        public ActionResult DriverToCollect(int? searchString)
        {
            var a = (from i in db.Maps
                     join
                     x in db.Orders on i.orderID equals x.OrderID
                     where i.Distance > 0 && x.Delivered==true && x.Collected==false
                     && x.ExpectedReturnDate == DateTime.Today && x.DriverID != null
                     orderby
                     x.OrderID ascending
                     select x);

            if (searchString != null)
            {
                a = (IOrderedQueryable<Order>)a.Where(s => s.OrderID == searchString);
            }

            return View(a.ToList());
        }

        [HttpPost]
        [ValidateInput(false)]
        public FileResult Export(string GridHtml)
        {
            using (MemoryStream stream = new System.IO.MemoryStream())
            {
                StringReader sr = new StringReader(GridHtml);
                Document pdfDoc = new Document(PageSize.A4, 10f, 10f, 100f, 0f);
                PdfWriter writer = PdfWriter.GetInstance(pdfDoc, stream);
                pdfDoc.Open();
                XMLWorkerHelper.GetInstance().ParseXHtml(writer, pdfDoc, sr);
                pdfDoc.Close();
                return File(stream.ToArray(), "applicaton/pdf", "Grid.pdf");
            }
        }
        public ActionResult CusDetails(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var order = db.Orders.Find(id);
           
            
            if (order == null)
            {
                return HttpNotFound();
            }

            
            return View(order);
        }

        public ActionResult AdminIndex(int? searchString, int? page)//int? page
        {
            
            var list = (from i in db.Orders
                       orderby i.OrderID descending
                        select i).Take(5000)/*db.Orders.Take(5000)*/;
            //var search = Convert.ToInt32(search);

            if (searchString != null)
            {
                list = list.Where(s => s.OrderID == searchString);
            }

            return View(list.ToList().ToPagedList(page ?? 1, 15));
        }

            public ActionResult Index(int? searchString, int? page)//int? page
             {
           
            var list = (from i in db.Orders
                        where i.Seen==false
                        select i).Take(5000)/*db.Orders.Take(5000)*/;
            

            if (searchString != null)
            {
                list = list.Where(s => s.OrderID == searchString);
            }

           
            return View(list.ToList().ToPagedList(page ?? 1, 15));


           


        }

       
        private string strCart = "Cart";
        public ActionResult ExportingOrderListing()
        {

            

            GridView gridView = new GridView();
            gridView.DataSource = db.Orders.ToList();
            gridView.DataBind();

            Response.ClearContent();
            Response.Buffer = true;

            Response.AddHeader("content-disposition", "attachment; filename = orders.xls");
            Response.ContentType = "application/ms-excel";
            Response.Charset = "";

            using (StringWriter sw = new StringWriter())
            {
                using (HtmlTextWriter htw = new HtmlTextWriter(sw))
                {
                    gridView.RenderControl(htw);

                    Response.Output.Write(sw.ToString());
                    Response.Flush();
                    Response.End();
                }
            }
            return View();


           
        }


       
        // GET: Order/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var order = db.Orders.Find(id);
            if (order == null)
            {
                return HttpNotFound();
            }


            return View(order);
        }

        

        [HttpPost]
        public ActionResult ActionQrCode(QRModel qr)
        {
            //ApplicationDbContext db = new ApplicationDbContext();
            //QRModel qr = new QRModel();

            var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
            ApplicationUser currentUser = UserManager.FindById(User.Identity.GetUserId());


            string ID = currentUser.Id;
            string Email = currentUser.Email;
            string Name = currentUser.Name;
            string SName = currentUser.Surname;

            QRCodeGenerator ObjQr = new QRCodeGenerator();
            //TempData["fullurl"] = HttpContext.Request.Url.AbsoluteUri;
            //qr.Message = TempData["fullurl"].ToString();
            qr.Message = Request.Url.ToString();

            QRCodeData qrCodeData = ObjQr.CreateQrCode(qr.Message, QRCodeGenerator.ECCLevel.Q);

            Bitmap bitMap = new QRCode(qrCodeData).GetGraphic(20);

            using (MemoryStream ms = new MemoryStream())

            {

                bitMap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);

                byte[] byteImage = ms.ToArray();

                ViewBag.Url = "data:image/png;base64," + Convert.ToBase64String(byteImage);
                ms.Position = 0;
                try
                {
                    // start of working email
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
                    attachment = new System.Net.Mail.Attachment(ms, "order.png");
                    MailMessage msz = new MailMessage(Email, Email)
                    {
                        From = new MailAddress(ConfigurationManager.AppSettings["Email"].ToString()),
                        Subject = "Order QR From GOLDPRIDE For " + Name.ToUpper(),

                        IsBodyHtml = true,
                        Body = " Good Day : " + Name.ToUpper() + " " + SName.ToUpper() +

                        ", Please find attached QR Code for ease of collection : "
                    };
                    msz.Attachments.Add(attachment);

                    client.Send(msz);


                    ModelState.Clear();

                    // end
                }
                catch
                {

                }

            }
            return RedirectToAction("CusDetails", "Order");
            //return View("CusDetails");
            //return RedirectToAction("{Order}/{CusDetails}/{11}");

        }

        public ActionResult MapIndex()
        {
            
            return View(db.Orders.ToList());
        }
        // GET: Order/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Order/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include ="OrderID,OrderName,Distance, OrderDate, PaymentType, PickUp, CustomerName, Surname, CustomerPhone, Email, CustomerAddress")]Order order)
        {
            if (ModelState.IsValid)
            {
               

                //string Distance = map.Distance;
                db.Orders.Add(order);
                db.SaveChanges();
                return RedirectToAction("MapIndex");
            }
            return View(order);
        }

        // GET: Order/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order order = db.Orders.Find(id);
            if (order == null)
            {
                return HttpNotFound();
            }

            return View(order);
        }

       

        // POST: Order/Edit/5
        [HttpPost]
        [Obsolete]
        public async Task<ActionResult> Edit(Order order, int? id)
        {
            if (ModelState.IsValid)
            {
                var sig = (from i in db.Orders
                           where i.OrderID == order.OrderID
                           select i.MySignature).Single();
                // db.Entry(order).State = EntityState.Modified;
                order.MySignature = sig;
                //if (sig != null)
                //{
                    try
                    {




                        if (order.PickUp == true && order.OutForDel == false &&
                            order.Delivered == false && order.Returned == false &&
                            order.OutForCol == false && order.Collected == false)
                        {
                            order.MySignature = sig;
                            try
                            {
                                order.MySignature = sig;

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
                                PdfPCell nextPostCell1 = new PdfPCell(new Phrase("Delivery Scheduled for: " + order.CustomerName, bodyFont));
                                nextPostCell1.Border = Rectangle.NO_BORDER;
                                nested.AddCell(nextPostCell1);
                                PdfPCell nextPostCell2 = new PdfPCell(new Phrase("Customer Phone " + order.CustomerPhone, bodyFont));
                                nextPostCell2.Border = Rectangle.NO_BORDER;
                                nested.AddCell(nextPostCell2);
                                PdfPCell nextPostCell3 = new PdfPCell(new Phrase("", bodyFont));
                                nextPostCell3.Border = Rectangle.NO_BORDER;
                                nested.AddCell(nextPostCell3);
                                PdfPCell nextPostCell4 = new PdfPCell(new Phrase(" " + order.Email, EmailFont));
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
                                PdfPCell nextPostCell1 = new PdfPCell(new Phrase("Order ID: " + order.OrderID, bodyFont));
                                nextPostCell1.Border = Rectangle.NO_BORDER;
                                nested.AddCell(nextPostCell1);

                                PdfPCell nextPostCell2 = new PdfPCell(new Phrase("Date of payment: " + order.OrderDate.ToString("dd/MM/yyyy HH:mm:ss"), bodyFont));
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

                            doc.Add(new Paragraph("GoodDay " + order.CustomerName + "\n" + "\n" +
                                "Thank you for collecting you order"+ "\n" +
                                "Items Collected at"+ DateTime.Now.ToString("dd/MM/yyyy HH:mm") + "\n" +
                                "\n" +
                                "Please note that they need to be returned by the "+ order.ExpectedReturnDate.ToString("dd/MM?yyyy")+
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
                            attachment = new System.Net.Mail.Attachment(memoryStream, "OutForDelivery.pdf");
                            MailMessage msz = new MailMessage(order.Email, order.Email)
                            {
                                From = new MailAddress(ConfigurationManager.AppSettings["Email"].ToString()),
                                Subject = "Details of delivery for " + order.CustomerName.ToUpper(),

                                IsBodyHtml = true,
                                Body = " Good Day : " + order.CustomerName.ToUpper() + "\n" + ", Please find attached for your order with GoldPride: " + order.OrderID ,
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

                        try
                        {
                            order.MySignature = sig;

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
                                PdfPCell nextPostCell1 = new PdfPCell(new Phrase("Delivery Scheduled for: " + order.CustomerName, bodyFont));
                                nextPostCell1.Border = Rectangle.NO_BORDER;
                                nested.AddCell(nextPostCell1);
                                PdfPCell nextPostCell2 = new PdfPCell(new Phrase("Customer Phone " + order.CustomerPhone, bodyFont));
                                nextPostCell2.Border = Rectangle.NO_BORDER;
                                nested.AddCell(nextPostCell2);
                                PdfPCell nextPostCell3 = new PdfPCell(new Phrase("", bodyFont));
                                nextPostCell3.Border = Rectangle.NO_BORDER;
                                nested.AddCell(nextPostCell3);
                                PdfPCell nextPostCell4 = new PdfPCell(new Phrase(" " + order.Email, EmailFont));
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
                                PdfPCell nextPostCell1 = new PdfPCell(new Phrase("Order ID: " + order.OrderID, bodyFont));
                                nextPostCell1.Border = Rectangle.NO_BORDER;
                                nested.AddCell(nextPostCell1);

                                PdfPCell nextPostCell2 = new PdfPCell(new Phrase("Date of payment: " + order.OrderDate.ToString("dd/MM/yyyy HH:mm:ss"), bodyFont));
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

                            doc.Add(new Paragraph("GoodDay " + order.CustomerName + "\n" + "\n" +
                                "Thank you for collecting you order" + "\n" +
                                "Items Collected at" + DateTime.Now.ToString("dd/MM/yyyy HH:mm") + "\n" +
                                "\n" +
                                "Please note that they need to be returned by the " + order.ExpectedReturnDate.ToString("dd/MM?yyyy") +
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
                            attachment = new System.Net.Mail.Attachment(memoryStream, "OutForDelivery.pdf");
                            MailMessage msz = new MailMessage(order.Email, order.Email)
                            {
                                From = new MailAddress(ConfigurationManager.AppSettings["Email"].ToString()),
                                Subject = "Details of delivery for " + order.CustomerName.ToUpper(),

                                IsBodyHtml = true,
                                Body = "Hi " + "" + order.CustomerName + "<br/><br/>" + "Your Order Number: " + "<b>" + order.OrderID + "</b>" + " " ,
                                
                            };
                            msz.Attachments.Add(attachment);
                            client.Send(msz);





                            //var apiKey = Environment.GetEnvironmentVariable("SENDGRID_API_KEY");
                            //var client = new SendGridClient(apiKey);
                            //SmtpClient smtp = new SmtpClient("smtp.sendgrid.net", Convert.ToInt32(587));
                            //smtp.Host = "smtp.sendgrid.net.com";
                            //smtp.Port = 587;
                            ////System.Net.NetworkCredential credentials = new System.Net.NetworkCredential("noreply.codetek@gmail.com", "SendGrid_Key");

                            //MailMessage ms = new MailMessage();
                            ////Email which you are getting 
                            ////from contact us page 
                            ////var to = new EmailAddress(details.Email);
                            //var From = new EmailAddress(ConfigurationManager.AppSettings["Email"].ToString());
                            //// var to = "noreply.codetek@gmail.com"; //new EmailAddress(ConfigurationManager.AppSettings["Email"].ToString()); //Where mail will be sent // You will need to make a Gmail for your business
                            ////var From = new EmailAddress(ConfigurationManager.AppSettings["Email"].ToString());
                            //var to = new EmailAddress(order.Email);
                            //var Subject = "Gold Pride Confirmation Of Collection";
                            ////var Body = "<br /><br />";
                            //var plainTextContent = " ";
                            //var htmlContent = "Hi " + "" + order.CustomerName + "<br/><br/>" + "Your Order Number: " + "<b>" + order.OrderID + "</b>" + " " ;



                            //// var IsBodyHtml = true;
                            //var msg = MailHelper.CreateSingleEmail(
                            //   From,
                            //   to,
                            //    Subject,
                            //    plainTextContent,
                            //    htmlContent
                            //    );

                            ////await Task.Run(() => { System.Threading.Thread.Sleep(300000); });
                            ////System.Threading.Thread.Sleep(300000);
                            //var response = client.SendEmailAsync(msg);

                            ModelState.Clear();
                            ViewBag.Message = "Thank you for Contacting us ";
                        }
                        catch (Exception ex)
                        {
                            ModelState.Clear();
                            ViewBag.Message = $" Sorry we are facing Problem here {ex.Message}";
                        }

                    }

                        //Returned
                        if (order.PickUp == true && order.OutForDel == false &&
                               order.Delivered == false && order.Returned == true &&
                               order.OutForCol == false && order.Collected == false)
                        {
                            order.MySignature = sig;
                            order.ActualReturnDate = DateTime.Today.Date;
                            try
                            {
                                order.ActualReturnDate = DateTime.Now.Date;
                                order.MySignature = sig;

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
                                PdfPCell nextPostCell1 = new PdfPCell(new Phrase("Delivery Scheduled for: " + order.CustomerName, bodyFont));
                                nextPostCell1.Border = Rectangle.NO_BORDER;
                                nested.AddCell(nextPostCell1);
                                PdfPCell nextPostCell2 = new PdfPCell(new Phrase("Customer Phone " + order.CustomerPhone, bodyFont));
                                nextPostCell2.Border = Rectangle.NO_BORDER;
                                nested.AddCell(nextPostCell2);
                                PdfPCell nextPostCell3 = new PdfPCell(new Phrase("", bodyFont));
                                nextPostCell3.Border = Rectangle.NO_BORDER;
                                nested.AddCell(nextPostCell3);
                                PdfPCell nextPostCell4 = new PdfPCell(new Phrase(" " + order.Email, EmailFont));
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
                                PdfPCell nextPostCell1 = new PdfPCell(new Phrase("Order ID: " + order.OrderID, bodyFont));
                                nextPostCell1.Border = Rectangle.NO_BORDER;
                                nested.AddCell(nextPostCell1);

                                PdfPCell nextPostCell2 = new PdfPCell(new Phrase("Date of payment: " + order.OrderDate.ToString("dd/MM/yyyy HH:mm:ss"), bodyFont));
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

                            doc.Add(new Paragraph("GoodDay " + order.CustomerName + "\n" + "\n" +
                                "Thank you for Returning you order" + "\n" +
                                "Items Returned at" + DateTime.Now.ToString("dd/MM/yyyy HH:mm") + "\n" +
                                "\n" +
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
                            attachment = new System.Net.Mail.Attachment(memoryStream, "OutForDelivery.pdf");
                            MailMessage msz = new MailMessage(order.Email, order.Email)
                            {
                                From = new MailAddress(ConfigurationManager.AppSettings["Email"].ToString()),
                                Subject = "Details of delivery for " + order.CustomerName.ToUpper(),

                                IsBodyHtml = true,
                                Body = " Good Day : " + order.CustomerName.ToUpper() + "\n" + ", Please find attached for your order with GoldPride: " + order.OrderID,
                                
                            };
                            msz.Attachments.Add(attachment);
                            msz.Body = "Please take a moment of your timet to review our services " + "https://2020grp26.azurewebsites.net/Reviews/Create";

                            client.Send(msz);



                            ModelState.Clear();
                            ViewBag.Message = "Thank you for Contacting us ";
                            }
                            catch (Exception ex)
                            {
                                ModelState.Clear();
                                ViewBag.Message = $" Sorry we are facing Problem here {ex.Message}";
                            }
                        }

                        //Out For Delivery
                        if (order.PickUp == false && order.OutForDel == true &&
                           order.Delivered == false && order.Returned == false &&
                           order.OutForCol == false && order.Collected == false)
                        {
                            order.MySignature = sig;
                            try
                            {
                                order.MySignature = sig;
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
                                    PdfPCell nextPostCell1 = new PdfPCell(new Phrase("Delivery Scheduled for: " + order.CustomerName, bodyFont));
                                    nextPostCell1.Border = Rectangle.NO_BORDER;
                                    nested.AddCell(nextPostCell1);
                                    PdfPCell nextPostCell2 = new PdfPCell(new Phrase("Customer Phone " + order.CustomerPhone, bodyFont));
                                    nextPostCell2.Border = Rectangle.NO_BORDER;
                                    nested.AddCell(nextPostCell2);
                                    PdfPCell nextPostCell3 = new PdfPCell(new Phrase("", bodyFont));
                                    nextPostCell3.Border = Rectangle.NO_BORDER;
                                    nested.AddCell(nextPostCell3);
                                    PdfPCell nextPostCell4 = new PdfPCell(new Phrase(" " + order.Email, EmailFont));
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
                                    PdfPCell nextPostCell1 = new PdfPCell(new Phrase("Order ID: " + order.OrderID, bodyFont));
                                    nextPostCell1.Border = Rectangle.NO_BORDER;
                                    nested.AddCell(nextPostCell1);

                                    PdfPCell nextPostCell2 = new PdfPCell(new Phrase("Date of payment: " + order.OrderDate.ToString("dd/MM/yyyy HH:mm:ss"), bodyFont));
                                    nextPostCell2.Border = Rectangle.NO_BORDER;
                                    nested.AddCell(nextPostCell2);

                                    PdfPCell nextPostCell3 = new PdfPCell(new Phrase("Payment Type:  " + "PayPal", bodyFont));
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

                                doc.Add(new Paragraph("GoodDay " + order.CustomerName + "\n" + "\n" +
                                    "We would like to inform you that we have dispatched your Order for delivery " + "\n" +
                                    "You can expect delivery of Order Number #" + order.OrderID + " before 5pm today " + DateTime.Today.ToString("dd/MM/yyyy") + "\n" +
                                    "\n" +
                                    "Thank you for using GoldPride for your item needs and we hope to see you again" + "\n" +
                                    "If your order is not delivered by today please contact us and we will assist you from there"
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
                                attachment = new System.Net.Mail.Attachment(memoryStream, "OutForDelivery.pdf");
                                MailMessage msz = new MailMessage(order.Email, order.Email)
                                {
                                    From = new MailAddress(ConfigurationManager.AppSettings["Email"].ToString()),
                                    Subject = "Details of delivery for " + order.CustomerName.ToUpper(),

                                    IsBodyHtml = true,
                                    Body = " Good Day : " + order.CustomerName.ToUpper() + "\n" + ", Please find attached for your order with GoldPride: " + order.OrderID,
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
                        }

                        //Delivered
                        if (order.PickUp == false && order.OutForDel == true &&
                           order.Delivered == true && order.Returned == false &&
                           order.OutForCol == false && order.Collected == false)
                        {
                            order.MySignature = sig;
                            try
                            {
                                order.MySignature = sig;
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
                                    PdfPCell nextPostCell1 = new PdfPCell(new Phrase("Delivery Scheduled for: " + order.CustomerName, bodyFont));
                                    nextPostCell1.Border = Rectangle.NO_BORDER;
                                    nested.AddCell(nextPostCell1);
                                    PdfPCell nextPostCell2 = new PdfPCell(new Phrase("Customer Phone " + order.CustomerPhone, bodyFont));
                                    nextPostCell2.Border = Rectangle.NO_BORDER;
                                    nested.AddCell(nextPostCell2);
                                    PdfPCell nextPostCell3 = new PdfPCell(new Phrase("", bodyFont));
                                    nextPostCell3.Border = Rectangle.NO_BORDER;
                                    nested.AddCell(nextPostCell3);
                                    PdfPCell nextPostCell4 = new PdfPCell(new Phrase(" " + order.Email, EmailFont));
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
                                    PdfPCell nextPostCell1 = new PdfPCell(new Phrase("Order ID: " + order.OrderID, bodyFont));
                                    nextPostCell1.Border = Rectangle.NO_BORDER;
                                    nested.AddCell(nextPostCell1);

                                    PdfPCell nextPostCell2 = new PdfPCell(new Phrase("Date of payment: " + order.OrderDate.ToString("dd/MM/yyyy HH:mm:ss"), bodyFont));
                                    nextPostCell2.Border = Rectangle.NO_BORDER;
                                    nested.AddCell(nextPostCell2);

                                    PdfPCell nextPostCell3 = new PdfPCell(new Phrase("Payment Type:  " + "PayPal", bodyFont));
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

                                doc.Add(new Paragraph("GoodDay " + order.CustomerName + "\n" + "\n" +
                                    "We would like to inform you that we have Delivered your Order " +
                                     +order.OrderID + "\n" +
                                    "\n" +
                                    "Thank you for using GoldPride for your item needs and we hope to see you again" + "\n" +
                                    "If your order is not delivered or not what you ordered  please contact us and we will assist you from there" + "\n" +
                                    "We will be collecting the items that you have hired on the " + DateTime.Now.AddDays(3).ToString("dd/MM/yyyy")
                                    + "\n" + "\n" + "\n" + "\n" +
                                    "Have  a good day " + "\n" +
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
                                attachment = new System.Net.Mail.Attachment(memoryStream, "Delivery.pdf");
                                MailMessage msz = new MailMessage(order.Email, order.Email)
                                {
                                    From = new MailAddress(ConfigurationManager.AppSettings["Email"].ToString()),
                                    Subject = "Details of delivery for " + order.CustomerName.ToUpper(),

                                    IsBodyHtml = true,
                                    Body = " Good Day : " + order.CustomerName.ToUpper() + "\n" + ", Please find attached for your order with GoldPride: " + order.OrderID,
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
                        }

                        //Out For Collection
                        if (order.PickUp == false && order.OutForDel == true &&
                           order.Delivered == true && order.Returned == false &&
                           order.OutForCol == true && order.Collected == false)
                        {
                            order.MySignature = sig;
                            try
                            {
                                order.MySignature = order.MySignature;
                                MemoryStream memoryStream = new MemoryStream();
                                StringBuilder status = new StringBuilder("");
                                var doc = new iTextSharp.text.Document(PageSize.A4, 10, 10, 10, 10);
                                PdfWriter writer = PdfWriter.GetInstance(doc, memoryStream);
                                var titleFont = FontFactory.GetFont("Arial", 12, Font.BOLD);
                                var titleFontBlue = FontFactory.GetFont("Arial", 14, Font.NORMAL, BaseColor.BLUE);
                                var boldTableFont = FontFactory.GetFont("Arial", 8, Font.BOLD);
                                var bodyFont = FontFactory.GetFont("Arial", 8, Font.NORMAL);
                                var EmailFont = FontFactory.GetFont("Arial", 8, Font.NORMAL, BaseColor.BLUE);
                                BaseColor TabelHeaderBackGroundColor = WebColors.GetRGBColor("#EEEEEE");

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
                                    PdfPCell nextPostCell1 = new PdfPCell(new Phrase("Delivery Scheduled for: " + order.CustomerName, bodyFont));
                                    nextPostCell1.Border = Rectangle.NO_BORDER;
                                    nested.AddCell(nextPostCell1);
                                    PdfPCell nextPostCell2 = new PdfPCell(new Phrase("Customer Phone " + order.CustomerPhone, bodyFont));
                                    nextPostCell2.Border = Rectangle.NO_BORDER;
                                    nested.AddCell(nextPostCell2);
                                    PdfPCell nextPostCell3 = new PdfPCell(new Phrase("", bodyFont));
                                    nextPostCell3.Border = Rectangle.NO_BORDER;
                                    nested.AddCell(nextPostCell3);
                                    PdfPCell nextPostCell4 = new PdfPCell(new Phrase(" " + order.Email, EmailFont));
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
                                    PdfPCell nextPostCell1 = new PdfPCell(new Phrase("Order ID: " + order.OrderID, bodyFont));
                                    nextPostCell1.Border = Rectangle.NO_BORDER;
                                    nested.AddCell(nextPostCell1);

                                    PdfPCell nextPostCell2 = new PdfPCell(new Phrase("Date of payment: " + order.OrderDate.ToString("dd/MM/yyyy HH:mm:ss"), bodyFont));
                                    nextPostCell2.Border = Rectangle.NO_BORDER;
                                    nested.AddCell(nextPostCell2);

                                    PdfPCell nextPostCell3 = new PdfPCell(new Phrase("Payment Type:  " + "PayPal", bodyFont));
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

                                doc.Add(new Paragraph("GoodDay " + order.CustomerName + "\n" + "\n" +
                                    "We would like to inform you that we will be collecting your Order " +
                                     +order.OrderID + "today." + "\n" +
                                    "\n" +
                                    "Thank you for using GoldPride for your item needs and we hope to see you again" + "\n" +
                                    "Please present the QR code attached to this email to our driver upon arrival"


                                    + "\n" + "\n" + "\n" + "\n" +
                                    "Have  a good day " + "\n" +
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

                                QRCodeGenerator ObjQr = new QRCodeGenerator();
                                QRModel qr = new QRModel();
                                //TempData["fullurl"] = HttpContext.Request.Url.AbsoluteUri;
                                //qr.Message = TempData["fullurl"].ToString();
                                qr.Message = Request.Url.ToString();

                                QRCodeData qrCodeData = ObjQr.CreateQrCode(qr.Message, QRCodeGenerator.ECCLevel.Q);

                                Bitmap bitMap = new QRCode(qrCodeData).GetGraphic(20);

                                using (MemoryStream ms2 = new MemoryStream())

                                {

                                    bitMap.Save(ms2, System.Drawing.Imaging.ImageFormat.Png);

                                    byte[] byteImage = ms2.ToArray();

                                    ViewBag.Url = "data:image/png;base64," + Convert.ToBase64String(byteImage);



                                    ms2.Position = 0;
                                    try
                                    {
                                        // start of working email
                                        SmtpClient client2 = new SmtpClient("smtp.sendgrid.net");
                                        client2.Port = 25;
                                        client2.Host = "smtp.sendgrid.net";
                                        client2.Timeout = 10000;
                                        client2.DeliveryMethod = SmtpDeliveryMethod.Network;
                                        client2.EnableSsl = true;
                                        client2.UseDefaultCredentials = false;
                                        var key = Environment.GetEnvironmentVariable("apikey");
                                        client2.Credentials = new NetworkCredential("apikey", key/*, user, password*/);
                                        System.Net.Mail.Attachment attachment;
                                        System.Net.Mail.Attachment attach;
                                        attachment = new System.Net.Mail.Attachment(ms2, "order.png");
                                        attach = new System.Net.Mail.Attachment(memoryStream, "outforcollection.pdf");
                                        MailMessage msz = new MailMessage(order.Email, order.Email)
                                        {
                                            From = new MailAddress(ConfigurationManager.AppSettings["Email"].ToString()),
                                            Subject = "Order QR From GOLDPRIDE For " + order.CustomerName.ToUpper(),

                                            IsBodyHtml = true,
                                            Body = " Good Day : " + order.CustomerName.ToUpper() + " " + order.Surname.ToUpper() +

                                            ", Please find attached QR Code for ease of collection : "
                                        };
                                        msz.Attachments.Add(attachment);
                                        msz.Attachments.Add(attach);

                                        client2.Send(msz);


                                        ModelState.Clear();

                                        // end
                                    }
                                    catch
                                    {

                                    }

                                }


                               
                            }
                            catch (Exception ex)
                            {
                                ModelState.Clear();
                                ViewBag.Message = $" Sorry we are facing Problem here {ex.Message}";
                            }
                        }

                        //Collected
                        if (order.PickUp == false && order.OutForDel == true &&
                           order.Delivered == true && order.Returned == false &&
                           order.OutForCol == true && order.Collected == true)
                        {
                            order.MySignature = sig;
                            order.ActualReturnDate = DateTime.Today.Date;
                            try
                            {
                                order.MySignature = sig;
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
                                    PdfPCell nextPostCell1 = new PdfPCell(new Phrase("Delivery Scheduled for: " + order.CustomerName, bodyFont));
                                    nextPostCell1.Border = Rectangle.NO_BORDER;
                                    nested.AddCell(nextPostCell1);
                                    PdfPCell nextPostCell2 = new PdfPCell(new Phrase("Customer Phone " + order.CustomerPhone, bodyFont));
                                    nextPostCell2.Border = Rectangle.NO_BORDER;
                                    nested.AddCell(nextPostCell2);
                                    PdfPCell nextPostCell3 = new PdfPCell(new Phrase("", bodyFont));
                                    nextPostCell3.Border = Rectangle.NO_BORDER;
                                    nested.AddCell(nextPostCell3);
                                    PdfPCell nextPostCell4 = new PdfPCell(new Phrase(" " + order.Email, EmailFont));
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
                                    PdfPCell nextPostCell1 = new PdfPCell(new Phrase("Order ID: " + order.OrderID, bodyFont));
                                    nextPostCell1.Border = Rectangle.NO_BORDER;
                                    nested.AddCell(nextPostCell1);

                                    PdfPCell nextPostCell2 = new PdfPCell(new Phrase("Date of payment: " + order.OrderDate.ToString("dd/MM/yyyy HH:mm:ss"), bodyFont));
                                    nextPostCell2.Border = Rectangle.NO_BORDER;
                                    nested.AddCell(nextPostCell2);

                                    PdfPCell nextPostCell3 = new PdfPCell(new Phrase("Payment Type:  " + "PayPal", bodyFont));
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

                                doc.Add(new Paragraph("GoodDay " + order.CustomerName + "\n" + "\n" +
                                    "We would like to inform you that we have successfully collected your Order from you " +
                                     +order.OrderID + "\n" +
                                    "\n" +
                                    "Thank you for using GoldPride for your item needs and we hope to see you again" + "\n" +
                                    "If there are no issues with the items that we have collcted from you in terms of damages or missing items your deposit will be refunded within 3-5 working days" + "\n" +
                                    "We will notify you once we have proccessed your refund of the deposit"

                                    + "\n" + "\n" + "\n" + "\n" +
                                    "Have  a good day " + "\n" +
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
                                attachment = new System.Net.Mail.Attachment(memoryStream, "CollectionFinalization.pdf");
                                MailMessage msz = new MailMessage(order.Email, order.Email)
                                {
                                    From = new MailAddress(ConfigurationManager.AppSettings["Email"].ToString()),
                                    Subject = "Details of delivery for " + order.CustomerName.ToUpper(),

                                    IsBodyHtml = true,
                                    Body = " Good Day : " + order.CustomerName.ToUpper() + "\n" + ", Please find attached for your order with GoldPride: " + order.OrderID,
                                };
                                msz.Attachments.Add(attachment);
                                 msz.Body = "Please take a moment of your time to review our services " + "https://2020grp26.azurewebsites.net/Reviews/Create";

                              client.Send(msz);
    

                                ModelState.Clear();
                                ViewBag.Message = "Thank you for Contacting us ";
                            }
                            catch (Exception ex)
                            {
                                ModelState.Clear();
                                ViewBag.Message = $" Sorry we are facing Problem here {ex.Message}";
                            }
                        }
                    }
                    catch (Exception ex)
                    {

                    }

                //}

                order.MySignature = sig;
                order.DateDiff = (int?)(((order.ActualReturnDate - order.ExpectedReturnDate).TotalDays * 7 - (order.ExpectedReturnDate.DayOfWeek - order.ActualReturnDate.DayOfWeek) * 2) / 7);
                order.ReminderDate = order.CollDate.AddDays(-1);
                db.Entry(order).State = EntityState.Modified;
                db.SaveChanges();
               if(Request.IsAuthenticated && User.IsInRole("Driver"))
                {
                    return RedirectToAction("DDelivery", "DriverVM");
                }
                else if (Request.IsAuthenticated && User.IsInRole("Admin"))
                {
                    return RedirectToAction("Index");
                }
               

            }

            return View(order);
        }
       
        // GET: Order/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Order/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
