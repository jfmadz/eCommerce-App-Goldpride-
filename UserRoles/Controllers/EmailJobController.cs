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
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using UserRoles.Models;

namespace UserRoles.Controllers
{
    public class EmailJobController  : Controller, IJob
    {
        // GET: EmailJob
        //public ActionResult Index()
        //{
        //    return View();
        //}
      

        public async Task Execute(IJobExecutionContext context)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            

            //Self Collection
            var selfC = (from x in db.Orders
                         join i in db.Maps
                         on x.OrderID equals i.Id
                         where i.Distance == 0
                         && x.PickUp == false
                         && x.Seen == true
                         && x.DriverID == null
                         && x.CollDate == DateTime.Today
                         select x);
            foreach (var item in selfC)
            {
                //string Email = item.Email;
                //string phone = item.CustomerPhone;
                //SmtpClient client = new SmtpClient("smtp.sendgrid.net");
                //client.Port = 25;
                //client.Host = "smtp.sendgrid.net";
                //client.Timeout = 10000;
                //client.DeliveryMethod = SmtpDeliveryMethod.Network;
                //client.EnableSsl = true;
                //client.UseDefaultCredentials = false;

                //var key = Environment.GetEnvironmentVariable("apikey");

                //client.Credentials = new NetworkCredential("apikey", key/*, user, password*/);


                //MailMessage msz = new MailMessage(Email, Email)
                //{
                //    From = new MailAddress(ConfigurationManager.AppSettings["Email"].ToString()),
                //    Subject = "Reminder for self Collection" + item.CustomerName.ToUpper(),

                //    IsBodyHtml = true,
                //    Body = " Good Day : " + item.CustomerName.ToUpper() + ", Please find attached order information for order ID : " + item.OrderID,


                //};
                //client.Send(msz);

                var apiKey = Environment.GetEnvironmentVariable("SENDGRID_API_KEY");
                var client = new SendGridClient(apiKey);
                SmtpClient smtp = new SmtpClient("smtp.sendgrid.net", Convert.ToInt32(587));
                smtp.Host = "smtp.sendgrid.net.com";
                smtp.Port = 587;
                MailMessage ms = new MailMessage();
               
                var from = new EmailAddress(ConfigurationManager.AppSettings["Email"].ToString());
                var to = new EmailAddress(User.Identity.Name);
                var subject = "Reminder to Collect";
                var plainTextContent = "";
                var header = "<b>" + "Hi" + ".\n" + item.CustomerName + " " + "We have recieved your payment for the following items :" + "</b>"
                    + "<br/>" + " <table>" +
                    "<tr>" +
                    "<td>" +
                    " <b> Order Number:</b>" +
                    "<td>" +
                    "<td><b>" + item.OrderID + "</b></td>" +
                    "<tr>";
                var htmlContent = "";
                var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, header + htmlContent);


                var response = client.SendEmailAsync(msg);

            }
            //End

            //Self Return

            var selfR = (from i in db.Maps
                         join
                         x in db.Orders on i.Id equals x.OrderID
                         where i.Distance == 0
                         && x.PickUp == true
                         && x.Seen == true
                         && x.DriverID == null
                         && x.ExpectedReturnDate == DateTime.Today
                         select x);

            foreach (var item in selfR)
            {
                string Email = item.Email;
                string phone = item.CustomerPhone;
                SmtpClient client = new SmtpClient("smtp.sendgrid.net");
                client.Port = 25;
                client.Host = "smtp.sendgrid.net";
                client.Timeout = 10000;
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.EnableSsl = true;
                client.UseDefaultCredentials = false;

                var key = Environment.GetEnvironmentVariable("apikey");

                client.Credentials = new NetworkCredential("apikey", key/*, user, password*/);


                MailMessage msz = new MailMessage(Email, Email)
                {
                    From = new MailAddress(ConfigurationManager.AppSettings["Email"].ToString()),
                    Subject = "Reminder to return items" + item.CustomerName.ToUpper(),

                    IsBodyHtml = true,
                    Body = " Good Day : " + item.CustomerName.ToUpper() + ", Please find attached order information for order ID : " + item.OrderID,


                };
                client.Send(msz);

            }

            //end


            //Deliver

            var info = (from i in db.Maps
                        join
                         x in db.Orders on i.Id equals x.OrderID
                        where/* i.Distance > 0 && */x.Collected == false && x.Delivered == false
                        && x.CollDate == DateTime.Today.AddDays(-1) && x.DriverID != null
                        select x);

            foreach (var item in info)
            {
                string Email = item.Email;
                string phone = item.CustomerPhone;
                SmtpClient client = new SmtpClient("smtp.sendgrid.net");
                client.Port = 25;
                client.Host = "smtp.sendgrid.net";
                client.Timeout = 10000;
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.EnableSsl = true;
                client.UseDefaultCredentials = false;

                var key = Environment.GetEnvironmentVariable("apikey");

                client.Credentials = new NetworkCredential("apikey", key/*, user, password*/);


                MailMessage msz = new MailMessage(Email, Email)
                {
                    From = new MailAddress(ConfigurationManager.AppSettings["Email"].ToString()),
                    Subject = "Items delivery is scheduled for today" + item.CustomerName.ToUpper(),

                    IsBodyHtml = true,
                    Body = " Good Day : " + item.CustomerName.ToUpper() + ", Please find attached order information for order ID : " + item.OrderID,


                };


                client.Send(msz);
            }


            //End

            //Return
            var Return = (from i in db.Maps
                        join
                        x in db.Orders on i.Id equals x.OrderID
                        where/* i.Distance > 0 && */x.Delivered == true && x.Collected == false
                        && x.ExpectedReturnDate == DateTime.Today && x.DriverID != null
                        select x);

            foreach (var item in Return)
            {
                string Email = item.Email;
                string phone = item.CustomerPhone;
                SmtpClient client = new SmtpClient("smtp.sendgrid.net");
                client.Port = 25;
                client.Host = "smtp.sendgrid.net";
                client.Timeout = 10000;
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.EnableSsl = true;
                client.UseDefaultCredentials = false;

                var key = Environment.GetEnvironmentVariable("apikey");

                client.Credentials = new NetworkCredential("apikey", key/*, user, password*/);


                MailMessage msz = new MailMessage(Email, Email)
                {
                    From = new MailAddress(ConfigurationManager.AppSettings["Email"].ToString()),
                    Subject = "Items Retrieval is scheduled for today " + item.CustomerName.ToUpper(),

                    IsBodyHtml = true,
                    Body = " Good Day : " + item.CustomerName.ToUpper() + ", Please find attached order information for order ID : " + item.OrderID,


                };


                client.Send(msz);



            }
            //end
        }
        
    }
}
