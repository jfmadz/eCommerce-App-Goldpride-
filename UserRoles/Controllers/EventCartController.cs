using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using PayPal.Api;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using UserRoles.Models;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.Ajax.Utilities;

using Image = iTextSharp.text.Image;
using iTextSharp.tool.xml.html;
using iTextSharp.text.html;
using System.Data.Odbc;
using System.Security.Claims;

using Microsoft.Owin.Security.Provider;
using System.Net.Mail;
using System.Configuration;
using PayFast;
using PayFast.AspNet;
using System.Threading.Tasks;

namespace UserRoles.Controllers
{
    //[Authorize]
    public class EventCartController : Controller
    {
        ApplicationDbContext db = new ApplicationDbContext();
        private string stCart = "Cart";
        private readonly PayFastSettings payFastSettings;

       public EventCartController()
        {
            this.payFastSettings = new PayFastSettings();
            this.payFastSettings.MerchantId = ConfigurationManager.AppSettings["MerchantId"];
            this.payFastSettings.MerchantKey = ConfigurationManager.AppSettings["MerchantKey"];
            this.payFastSettings.PassPhrase = ConfigurationManager.AppSettings["PassPhrase"];
            this.payFastSettings.ProcessUrl = ConfigurationManager.AppSettings["ProcessUrl"];
            this.payFastSettings.ValidateUrl = ConfigurationManager.AppSettings["ValidateUrl"];
            this.payFastSettings.ReturnUrl = ConfigurationManager.AppSettings["ReturnUrl"];
           
            this.payFastSettings.CancelUrl = ConfigurationManager.AppSettings["CancelUrl"];
            this.payFastSettings.NotifyUrl = ConfigurationManager.AppSettings["NotifyUrl"];
        }
        // GET: EventCart
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult OrderSuccess()
        {
            return View();
        }
        [Authorize]
        public ActionResult Create()
        {
            //ViewBag.Key = new SelectList(db.NumberOfGuests, "Key", "NumberOfPeople");
            ViewBag.locationID = new SelectList(db.Locations, "locationID", "hallName");
            return View();
        }

