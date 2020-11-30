using EventBook.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using UserRoles.Models;
using PayPal.Api;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Web.Security;
using System.Web.WebSockets;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.Configuration;
using Microsoft.AspNet.Identity;
using System.IO;

using EllipticCurve;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web.UI.WebControls;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.Ajax.Utilities;

using System.Text;
using Image = iTextSharp.text.Image;
using iTextSharp.tool.xml.html;
using iTextSharp.text.html;
using System.Data.Odbc;
using System.Security.Claims;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security.Provider;
using QRCoder;
using System.Data.Entity.Validation;
using PayFast;
using PayFast.AspNet;
namespace UserRoles.Controllers
{
    [Authorize]

    public class ShoppingCartController : Controller
    {
        ApplicationDbContext db = new ApplicationDbContext();

        private string strCart = "Cart";
        private readonly PayFastSettings payFastSettings;

        public ShoppingCartController()
        {
            this.payFastSettings = new PayFastSettings();
            this.payFastSettings.MerchantId = ConfigurationManager.AppSettings["MerchantId"];
            this.payFastSettings.MerchantKey = ConfigurationManager.AppSettings["MerchantKey"];
            this.payFastSettings.PassPhrase = ConfigurationManager.AppSettings["PassPhrase"];
            this.payFastSettings.ProcessUrl = ConfigurationManager.AppSettings["ProcessUrl"];
            this.payFastSettings.ValidateUrl = ConfigurationManager.AppSettings["ValidateUrl"];
            this.payFastSettings.ReturnUrl = ConfigurationManager.AppSettings["ReturnUr"];
            //this.payFastSettings.ReturnUrl = ConfigurationManager.AppSettings["ReturnUrl"];
            this.payFastSettings.CancelUrl = ConfigurationManager.AppSettings["CancelUrl"];
            this.payFastSettings.NotifyUrl = ConfigurationManager.AppSettings["NotifyUrl"];
        }
        // GET: ShoppingCart
        public ActionResult Index()
        {

            // Return the view
            return View();
            //return View();
        }

