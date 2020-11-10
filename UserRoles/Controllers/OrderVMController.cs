
using SendGrid;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UserRoles.Models;
using PagedList;
using System.Web.UI;
using iTextSharp.text.pdf;
using iTextSharp.text;
using System.Net.Mail;
using System.Net;
using System.Configuration;

namespace UserRoles.Controllers
{
    public class OrderVMController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private string strCart = "Cart";
        // GET: OrderVM
        [AcceptVerbs(HttpVerbs.Get)]

        public ActionResult Index(int? page)
        {
            var list = db.Orders.Take(100);
            List<Cart> lstCart = (List<Cart>)Session[strCart];
            OrderDetail orderDetail = new OrderDetail();
            Order order = new Order();
            List<string> productName = new List<string>();
            List<int> productId = new List<int>();
            List<int> productQuantity = new List<int>();
            List<decimal> productPrice = new List<decimal>();

           
            try
            {
                foreach (Cart cart in lstCart)
                {
                    orderDetail.OrderID = order.OrderID;
                    orderDetail.ProductName = cart.ItemsHire.ProductName;
                    orderDetail.ProductID = cart.ItemsHire.ProductID;
                    orderDetail.Quantity = cart.Quantity;
                    orderDetail.Price = cart.ItemsHire.Price;
                    orderDetail.Total = cart.Quantity * cart.ItemsHire.Price;


                    var doc = new Document();
                    MemoryStream memoryStream = new MemoryStream();
                    PdfWriter writer = PdfWriter.GetInstance(doc, memoryStream);

                    doc.Open();

                    Paragraph heading = new Paragraph("<<<<<<<<GOOD DAY " + order.CustomerName.ToUpper() + ">>>>>>>>", new Font());

                    heading.Alignment = 1;
                    doc.Add(heading);
                    PdfPCell cell = new PdfPCell(new Phrase("Student Details"));
                    cell.Colspan = 3;
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    PdfPTable table = new PdfPTable(1);

                    table.AddCell(cell);
                    table.AddCell("Name :" + order.CustomerName);
                    table.AddCell("Surnname :" + order.Surname);
                    table.AddCell("Phone Number :" + order.CustomerPhone);
                    table.AddCell("Order ID :" + orderDetail.OrderID);
                    table.AddCell("Product Name :" + orderDetail.ProductName);
                    table.AddCell("Quantity :" + orderDetail.Quantity);
                    table.AddCell("Price :" + orderDetail.Price);
                    table.AddCell("Total :" + orderDetail.Total);
                    table.AddCell("Customer Email :" + order.Email);
                    table.AddCell("Customer Address :" + order.CollDate);
                    doc.Add(table);

                    PdfContentByte content = writer.DirectContent;
                    Rectangle rectangle = new Rectangle(doc.PageSize);
                    rectangle.Left += doc.LeftMargin;
                    rectangle.Right -= doc.RightMargin;
                    rectangle.Top -= doc.TopMargin;
                    rectangle.Bottom += doc.BottomMargin;
                    content.SetColorStroke(GrayColor.BLACK);
                    content.Rectangle(rectangle.Left, rectangle.Bottom, rectangle.Width, rectangle.Height);
                    content.Stroke();


                    writer.CloseStream = false;
                    doc.Close();




                    memoryStream.Position = 0;

                    MailMessage mm = new MailMessage(order.Email, order.Email)
                    {
                        Subject = "Order Details for " + order.CustomerName.ToUpper(),
                        IsBodyHtml = true,
                        Body = " Good Day : " + order.CustomerName.ToUpper() + ", Please find attached purchase information for Order ID : " + orderDetail.OrderID
                    };


                    mm.Attachments.Add(new Attachment(memoryStream, "order.pdf"));
                    SmtpClient smtp = new SmtpClient
                    {
                        Host = "smtp.gmail.com",
                        Port = 587,
                        EnableSsl = true,
                        Credentials = new NetworkCredential("noreply.codetek@gmail.com", "Ctek2020")

                    };
                    smtp.Send(mm);
                }
                // return RedirectToAction("Index");
            }
            catch (Exception e)
            {

                Response.Write(e.Message);
            }
            return View(list.ToPagedList(page ?? 1, 15));
        }









        //Using foreach loop fill data from custmerlist to List<CustomerVM>.

        /* return View(orderVm.ToPagedList(page ?? 1,15));*/ //List of CustomerVM (ViewModel)