       [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,FName,CEmail,LName,Cell,Date,Quantity,locationID")] BookEvent bookEvent)
        {
            var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
            ApplicationUser currentUser = UserManager.FindById(User.Identity.GetUserId());
            string ID = currentUser.Id;
            string Email = currentUser.Email;
            string Name = currentUser.Name;
            string SName = currentUser.Surname;
            string phone = currentUser.PhoneNumber;
           

            if (!db.BookEvents.Any(b => b.Date >= bookEvent.Date && b.Date <= bookEvent.Date ||
            (b.Date <= bookEvent.Date && b.Date >= bookEvent.Date) ||
            (b.Date <= bookEvent.Date && b.Date >= bookEvent.Date)) // no booking in a booking
            && bookEvent.Date.Date > DateTime.Today  //no previous dates
            && bookEvent.Date.Date != DateTime.Today) // no booking today
            {
                try
                {
                    if (ModelState.IsValid)
                    {
                        //int quantities = bookEvent.Quantity;
                        //List<EventCart> lstCart = (List<EventCart>)Session[strCart];
                        
                        bookEvent.CEmail = Email;
                        bookEvent.FName = Name;
                        bookEvent.LName = SName;
                        bookEvent.Cell = phone;
                        bookEvent.DateOfOrder = DateTime.Now;
                        bookEvent.ReminderDayBefore = bookEvent.Date.AddDays(-1);
                        bookEvent.Reminder = bookEvent.Date.Date;
                        db.BookEvents.Add(bookEvent);
                        db.SaveChanges();
                        return RedirectToAction("CusIndex", "Packages");
                    }
                }
                catch (Exception ex)
                {
                    ViewBag.Message = $" Date is Unavailable {ex.Message}";
                }
            }
            else
            {
                ViewBag.Message = "Date is Unavailable";
            }


            //ViewBag.Key = new SelectList(db.NumberOfGuests, "Key", "NumberOfPeople", bookEvent.Key);
            ViewBag.locationID = new SelectList(db.Locations, "locationID", "hallName", bookEvent.locationID);
            return View(bookEvent);
        }
        public ActionResult OrderNow(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (Session[stCart] == null)
            {
                var quant = (from i in db.BookEvents
                             orderby i.ID descending
                             select i).First();
                List<EventCart> IsCart = new List<EventCart>
                {
                    new EventCart(db.Packages.Find(id),quant.Quantity),

                };
                Session[stCart] = IsCart;
            }
           
            return RedirectToAction("Index", "EventCart");
        }
       
        public ActionResult Process()
        {
            ApplicationDbContext db = new ApplicationDbContext();
            var order = (from i in db.BookEvents
                         where i.CEmail == User.Identity.Name
                         orderby i.ID descending
                         select i).First();
           
            var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
            ApplicationUser currentUser = UserManager.FindById(User.Identity.GetUserId());
            string ID = currentUser.Id;
            string Email = currentUser.Email;
            string Name = currentUser.Name;
            string SName = currentUser.Surname;
            byte[] Sign = currentUser.MySignature;

            List<EventCart> lstCart = (List<EventCart>)Session[stCart];
            EventOrderDetail eventorderDetail = new EventOrderDetail();

            List<string> productName = new List<string>();
            List<int> productId = new List<int>();
            List<int> productQuantity = new List<int>();
            List<decimal> productPrice = new List<decimal>();
            List<DateTime> date = new List<DateTime>();


            List<EventCart> temp = (List<EventCart>)Session["Cart"];
            var deposit = String.Format("{0:C}", temp.Sum(x => x.Quantity * x.Package.Price / 3));
            var total = String.Format("{0:C}", temp.Sum(x =>x.Quantity * x.Package.Price) + deposit);
            foreach (EventCart cart in lstCart)
            {
                eventorderDetail.ID = order.ID;
                eventorderDetail.packageName = cart.Package.packageName;
                eventorderDetail.PackageId = cart.Package.PackageId;
                eventorderDetail.Quantity = cart.Quantity;
                eventorderDetail.Price = cart.Package.Price;
                eventorderDetail.Date = DateTime.Now;

                eventorderDetail.Deposit = temp.Sum(x =>x.Quantity * x.Package.Price / 3);
                eventorderDetail.Total =  temp.Sum(x =>x.Quantity * x.Package.Price   ) + eventorderDetail.Deposit;


                productName.Add(cart.Package.packageName);
                productQuantity.Add(cart.Quantity);
                productPrice.Add(cart.Package.Price);
                date.Add(cart.Date);
               


                db.EventOrderDetails.Add(eventorderDetail);
                db.SaveChanges();

               
            }
            try
            {
                MemoryStream memoryStream = new MemoryStream();

                StringBuilder status = new StringBuilder("");
                var doc = new Document(PageSize.A4, 10, 10, 10, 10);
                PdfWriter writer = PdfWriter.GetInstance(doc, memoryStream);
                var titleFont = FontFactory.GetFont("Arial", 12, Font.BOLD);
                var titleFontBlue = FontFactory.GetFont("Arial", 14, Font.NORMAL, BaseColor.BLUE);
                var boldTableFont = FontFactory.GetFont("Arial", 8, Font.BOLD);
                var bodyFont = FontFactory.GetFont("Arial", 8, Font.NORMAL);
                var EmailFont = FontFactory.GetFont("Arial", 8, Font.NORMAL, BaseColor.BLUE);
                BaseColor TabelHeaderBackGroundColor = WebColors.GetRGBColor("#EEEEEE");
                // doc.SetWidths(headers);

                Rectangle pageSize = writer.PageSize;
                doc.Open();
                PdfPTable header = new PdfPTable(3);
                header.HorizontalAlignment = 0;
                header.WidthPercentage = 100;
                header.SetWidths(new float[] { 100f, 320f, 100f });
                header.DefaultCell.Border = Rectangle.NO_BORDER;

                iTextSharp.text.Image logo = iTextSharp.text.Image.GetInstance(HttpContext.Server.MapPath("~/Content/Images/GP_BLUE.jpg"));
                
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
                    PdfPCell nextPostCell1 = new PdfPCell(new Phrase("INVOICE TO:", bodyFont));
                    nextPostCell1.Border = Rectangle.NO_BORDER;
                    nested.AddCell(nextPostCell1);
                    PdfPCell nextPostCell2 = new PdfPCell(new Phrase(" " + Name, bodyFont));
                    nextPostCell2.Border = Rectangle.NO_BORDER;
                    nested.AddCell(nextPostCell2);
                    PdfPCell nextPostCell3 = new PdfPCell(new Phrase("" + eventorderDetail.Date, bodyFont));
                    nextPostCell3.Border = Rectangle.NO_BORDER;
                    nested.AddCell(nextPostCell3);
                    PdfPCell nextPostCell4 = new PdfPCell(new Phrase(" " + Email, EmailFont));
                    nextPostCell4.Border = Rectangle.NO_BORDER;
                    nested.AddCell(nextPostCell4);
                    nested.AddCell("");
                    PdfPCell nesthousing = new PdfPCell(nested);
                    nesthousing.Border = Rectangle.NO_BORDER;
                    //nesthousing.BorderColorBottom = new BaseColor(System.Drawing.Color.Black);
                    //nesthousing.BorderWidthBottom = 1f;
                    nesthousing.Rowspan = 5;
                    nesthousing.PaddingBottom = 10f;
                    Invoicetable.AddCell(nesthousing);
                }
                {
                    PdfPCell middlecell = new PdfPCell();
                    middlecell.Border = Rectangle.NO_BORDER;
                    //middlecell.BorderColorBottom = new BaseColor(System.Drawing.Color.Black);
                    //middlecell.BorderWidthBottom = 1f;
                    Invoicetable.AddCell(middlecell);
                }

                {
                    PdfPTable nested = new PdfPTable(1);
                    nested.DefaultCell.Border = Rectangle.NO_BORDER;
                    PdfPCell nextPostCell1 = new PdfPCell(new Phrase("Invoice Number: " + eventorderDetail.Key, titleFontBlue));
                    nextPostCell1.Border = Rectangle.NO_BORDER;
                    nested.AddCell(nextPostCell1);

                    PdfPCell nextPostCell2 = new PdfPCell(new Phrase("Date of Invoice: " + eventorderDetail.Date.ToString("dd/MM/yyyy HH:mm:ss"), bodyFont));
                    nextPostCell2.Border = Rectangle.NO_BORDER;
                    nested.AddCell(nextPostCell2);

                    PdfPCell nextPostCell3 = new PdfPCell(new Phrase("Date of event: " + order.Date.ToString("dd/MM/yyyy HH:mm:ss"), bodyFont));
                    nextPostCell3.Border = Rectangle.NO_BORDER;
                    nested.AddCell(nextPostCell3);

                    
                    PdfPCell nextPostCell4 = new PdfPCell(new Phrase("Payment Mehtod: " + "PayFast", bodyFont));
                    nextPostCell4.Border = Rectangle.NO_BORDER;
                    nested.AddCell(nextPostCell4);



                    nested.AddCell("");
                    PdfPCell nesthousing = new PdfPCell(nested);
                    nesthousing.Border = Rectangle.NO_BORDER;
                    //nesthousing.BorderColorBottom = new BaseColor(System.Drawing.Color.Black);
                    //nesthousing.BorderWidthBottom = 1f;
                    nesthousing.Rowspan = 5;
                    nesthousing.PaddingBottom = 10f;
                    Invoicetable.AddCell(nesthousing);
                }


                doc.Add(header);
                Invoicetable.PaddingTop = 10f;

                doc.Add(Invoicetable);

                #region Items Table
                //Create body table
                PdfPTable itemTable = new PdfPTable(5);

                itemTable.HorizontalAlignment = 0;
                itemTable.WidthPercentage = 100;
                itemTable.SetWidths(new float[] { 5, 40, 10, 20, 25 });  // then set the column's __relative__ widths
                itemTable.SpacingAfter = 40;
                itemTable.DefaultCell.Border = Rectangle.BOX;
                PdfPCell cell1 = new PdfPCell(new Phrase("Pack ID", boldTableFont));
                cell1.BackgroundColor = TabelHeaderBackGroundColor;
                cell1.HorizontalAlignment = Element.ALIGN_CENTER;
                itemTable.AddCell(cell1);
                PdfPCell cell2 = new PdfPCell(new Phrase("Package NAME", boldTableFont));
                cell2.BackgroundColor = TabelHeaderBackGroundColor;
                cell2.HorizontalAlignment = 1;
                itemTable.AddCell(cell2);

                PdfPCell cell3 = new PdfPCell(new Phrase("Number of guests", boldTableFont));
                cell3.BackgroundColor = TabelHeaderBackGroundColor;
                cell3.HorizontalAlignment = Element.ALIGN_CENTER;
                itemTable.AddCell(cell3);

                PdfPCell cell4 = new PdfPCell(new Phrase("Price", boldTableFont));
                cell4.BackgroundColor = TabelHeaderBackGroundColor;
                cell4.HorizontalAlignment = Element.ALIGN_CENTER;
                itemTable.AddCell(cell4);

                //PdfPCell cell5 = new PdfPCell(new Phrase("Delivery Cost", boldTableFont));
                //cell5.BackgroundColor = TabelHeaderBackGroundColor;
                //cell5.HorizontalAlignment = Element.ALIGN_CENTER;
                //itemTable.AddCell(cell5);

                PdfPCell cell5 = new PdfPCell(new Phrase("TOTAL", boldTableFont));
                cell5.BackgroundColor = TabelHeaderBackGroundColor;
                cell5.HorizontalAlignment = Element.ALIGN_CENTER;
                itemTable.AddCell(cell5);

                foreach (EventCart cart in lstCart)
                {

                    eventorderDetail.packageName = cart.Package.packageName;
                    eventorderDetail.PackageId = cart.Package.PackageId;
                    eventorderDetail.Quantity = cart.Quantity;
                    eventorderDetail.Price = cart.Package.Price;
                    eventorderDetail.Date = DateTime.Now;

                    eventorderDetail.Deposit = temp.Sum(x => x.Quantity * x.Package.Price / 3);
                    eventorderDetail.Total = temp.Sum(x => x.Quantity * x.Package.Price /* + Convert.ToDecimal(Delivery)*/  ) + eventorderDetail.Deposit/* + Convert.ToDecimal(Delivery.Distance.ToString())*/;


                    PdfPCell numberCell = new PdfPCell(new Phrase("" + eventorderDetail.PackageId, bodyFont));
                    numberCell.HorizontalAlignment = 1;
                    numberCell.PaddingLeft = 10f;
                    numberCell.Border = Rectangle.LEFT_BORDER | Rectangle.RIGHT_BORDER;
                    itemTable.AddCell(numberCell);

                    var _phrase = new Phrase();
                    _phrase.Add(new Chunk(" " + eventorderDetail.packageName, bodyFont));
                    PdfPCell descCell = new PdfPCell(_phrase);
                    descCell.HorizontalAlignment = 0;
                    descCell.PaddingLeft = 10f;
                    descCell.Border = Rectangle.LEFT_BORDER | Rectangle.RIGHT_BORDER;
                    itemTable.AddCell(descCell);

                    PdfPCell qtyCell = new PdfPCell(new Phrase(" " + eventorderDetail.Quantity, bodyFont));
                    qtyCell.HorizontalAlignment = 1;
                    qtyCell.PaddingLeft = 10f;
                    qtyCell.Border = Rectangle.LEFT_BORDER | Rectangle.RIGHT_BORDER;
                    itemTable.AddCell(qtyCell);

                    PdfPCell amountCell = new PdfPCell(new Phrase(" " + eventorderDetail.Price, bodyFont));
                    amountCell.HorizontalAlignment = 1;
                    amountCell.PaddingLeft = 10f;
                    amountCell.Border = Rectangle.LEFT_BORDER | Rectangle.RIGHT_BORDER;
                    itemTable.AddCell(amountCell);

                    //PdfPCell deliveryamtCell = new PdfPCell(new Phrase("240 " /*+ Delivery*/, bodyFont));
                    //deliveryamtCell.HorizontalAlignment = 1;
                    //deliveryamtCell.Border = Rectangle.LEFT_BORDER | Rectangle.RIGHT_BORDER;
                    //itemTable.AddCell(deliveryamtCell);

                    PdfPCell totalamtCell = new PdfPCell(new Phrase(" " + temp.Sum(x => x.Quantity * x.Package.Price ), bodyFont));
                    totalamtCell.HorizontalAlignment = 1;
                    totalamtCell.Border = Rectangle.LEFT_BORDER | Rectangle.RIGHT_BORDER;
                    itemTable.AddCell(totalamtCell);

                }
                ////Delivery start
                //PdfPCell deliveryCell = new PdfPCell(new Phrase(""));
                //deliveryCell.Border = Rectangle.LEFT_BORDER | Rectangle.TOP_BORDER;
                //itemTable.AddCell(deliveryCell);

                //PdfPCell deliveryCell2 = new PdfPCell(new Phrase(""));
                //deliveryCell2.Border = Rectangle.TOP_BORDER; //Rectangle.NO_BORDER; //Rectangle.TOP_BORDER;
                //itemTable.AddCell(deliveryCell2);

                //PdfPCell deliveryCell3 = new PdfPCell(new Phrase(""));
                //deliveryCell3.Border = Rectangle.TOP_BORDER; //Rectangle.NO_BORDER; //Rectangle.TOP_BORDER;
                //itemTable.AddCell(deliveryCell3);

                //PdfPCell DeliveryCell = new PdfPCell(new Phrase("Delivery", boldTableFont));
                //DeliveryCell.Border = Rectangle.TOP_BORDER;   //Rectangle.NO_BORDER; //Rectangle.TOP_BORDER;
                //DeliveryCell.HorizontalAlignment = 1;
                //itemTable.AddCell(DeliveryCell);

                //PdfPCell DelCell = new PdfPCell(new Phrase("$" + Delivery, boldTableFont));
                //DelCell.HorizontalAlignment = 1;
                //itemTable.AddCell(DelCell);

                ////Delivery End

                //Deposit start

                PdfPCell depCell = new PdfPCell(new Phrase(""));
                depCell.Border = Rectangle.LEFT_BORDER | Rectangle.TOP_BORDER;
                itemTable.AddCell(depCell);

                PdfPCell depCell2 = new PdfPCell(new Phrase(""));
                depCell2.Border = Rectangle.TOP_BORDER; //Rectangle.NO_BORDER; //Rectangle.TOP_BORDER;
                itemTable.AddCell(depCell2);

                PdfPCell depCell3 = new PdfPCell(new Phrase(""));
                depCell3.Border = Rectangle.TOP_BORDER; //Rectangle.NO_BORDER; //Rectangle.TOP_BORDER;
                itemTable.AddCell(depCell3);

                PdfPCell depCelll = new PdfPCell(new Phrase("Deposit", boldTableFont));
                depCelll.Border = Rectangle.TOP_BORDER;   //Rectangle.NO_BORDER; //Rectangle.TOP_BORDER;
                depCelll.HorizontalAlignment = 1;
                itemTable.AddCell(depCelll);

                PdfPCell depCell4 = new PdfPCell(new Phrase("" + temp.Sum(x => x.Quantity * x.Package.Price /3).ToString("C"), boldTableFont));
                depCell4.HorizontalAlignment = 1;
                itemTable.AddCell(depCell4);

                //Deposit End

                // Table footer
                PdfPCell totalAmtCell1 = new PdfPCell(new Phrase(""));
                totalAmtCell1.Border = Rectangle.LEFT_BORDER | Rectangle.TOP_BORDER;
                itemTable.AddCell(totalAmtCell1);
                PdfPCell totalAmtCell2 = new PdfPCell(new Phrase(""));
                totalAmtCell2.Border = Rectangle.TOP_BORDER; //Rectangle.NO_BORDER; //Rectangle.TOP_BORDER;
                itemTable.AddCell(totalAmtCell2);
                PdfPCell totalAmtCell3 = new PdfPCell(new Phrase(""));
                totalAmtCell3.Border = Rectangle.TOP_BORDER; //Rectangle.NO_BORDER; //Rectangle.TOP_BORDER;
                itemTable.AddCell(totalAmtCell3);
                PdfPCell totalAmtStrCell = new PdfPCell(new Phrase("Total Amount", boldTableFont));
                totalAmtStrCell.Border = Rectangle.TOP_BORDER;   //Rectangle.NO_BORDER; //Rectangle.TOP_BORDER;
                totalAmtStrCell.HorizontalAlignment = 1;
                itemTable.AddCell(totalAmtStrCell);
                PdfPCell totalAmtCell = new PdfPCell(new Phrase("" + temp.Sum(x => x.Quantity * x.Package.Price + eventorderDetail.Deposit).ToString("C"), boldTableFont));
                totalAmtCell.HorizontalAlignment = 1;
                itemTable.AddCell(totalAmtCell);

                PdfPCell cell = new PdfPCell(new Phrase("***NOTICE: A finance charge of 1.5% will be made on unpaid balances after 30 days. ***", bodyFont));
                cell.Colspan = 5;
                cell.HorizontalAlignment = 1;
                itemTable.AddCell(cell);
                doc.Add(itemTable);
                #endregion

                PdfContentByte cb = new PdfContentByte(writer);


                BaseFont bf = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1250, true);
                cb = new PdfContentByte(writer);
                cb = writer.DirectContent;
                cb.BeginText();
                cb.SetFontAndSize(bf, 8);
                cb.SetTextMatrix(pageSize.GetLeft(120), 20);
                cb.ShowText("Invoice was created on a computer and is valid without the signature and seal. ");
                cb.EndText();

                //Move the pointer and draw line to separate footer section from rest of page
                cb.MoveTo(40, doc.PageSize.GetBottom(50));
                cb.LineTo(doc.PageSize.Width - 40, doc.PageSize.GetBottom(50));
                cb.Stroke();




                //PdfContentByte content = writer.DirectContent;
                //Rectangle rectangle = new Rectangle(doc.PageSize);
                //rectangle.Left += doc.LeftMargin;
                //rectangle.Right -= doc.RightMargin;
                //rectangle.Top -= doc.TopMargin;
                //rectangle.Bottom += doc.BottomMargin;
                //content.SetColorStroke(GrayColor.BLACK);
                //content.Rectangle(rectangle.Left, rectangle.Bottom, rectangle.Width, rectangle.Height);
                //content.Stroke();


                writer.CloseStream = false;
                doc.Close();

                



                memoryStream.Position = 0;
               

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
                attachment = new System.Net.Mail.Attachment(memoryStream, "order.pdf");
                MailMessage msz = new MailMessage(Email, Email)
                {
                    From = new MailAddress(ConfigurationManager.AppSettings["Email"].ToString()),
                    Subject = "order Details for " + Name.ToUpper(),

                    IsBodyHtml = true,
                    Body = " Good Day : " + Name.ToUpper() + ", Please find attached order information for order ID : " + eventorderDetail.Key,
                };
                msz.Attachments.Add(attachment);

                client.Send(msz);


                ModelState.Clear();

                // end

                //var at = new SendGrid.Helpers.Mail.Attachment(memoryStream, "Order.pdf");
                //msz.Attachments.Add(new System.Net.Mail.Attachment(memoryStream, "order.pdf"));
                // msz.AddAttachment(Server.MapPath(""));
                //SmtpClient smtp = new SmtpClient();

                // var response = client.SendMailAsync(msz);
                //client.SendAsync(msz, null);
                // smtp.SendAsync(msz);

                //return File(memoryStream.ToArray(), "application/pdf", "Order.pdf");

            }

