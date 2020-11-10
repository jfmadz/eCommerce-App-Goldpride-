using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using QRCoder;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using UserRoles.Models;

namespace UserRoles.Controllers
{
    public class QrGenerateController : Controller
    {
        // GET: QrGenerate
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]

        public ActionResult ActionQrCode()

        {

            return View();

        }



        [HttpPost]

        public ActionResult ActionQrCode(QRModel qr)

        {
            

            QRCodeGenerator ObjQr = new QRCodeGenerator();
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
                    MailMessage msz = new MailMessage("josh.madurai@gmail.com", "josh.madurai@gmail.com")
                    {
                        From = new MailAddress(ConfigurationManager.AppSettings["Email"].ToString()),
                        Subject = "Order QR From GOLDPRIDE For " + "Joshua" /*Name.ToUpper()*/,

                        IsBodyHtml = true,
                        Body = " Good Day : " + "Joshua"/*Name.ToUpper()*/ +" "+"MAdurai"/*SName.ToUpper()*/ + 
                        
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


            return View();
            //return RedirectToAction("ActionQrCode", "QrGenerate");

        }
    }
}