        // return View();
        //[HttpPost]
        public ActionResult CusIndex(string searchString, int? page)
        {
            var list = db.OrderVMs.Take(100);
            //search code test
            if ((!string.IsNullOrEmpty(searchString)))
            {
                list = list.Where(s => s.Email.Contains(searchString));
            }


            // var list = db.Orders.Take(100);
            List<Cart> lstCart = (List<Cart>)Session[strCart];
            OrderDetail orderDetail = new OrderDetail();
            Order order = new Order();
            List<string> productName = new List<string>();
            List<int> productId = new List<int>();
            List<int> productQuantity = new List<int>();
            List<decimal> productPrice = new List<decimal>();

            foreach (Cart cart in lstCart)
            {

                orderDetail.OrderID = order.OrderID;
                orderDetail.ProductName = cart.ItemsHire.ProductName;
                orderDetail.ProductID = cart.ItemsHire.ProductID;
                orderDetail.Quantity = cart.Quantity;
                orderDetail.Price = cart.ItemsHire.Price;
                orderDetail.Total = cart.Quantity * cart.ItemsHire.Price;

                productName.Add(cart.ItemsHire.ProductName);
                productQuantity.Add(cart.Quantity);
                productPrice.Add(cart.ItemsHire.Price);
            }
           
            try
            {
                foreach (Cart cart in lstCart)
                {
                    orderDetail.OrderID = order.OrderID;
                    orderDetail.ProductName = cart.ItemsHire.ProductName;
                    orderDetail.ProductID = cart.ItemsHire.ProductID;
                    orderDetail.Quantity = cart.Quantity;
                    orderDetail.Price = cart.ItemsHire.Price;
                    orderDetail.Total = cart.Quantity * cart.ItemsHire.Price;


                    var doc = new Document();
                    MemoryStream memoryStream = new MemoryStream();
                    PdfWriter writer = PdfWriter.GetInstance(doc, memoryStream);

                    doc.Open();

                    Paragraph heading = new Paragraph("<<<<<<<<GOOD DAY " + order.CustomerName.ToUpper() + ">>>>>>>>", new Font());

                    heading.Alignment = 1;
                    doc.Add(heading);
                    PdfPCell cell = new PdfPCell(new Phrase("Student Details"));
                    cell.Colspan = 3;
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    PdfPTable table = new PdfPTable(1);

                    table.AddCell(cell);
                    table.AddCell("Name :" + order.CustomerName);
                    table.AddCell("Surnname :" + order.Surname);
                    table.AddCell("Phone Number :" + order.CustomerPhone);
                    table.AddCell("Order ID :" + orderDetail.OrderID);
                    table.AddCell("Phone Number :" + orderDetail.ProductName);
                    table.AddCell("Phone Number :" + orderDetail.Quantity);
                    table.AddCell("Phone Number :" + orderDetail.Price);
                    table.AddCell("Phone Number :" + orderDetail.Total);
                    table.AddCell("Phone Number :" + order.Email);
                    table.AddCell("Phone Number :" + order.CollDate);
                    doc.Add(table);

                    PdfContentByte content = writer.DirectContent;
                    Rectangle rectangle = new Rectangle(doc.PageSize);
                    rectangle.Left += doc.LeftMargin;
                    rectangle.Right -= doc.RightMargin;
                    rectangle.Top -= doc.TopMargin;
                    rectangle.Bottom += doc.BottomMargin;
                    content.SetColorStroke(GrayColor.BLACK);
                    content.Rectangle(rectangle.Left, rectangle.Bottom, rectangle.Width, rectangle.Height);
                    content.Stroke();


                    writer.CloseStream = false;
                    doc.Close();




                    memoryStream.Position = 0;

                    MailMessage mm = new MailMessage(order.Email, order.Email)
                    {
                        Subject = "Order Details for " + order.CustomerName.ToUpper(),
                        IsBodyHtml = true,
                        Body = " Good Day : " + order.CustomerName.ToUpper() + ", Please find attached purchase information for Order ID : " + orderDetail.OrderID
                    };


                    mm.Attachments.Add(new Attachment(memoryStream, "order.pdf"));
                    SmtpClient smtp = new SmtpClient
                    {
                        Host = "smtp.gmail.com",
                        Port = 587,
                        EnableSsl = true,
                        Credentials = new NetworkCredential("noreply.codetek@gmail.com", "Ctek2020")

                    };
                    smtp.Send(mm);
                }
                // return RedirectToAction("Index");
            }
            catch (Exception e)
            {

                Response.Write(e.Message);
            }
            return View(list.ToPagedList(page ?? 1, 15));





          
        }



       
    }



}