            catch (Exception ex)
            {
                ModelState.Clear();
                
            }

        
            Session.Remove(stCart);



            return RedirectToAction("Index", "Home");
        }
       
        public ActionResult OnceOff()
        {
            var onceOffRequest = new PayFastRequest(this.payFastSettings.PassPhrase);

            // Merchant Details
            onceOffRequest.merchant_id = this.payFastSettings.MerchantId;
            onceOffRequest.merchant_key = this.payFastSettings.MerchantKey;
            onceOffRequest.return_url = this.payFastSettings.ReturnUrl;
            onceOffRequest.cancel_url = this.payFastSettings.CancelUrl;
            onceOffRequest.notify_url = this.payFastSettings.NotifyUrl;

            // Buyer Details
            onceOffRequest.email_address = "sbtu01@payfast.co.za";

            // Transaction Details
            List<EventCart> listCart = (List<EventCart>)Session[stCart];
            foreach (var cart in listCart)
            {
                //var deposit = String.Format("{0:C}", listCart.Sum(x => x.Quantity * x.Package.Price / 3));
                //var total = String.Format("{0:C}", listCart.Sum(x => x.Quantity * x.Package.Price) + deposit);

                //double fTotal = Convert.ToDouble(deposit + total);
                double total = Convert.ToDouble(listCart.Sum(x => x.Quantity * x.Package.Price) + listCart.Sum(x => x.Quantity * x.Package.Price / 3));
                string name = cart.Package.packageName;
                string Desc = cart.Package.Description;
                onceOffRequest.m_payment_id = "8d00bf49-e979-4004-228c-08d452b86380";
                onceOffRequest.amount = total;
                onceOffRequest.item_name =cart.Quantity + "x" + name;
                //onceOffRequest.item_description ="sdfdsvc";
               
            }
            // Transaction Options
            onceOffRequest.email_confirmation = true;
            onceOffRequest.confirmation_address = "sbtu01@payfast.co.za";

            var redirectUrl = $"{this.payFastSettings.ProcessUrl}{onceOffRequest.ToString()}";
            /*return View("OrderSuccess");*/ //enable for local testing
            return Redirect(redirectUrl);
        }
        [HttpPost]
        public async Task<ActionResult> Notify([ModelBinder(typeof(PayFastNotifyModelBinder))] PayFastNotify payFastNotifyViewModel)
        {
            payFastNotifyViewModel.SetPassPhrase(this.payFastSettings.PassPhrase);

            var calculatedSignature = payFastNotifyViewModel.GetCalculatedSignature();

            var isValid = payFastNotifyViewModel.signature == calculatedSignature;

            System.Diagnostics.Debug.WriteLine($"Signature Validation Result: {isValid}");

            // The PayFast Validator is still under developement
            // Its not recommended to rely on this for production use cases
            var payfastValidator = new PayFastValidator(this.payFastSettings, payFastNotifyViewModel, IPAddress.Parse(this.HttpContext.Request.UserHostAddress));

            var merchantIdValidationResult = payfastValidator.ValidateMerchantId();

            System.Diagnostics.Debug.WriteLine($"Merchant Id Validation Result: {merchantIdValidationResult}");

            var ipAddressValidationResult = payfastValidator.ValidateSourceIp();

            System.Diagnostics.Debug.WriteLine($"Ip Address Validation Result: {ipAddressValidationResult}");

            // Currently seems that the data validation only works for successful payments
            if (payFastNotifyViewModel.payment_status == PayFastStatics.CompletePaymentConfirmation)
            {
                var dataValidationResult = await payfastValidator.ValidateData();

                System.Diagnostics.Debug.WriteLine($"Data Validation Result: {dataValidationResult}");
            }

            if (payFastNotifyViewModel.payment_status == PayFastStatics.CancelledPaymentConfirmation)
            {
                System.Diagnostics.Debug.WriteLine($"Subscription was cancelled");
            }

            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }



