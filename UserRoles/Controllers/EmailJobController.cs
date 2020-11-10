using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Nexmo.Api;
using Quartz;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;
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

            var info = (from i in db.Orders
                        where i.CollDate == DateTime.Today 
                        select i);
           
            foreach (var item in info)
            {
                string Email = item.Email;
                string phone = item.CustomerPhone;
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
                //    Subject = "order Details for " + item.CustomerName.ToUpper(),

                //    IsBodyHtml = true,
                //    Body = " Good Day : " + item.CustomerName.ToUpper() + ", Please find attached order information for order ID : " + item.OrderID,


                //};


                //client.Send(msz);
                try
                {
                    var client = new Client(creds: new Nexmo.Api.Request.Credentials
                    {
                        ApiKey = "911d5752",
                        ApiSecret = "Kwg7mCuuhT2NQ7N2"
                    });
                    var results = client.SMS.Send(request: new SMS.SMSRequest
                    {
                        from = "Vonage APIs",
                        //to = "27748736622",
                        to = phone,
                        text = "Hello from Vonage SMS API"
                    });
                }
                catch(Exception ex)
                {

                }
               
                
            }
          


        }
        
    }
}