        public ActionResult OrderNow(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (Session[strCart] == null)
            {
                List<Cart> IsCart = new List<Cart>
                {
                    new Cart(db.ItemsHires.Find(id),1),
                    
                };
                Session[strCart] = IsCart;
            }
            else
            {
                List<Cart> IsCart = (List<Cart>)Session[strCart];
                int check = isExistingCheck(id);
                if (check == -1)
                    IsCart.Add(new Cart(db.ItemsHires.Find(id), 1));
                else
                    IsCart[check].Quantity++;

                Session[strCart] = IsCart;
            }
            return RedirectToAction("ItemsInventory", "ItemsHires");
        }
        private int isExistingCheck(int? id)
        {
            List<Cart> IsCart = (List<Cart>)Session[strCart];
            for (int i = 0; i < IsCart.Count; i++)
            {
                if (IsCart[i].ItemsHire.ProductID == id) return i;
            }
            return -1;
        }
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            int check = isExistingCheck(id);
            List<Cart> IsCart = (List<Cart>)Session[strCart];
            IsCart.RemoveAt(check);
            Session["Cart"] = IsCart;
            return View("Index");
        }
        public ActionResult UpdateCart(FormCollection frc)
        {

            string[] quantities = frc.GetValues("quantity");
            List<Cart> lstCart = (List<Cart>)Session[strCart];
            for (int i = 0; i < lstCart.Count; i++)
            {

                lstCart[i].Quantity = Convert.ToInt32(quantities[i]);
            }
            Session[strCart] = lstCart;
            return View("Index");
        }
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ItemsHire itemsHire = db.ItemsHires.Find(id);
            if (itemsHire == null)
            {
                return HttpNotFound();
            }
            return View(itemsHire);
        }
        public ActionResult Checkout(FormCollection frc)
        {

            return View("Checkout");
        }
        public ActionResult OrderSuccess()
        {

            return View();
        }

        public ActionResult DetailCapture()
        {
            
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DetailCapture([Bind(Include = "MySignature,CustomerName,Surname,CustomerPhone,Email,CollDate,OrderDate,PaymentType,ExpectedReturnDate,ReminderDate")] Models.Order order)
        {
            var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
            ApplicationUser currentUser = UserManager.FindById(User.Identity.GetUserId());
            string ID = currentUser.Id;
            string Email = currentUser.Email;
            string Name = currentUser.Name;
            string SName = currentUser.Surname;
            string phone = currentUser.PhoneNumber;
            byte[] Sign = currentUser.MySignature;

            if (order.CollDate.Date > DateTime.Today  //no previous dates
                && order.CollDate.Date != DateTime.Today) // no booking today

            {
                try
                {
                    if (ModelState.IsValid)
                    {
                        order.MySignature = Sign;
                        order.OrderDate = DateTime.Now;
                        order.ExpectedReturnDate = order.CollDate.AddDays(2);
                        order.PaymentType = "PayFast";
                        order.SelfReturnReminder = order.ExpectedReturnDate.AddDays(-1);
                        order.ReminderDate = order.CollDate.AddDays(-1);
                        db.Orders.Add(order);
                        db.SaveChanges();
                        return RedirectToAction("Create", "Maps");
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
            return View(/*order*/);
        }


        [Obsolete]
        public async Task<ActionResult> ProcessOrderAsync(FormCollection frc)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            var Delivery = (from x in db.Maps
                            where x.Email == User.Identity.Name
                            orderby x.Id descending
                            select x.Distance).First().ToString();
            var order = (from i in db.Orders
                         where i.Email == User.Identity.Name
                         orderby i.OrderID descending
                         select i).First();
            // string cos = Delivery.Distance;
            var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
            ApplicationUser currentUser = UserManager.FindById(User.Identity.GetUserId());
            string ID = currentUser.Id;
            string Email = currentUser.Email;
            string Name = currentUser.Name;
            string SName = currentUser.Surname;
            byte[] Sign = currentUser.MySignature;
            //var Delivery = (from i in db.Maps
            //                select i).Last();

            List<Cart> lstCart = (List<Cart>)Session[strCart];
            //Models.Order ord = new Models.Order();

            //string a = ord.CollDate.ToString();
            //1.savw the order into order table
           //var order = new Models.Order()
           //{
           //    MySignature = Sign,
           //    CustomerName = frc["cusName"],
           //    Surname = frc["cusSurname"],
           //    CustomerPhone = frc["cusPhone"],
           //    Email = frc["cusEmail"],
           //     //CollDate = Convert.ToDateTime(frc["colDate"]).ToShortDateString(),
           //     CollDate = Convert.ToDateTime(frc["collDate"]).ToShortDateString(),
           //      OrderDate = DateTime.Now,
           //     PaymentType = "PayPal",
           //     //ExpectedReturnDate = Convert.ToDateTime(frc["collDate"]).AddDays(2).ToString("dd/MM/yyyy")
           //     ExpectedReturnDate = Convert.ToDateTime(frc["collDate"]).Date.AddDays(2)
           //     //Status = "Processing"

           // };
           // db.Orders.Add(order);
            //string ReturnDate = "";
            //ReturnDate = Convert.ToDateTime(order.CollDate).AddDays(3).ToString(/*"dd/MM/yyyy"*/);

            OrderDetail orderDetail = new OrderDetail();
          
            //2. save the order detail into the Order detail table
            List<string> productName = new List<string>();
            List<int> productId = new List<int>();
            List<int> productQuantity = new List<int>();
            List<decimal> productPrice = new List<decimal>();
            List<string> distance = new List<string>();
            //List<decimal> deposit = new List<decimal>();
            List<Cart> temp = (List<Cart>)Session["Cart"];
            //var deposit = String.Format("{0:C}", temp.Sum(x => x.Quantity * x.ItemsHire.Price / 2));
            var total = String.Format("{0:C}", (temp.Sum(x => x.Quantity * x.ItemsHire.Price) + temp.Sum(x => x.Quantity * x.ItemsHire.Price/2)) /* + Delivery.ToString()*//* + Delivery.Distance.ToString()*/) ;
            foreach (Cart cart in lstCart)
            {
                
                orderDetail.OrderID = order.OrderID;
                orderDetail.ProductName = cart.ItemsHire.ProductName;
                orderDetail.ProductID = cart.ItemsHire.ProductID;
                orderDetail.Quantity = cart.Quantity;
                orderDetail.Price = cart.ItemsHire.Price;
                //orderDetail.Distance = Delivery.ToString();
                orderDetail.Deposit = temp.Sum(x => x.Quantity * x.ItemsHire.Price/2); 
                orderDetail.Total = temp.Sum(x => x.Quantity * x.ItemsHire.Price /* + Convert.ToDecimal(Delivery)*/  ) + orderDetail.Deposit/* + Convert.ToDecimal(Delivery.Distance.ToString())*/; 


                productName.Add(cart.ItemsHire.ProductName);
                productQuantity.Add(cart.Quantity);
                productPrice.Add(cart.ItemsHire.Price);
                //deposit.Add(orderDetail.Deposit);


                db.OrderDetails.Add(orderDetail);
                ItemsHire prod = (from p in db.ItemsHires where p.ProductID == orderDetail.ProductID select p).Single();
                prod.Quantity -= orderDetail.Quantity;
                db.SaveChanges();
                
            }
            try
            {
                ViewBag.Orders = db.Orders.ToList().FindAll(x => x.OrderID == order.OrderID);
               

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
                    PdfPCell nextPostCell1 = new PdfPCell(new Phrase("INVOICE TO:", bodyFont));
                    nextPostCell1.Border = Rectangle.NO_BORDER;
                    nested.AddCell(nextPostCell1);
                    PdfPCell nextPostCell2 = new PdfPCell(new Phrase(" " + Name , bodyFont));
                    nextPostCell2.Border = Rectangle.NO_BORDER;
                    nested.AddCell(nextPostCell2);
                    PdfPCell nextPostCell3 = new PdfPCell(new Phrase("" + order.CollDate, bodyFont));
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
                    PdfPCell nextPostCell1 = new PdfPCell(new Phrase("Invoice Number: " + order.OrderID, titleFontBlue));
                    nextPostCell1.Border = Rectangle.NO_BORDER;
                    nested.AddCell(nextPostCell1);

                    PdfPCell nextPostCell2 = new PdfPCell(new Phrase("Date of Invoice: " + order.OrderDate.ToString("dd/MM/yyyy HH:mm:ss"), bodyFont));
                    nextPostCell2.Border = Rectangle.NO_BORDER;
                    nested.AddCell(nextPostCell2);
                    
                    PdfPCell nextPostCell3 = new PdfPCell(new Phrase("Pick Up Date: " + order.CollDate.ToShortDateString(), bodyFont));
                    nextPostCell3.Border = Rectangle.NO_BORDER;
                    nested.AddCell(nextPostCell3);

                    PdfPCell nextPostCell4 = new PdfPCell(new Phrase("Return Date: " + order.ExpectedReturnDate.ToShortDateString(), bodyFont));
                    nextPostCell4.Border = Rectangle.NO_BORDER;
                    nested.AddCell(nextPostCell4);


                    //PdfPCell nextPostCell5 = new PdfPCell(new Phrase(" Hire Period: " , bodyFont));
                    //nextPostCell5.Border = Rectangle.NO_BORDER;
                    //nested.AddCell(nextPostCell5);

                    
                    PdfPCell nextPostCell5 = new PdfPCell(new Phrase("Collection Status: " + order.PickUp, bodyFont));
                    nextPostCell5.Border = Rectangle.NO_BORDER;
                    nested.AddCell(nextPostCell5);

                    PdfPCell nextPostCell6 = new PdfPCell(new Phrase("Collectors Name: " + order.CustomerName, bodyFont));
                    nextPostCell6.Border = Rectangle.NO_BORDER;
                    nested.AddCell(nextPostCell6);

                    //PdfPCell nextPostCell6 = new PdfPCell(new Phrase("Collectors Address" + order.CustomerName, bodyFont));
                    //nextPostCell5.Border = Rectangle.NO_BORDER;
                    //nested.AddCell(nextPostCell6);

                    PdfPCell nextPostCell7 = new PdfPCell(new Phrase("Collectors Number: " + "\n" + order.CustomerPhone, bodyFont));
                    nextPostCell7.Border = Rectangle.NO_BORDER;
                    nested.AddCell(nextPostCell7);

                    PdfPCell nextPostCell8 = new PdfPCell(new Phrase("Payment Mehtod: " + "PayFast", bodyFont));
                    nextPostCell8.Border = Rectangle.NO_BORDER;
                    nested.AddCell(nextPostCell8);

                   

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
                itemTable.SetWidths(new float[] {5, 40, 10, 20, 25 });  // then set the column's __relative__ widths
                itemTable.SpacingAfter = 40;
                itemTable.DefaultCell.Border = Rectangle.BOX;
                PdfPCell cell1 = new PdfPCell(new Phrase("PROD ID", boldTableFont));
                cell1.BackgroundColor = TabelHeaderBackGroundColor;
                cell1.HorizontalAlignment = Element.ALIGN_CENTER;
                itemTable.AddCell(cell1);
                PdfPCell cell2 = new PdfPCell(new Phrase("PRODUCT NAME", boldTableFont));
                cell2.BackgroundColor = TabelHeaderBackGroundColor;
                cell2.HorizontalAlignment = 1;
                itemTable.AddCell(cell2);

                PdfPCell cell3 = new PdfPCell(new Phrase("QUANTITY", boldTableFont));
                cell3.BackgroundColor = TabelHeaderBackGroundColor;
                cell3.HorizontalAlignment = Element.ALIGN_CENTER;
                itemTable.AddCell(cell3);

                PdfPCell cell4 = new PdfPCell(new Phrase("UNIT AMOUNT", boldTableFont));
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
               
                foreach (Cart cart in lstCart)
                {
                    orderDetail.OrderID = order.OrderID;
                    orderDetail.ProductName = cart.ItemsHire.ProductName;
                    orderDetail.ProductID = cart.ItemsHire.ProductID;
                    orderDetail.Quantity = cart.Quantity;
                    orderDetail.Price = cart.ItemsHire.Price;
                    orderDetail.SubTotal = cart.Quantity * cart.ItemsHire.Price;
                    orderDetail.Deposit = temp.Sum(x => x.Quantity * x.ItemsHire.Price/2)  ;
                    orderDetail.Total = orderDetail.SubTotal + orderDetail.Deposit + Convert.ToDecimal(Delivery) /*temp.Sum(x => x.Quantity * cart.ItemsHire.Price)*//* + Convert.ToDecimal(Delivery) + orderDetail.Deposit*/;

                    PdfPCell numberCell = new PdfPCell(new Phrase( ""+ orderDetail.ProductID, bodyFont));
                    numberCell.HorizontalAlignment = 1;
                    numberCell.PaddingLeft = 10f;
                    numberCell.Border = Rectangle.LEFT_BORDER | Rectangle.RIGHT_BORDER;
                    itemTable.AddCell(numberCell);

                    var _phrase = new Phrase();
                    _phrase.Add(new Chunk(" " + orderDetail.ProductName, bodyFont));
                    PdfPCell descCell = new PdfPCell(_phrase);
                    descCell.HorizontalAlignment = 0;
                    descCell.PaddingLeft = 10f;
                    descCell.Border = Rectangle.LEFT_BORDER | Rectangle.RIGHT_BORDER;
                    itemTable.AddCell(descCell);

                    PdfPCell qtyCell = new PdfPCell(new Phrase(" "+ orderDetail.Quantity, bodyFont));
                    qtyCell.HorizontalAlignment = 1;
                    qtyCell.PaddingLeft = 10f;
                    qtyCell.Border = Rectangle.LEFT_BORDER | Rectangle.RIGHT_BORDER;
                    itemTable.AddCell(qtyCell);

                    PdfPCell amountCell = new PdfPCell(new Phrase(" " + orderDetail.Price, bodyFont));
                    amountCell.HorizontalAlignment = 1;
                    amountCell.PaddingLeft = 10f;
                    amountCell.Border = Rectangle.LEFT_BORDER | Rectangle.RIGHT_BORDER;
                    itemTable.AddCell(amountCell);

                    //PdfPCell deliveryamtCell = new PdfPCell(new Phrase("240 " /*+ Delivery*/, bodyFont));
                    //deliveryamtCell.HorizontalAlignment = 1;
                    //deliveryamtCell.Border = Rectangle.LEFT_BORDER | Rectangle.RIGHT_BORDER;
                    //itemTable.AddCell(deliveryamtCell);

                    PdfPCell totalamtCell = new PdfPCell(new Phrase(" "+ orderDetail.SubTotal, bodyFont));
                    totalamtCell.HorizontalAlignment = 1;
                    totalamtCell.Border = Rectangle.LEFT_BORDER | Rectangle.RIGHT_BORDER;
                    itemTable.AddCell(totalamtCell);

                }
                //Delivery start
                PdfPCell deliveryCell = new PdfPCell(new Phrase(""));
                deliveryCell.Border = Rectangle.LEFT_BORDER | Rectangle.TOP_BORDER;
                itemTable.AddCell(deliveryCell);

                PdfPCell deliveryCell2 = new PdfPCell(new Phrase(""));
                deliveryCell2.Border = Rectangle.TOP_BORDER; //Rectangle.NO_BORDER; //Rectangle.TOP_BORDER;
                itemTable.AddCell(deliveryCell2);

                PdfPCell deliveryCell3 = new PdfPCell(new Phrase(""));
                deliveryCell3.Border = Rectangle.TOP_BORDER; //Rectangle.NO_BORDER; //Rectangle.TOP_BORDER;
                itemTable.AddCell(deliveryCell3);

                PdfPCell DeliveryCell = new PdfPCell(new Phrase("Delivery", boldTableFont));
                DeliveryCell.Border = Rectangle.TOP_BORDER;   //Rectangle.NO_BORDER; //Rectangle.TOP_BORDER;
                DeliveryCell.HorizontalAlignment = 1;
                itemTable.AddCell(DeliveryCell);

                PdfPCell DelCell = new PdfPCell(new Phrase("$" + Delivery, boldTableFont));
                DelCell.HorizontalAlignment = 1;
                itemTable.AddCell(DelCell);

                //Delivery End

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

                PdfPCell depCell4 = new PdfPCell(new Phrase("$" + orderDetail.Deposit, boldTableFont));
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
                PdfPCell totalAmtCell = new PdfPCell(new Phrase("$" + orderDetail.Total , boldTableFont));
                totalAmtCell.HorizontalAlignment = 1;
                itemTable.AddCell(totalAmtCell);

                PdfPCell cell = new PdfPCell(new Phrase("Thank You For Your Support", bodyFont));
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

                QRModel qr = new QRModel();

                QRCodeGenerator ObjQr = new QRCodeGenerator();

                qr.Message = "https://2020grp26.azurewebsites.net/Order/Details/"+ order.OrderID;

                QRCodeData qrCodeData = ObjQr.CreateQrCode(qr.Message, QRCodeGenerator.ECCLevel.Q);

                System.Drawing.Bitmap bitMap = new QRCode(qrCodeData).GetGraphic(20);

                using (MemoryStream ms = new MemoryStream())

                {

                    bitMap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);

                    byte[] byteImage = ms.ToArray();

                    ViewBag.Url = "data:image/png;base64," + Convert.ToBase64String(byteImage);
                    ms.Position = 0;



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
                    System.Net.Mail.Attachment attach;
                    attachment = new System.Net.Mail.Attachment(memoryStream, "order.pdf");
                    attach = new System.Net.Mail.Attachment(ms, "order.png");
                    MailMessage msz = new MailMessage(Email, Email)
                    {
                        From = new MailAddress(ConfigurationManager.AppSettings["Email"].ToString()),
                        Subject = "order Details for " + Name.ToUpper(),

                        IsBodyHtml = true,
                        Body = " Good Day : " + Name.ToUpper() + ", Please find attached order information for order ID : " + order.OrderID,
                    };
                    msz.Attachments.Add(attachment);
                    msz.Attachments.Add(attach);

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

                }

            }

            catch (Exception ex)
            {
                ModelState.Clear();
                ViewBag.Message = $"sorry we are facing a problem{ex.Message}";
            }

           

            //3. Remove shopping cart session
            Session.Remove(strCart);



            return RedirectToAction("Index","Home");
        }
        public ActionResult OnceOff()
        {
            var Delivery = (from x in db.Maps
                            where x.Email == User.Identity.Name
                            orderby x.Id descending
                            select x.Distance).First().ToString();
            double del = Convert.ToDouble(Delivery);
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

            List<Cart> listCart = (List<Cart>)Session[strCart];
            foreach (var cart in listCart)
            {

                double total = Convert.ToDouble(listCart.Sum(x => x.Quantity * x.ItemsHire.Price) + listCart.Sum(x => x.Quantity * x.ItemsHire.Price / 2)) + del;
                //string name = cart.ItemsHire.ProductName;
                string Desc = cart.ItemsHire.Description;
                onceOffRequest.m_payment_id = "8d00bf49-e979-4004-228c-08d452b86380";
                onceOffRequest.item_name = ".";
                onceOffRequest.amount = Convert.ToDouble(total);


            }
            // Transaction Options
            onceOffRequest.email_confirmation = true;
            onceOffRequest.confirmation_address = "sbtu01@payfast.co.za";

            var redirectUrl = $"{this.payFastSettings.ProcessUrl}{onceOffRequest.ToString()}";
            //return View("OrderSuccess");
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






        //PayPal Payement
        //private Payment payment;
        
        //// Create a Payment with apiContext
        //private Payment CreatePayment(APIContext apiContext, string redirectUrl)
        //{
        //    //var Delivery = (from x in db.Maps.Take(1)
                            
        //    //                select x.Distance);


        //    var Delivery = (from x in db.Maps
        //                    where x.Email == User.Identity.Name
        //                    orderby x.Id descending
        //                    select x.Distance).First().ToString();
            



        //    var listItems = new ItemList() { items = new List<PayPal.Api.Item>() };

        //    List<Cart> listCart = (List<Cart>)Session[strCart];
        //    foreach (var cart in listCart)
        //    {
        //        listItems.items.Add(new PayPal.Api.Item()
        //        {
        //            name = cart.ItemsHire.ProductName,
        //            currency = "USD",
        //            price = cart.ItemsHire.Price.ToString(),
        //            quantity = cart.Quantity.ToString(),
        //            sku = "sku"
        //        });
        //    }

        //    var payer = new Payer() { payment_method = "paypal" };

        //    //config  for redirectURLS 
        //    var redirUrls = new RedirectUrls()
        //    {
        //        cancel_url = redirectUrl,
        //        return_url = redirectUrl
        //    };
        //    //string dist = Delivery.ToString();
        //    // create detail object
           
        //    var details = new Details()
        //    {
                 
                
        //    tax = "0",
        //    insurance = listCart.Sum(x => x.Quantity * x.ItemsHire.Price/2).ToString(),
        //        //shipping = " 0"/*Convert.ToString(Delivery),*/,
        //       shipping = Delivery,
        //        subtotal = listCart.Sum(x => x.Quantity * x.ItemsHire.Price ) .ToString()

        //    };

        //    // Create amount object
        //    var amount = new Amount()
        //    {
        //        currency = "USD",
        //        total = (Convert.ToDouble(details.tax) + Convert.ToDouble(details.subtotal) + Convert.ToDouble(details.shipping) + Convert.ToDouble(details.insurance)).ToString(),
        //        details = details
        //    };

        //    //create transaction
        //    var transactionList = new List<Transaction>();
        //    transactionList.Add(new Transaction()
        //    {
        //        description = "testing transacton description",
        //        invoice_number = Convert.ToString((new Random()).Next(100000)),
        //        amount = amount,
        //        item_list = listItems
        //    });

        //    payment = new Payment()
        //    {
        //        intent = "sale",
        //        payer = payer,
        //        transactions = transactionList,
        //        redirect_urls = redirUrls
        //    };

        //    return payment.Create(apiContext);
        //}

        ////Create Execute Paymenent
        //private Payment ExecutePayment(APIContext apiContext, string payerId, string paymentId)
        //{
        //    var paymentExecution = new PaymentExecution()
        //    {
        //        payer_id = payerId
        //    };
        //    payment = new Payment() { id = paymentId };
        //    return payment.Execute(apiContext, paymentExecution);
        //}

        //// Create PaymentWithPaypal
        
        //public ActionResult PaymentWithPaypal()
        //{
        //    //Getting context from the paypal base on slientId and secret for payment
        //    APIContext apiContext = PaypalConfiguration.GetAPIContext();
        //    try
        //    {
        //        string payerId = Request.Params["PayerID"];
        //        if (string.IsNullOrEmpty(payerId))
        //        {
        //            //Create a payement
        //            string baseURI = Request.Url.Scheme + "://" + Request.Url.Authority + "/ShoppingCart/PaymentWithpaypal?";
        //            var guid = Convert.ToString((new Random()).Next(100000));
        //            var createdPayment = CreatePayment(apiContext, baseURI + "guid=" + guid);

        //            //Get link returned frp, paypal
        //            var links = createdPayment.links.GetEnumerator();
        //            string paypalRedirectUrl = string.Empty;

        //            while (links.MoveNext())
        //            {
        //                Links link = links.Current;
        //                if (link.rel.ToLower().Trim().Equals("approval_url"))
        //                {
        //                    paypalRedirectUrl = link.href;
        //                }
        //            }
        //            Session.Add(guid, createdPayment.id);
        //            return Redirect(paypalRedirectUrl);
        //        }
        //        else
        //        {
        //            // This one will be executed when  we have received all the payment from params
        //            var guid = Request.Params["guid"];
        //            var executePayment = ExecutePayment(apiContext, payerId, Session[guid] as string);
        //            if (executePayment.state.ToLower() != "approved")
        //            {
        //                Session.Remove(strCart);
        //                return View("Failure");

        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
               

        //    }
        //    //Session.Remove(strCart);
        //    return View("OrderSuccess");
           


        //}
       
    }
}
//}

































































































































































































































































































































