        //    public ActionResult PaymentWithPaypal(string Cancel = null)
        //    {
        //        //getting the apiContext
        //        APIContext apiContext = PayPalConfig.GetAPIContext();

        //        try
        //        {
        //            //A resource representing a Payer that funds a payment Payment Method as paypal
        //            //Payer Id will be returned when payment proceeds or click to pay
        //            string payerId = Request.Params["PayerID"];

        //            if (string.IsNullOrEmpty(payerId))
        //            {
        //                //this section will be executed first because PayerID doesn't exist
        //                //it is returned by the create function call of the payment class

        //                // Creating a payment
        //                // baseURL is the url on which paypal sendsback the data.
        //                string baseURI = Request.Url.Scheme + "://" + Request.Url.Authority +
        //                            "/EventCart/PaymentWithPayPal?";

        //                //here we are generating guid for storing the paymentID received in session
        //                //which will be used in the payment execution

        //                var guid = Convert.ToString((new Random()).Next(100000));

        //                //CreatePayment function gives us the payment approval url
        //                //on which payer is redirected for paypal account payment

        //                var createdPayment = this.CreatePayment(apiContext, baseURI + "guid=" + guid);

        //                //get links returned from paypal in response to Create function call

        //                var links = createdPayment.links.GetEnumerator();

        //                string paypalRedirectUrl = null;

        //                while (links.MoveNext())
        //                {
        //                    Links lnk = links.Current;

        //                    if (lnk.rel.ToLower().Trim().Equals("approval_url"))
        //                    {
        //                        //saving the payapalredirect URL to which user will be redirected for payment
        //                        paypalRedirectUrl = lnk.href;
        //                    }
        //                }

        //                // saving the paymentID in the key guid
        //                Session.Add(guid, createdPayment.id);

        //                return Redirect(paypalRedirectUrl);
        //            }
        //            else
        //            {

        //                // This function exectues after receving all parameters for the payment

        //                var guid = Request.Params["guid"];

        //                var executedPayment = ExecutePayment(apiContext, payerId, Session[guid] as string);

        //                //If executed payment failed then we will show payment failure message to user
        //                if (executedPayment.state.ToLower() != "approved")
        //                {
        //                    return View("Failure");
        //                }
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            return View("OrderSuccess");
        //        }

        //        //on successful payment, show success page to user.
        //        return View("OrderSuccess");
        //    }
        //    private PayPal.Api.Payment payment;
        //    private Payment ExecutePayment(APIContext apiContext, string payerId, string paymentId)
        //    {
        //        var paymentExecution = new PaymentExecution() { payer_id = payerId };
        //        this.payment = new Payment() { id = paymentId };
        //        return this.payment.Execute(apiContext, paymentExecution);
        //    }

        //    private Payment CreatePayment(APIContext apiContext, string redirectUrl)
        //    {

        //        //create itemlist and add item objects to it
        //        var itemList = new ItemList() { items = new List<Item>() };
        //        List<EventCart> listCart = (List<EventCart>)Session[stCart];
        //        foreach (var cart in listCart)
        //        {
        //            itemList.items.Add(new Item()
        //            {
        //                name = cart.Package.packageName /*"Item Name comes here"*/,
        //                currency = "USD",
        //                price = cart.Package.Price.ToString(),
        //                quantity = cart.Quantity.ToString(),
        //                sku = "sku"
        //                //name = "Item Name comes here",
        //                //   currency = "USD",
        //                //price = "1"
        //                //quantity = "1"
        //                //    sku = "sku"
        //            });
        //        }


        //        var payer = new Payer() { payment_method = "paypal" };

        //        // Configure Redirect Urls here with RedirectUrls object
        //        var redirUrls = new RedirectUrls()
        //        {
        //            cancel_url = redirectUrl + "&Cancel=true",
        //            return_url = redirectUrl
        //        };

        //        // Adding Tax, shipping and Subtotal details
        //        var details = new Details()
        //        {
        //            //tax = "1",
        //            //shipping = "1",
        //            insurance = listCart.Sum(x => x.Quantity * x.Package.Price/3).ToString(),
        //            subtotal = listCart.Sum(x => x.Quantity * x.Package.Price).ToString()
        //        };

        //        //Final amount with details
        //        var amount = new Amount()
        //        {
        //            currency = "USD",
        //            total = Convert.ToDouble(details.subtotal) + Convert.ToDouble(details.insurance).ToString(), // Total must be equal to sum of tax, shipping and subtotal.
        //            details = details
        //        };

        //        var transactionList = new List<Transaction>();
        //        // Adding description about the transaction
        //        transactionList.Add(new Transaction()
        //        {
        //            description = "Transaction description",
        //            invoice_number = "your invoice number", //Generate an Invoice No
        //            amount = amount,
        //            item_list = itemList
        //        });


        //        this.payment = new Payment()
        //        {
        //            intent = "sale",
        //            payer = payer,
        //            transactions = transactionList,
        //            redirect_urls = redirUrls
        //        };

        //        // Create a payment using a APIContext
        //        return this.payment.Create(apiContext);




        //}
        // POST: EventCart/Delete/5
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
