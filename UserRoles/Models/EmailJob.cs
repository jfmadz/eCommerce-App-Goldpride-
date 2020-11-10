using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net.Mail;
using Quartz;
using System.Net;
using System.Threading.Tasks;
using System.Configuration;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity;

namespace UserRoles.Models
{
    public class EmailJob : IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {
            ApplicationDbContext db = new ApplicationDbContext();

            var info = (from i in db.Orders
                       where i.CollDate == DateTime.Now.AddDays(-1)
                       select i).Distinct().ToList().ToString();
           

            using (var message = new MailMessage())
                    {
                        message.Subject = "Test";
                        message.Body = "Test at " + DateTime.Now;
                        using (SmtpClient client = new SmtpClient
                        {
                            EnableSsl = true,
                            Host = "smtp.gmail.com",
                            Port = 587,
                            Credentials = new NetworkCredential("noreply.codetek@gmail.com", "ctek2020")

                        })
                        {
                            client.Send(message);
                        }
                    }
                    //SmtpClient client = new SmtpClient("smtp.sendgrid.net");
                    //client.Port = 25;
                    //client.Host = "smtp.sendgrid.net";
                    //client.Timeout = 10000;
                    //client.DeliveryMethod = SmtpDeliveryMethod.Network;
                    //client.EnableSsl = true;
                    //client.UseDefaultCredentials = false;

                    //var key = Environment.GetEnvironmentVariable("apikey");

                    //client.Credentials = new NetworkCredential("apikey", key/*, user, password*/);

                    ////System.Net.Mail.Attachment attachment;
                    ////System.Net.Mail.Attachment attach;
                    ////attachment = new System.Net.Mail.Attachment(memoryStream, "order.pdf");
                    ////attach = new System.Net.Mail.Attachment(ms, "order.png");
                    //MailMessage msz = new MailMessage(item.Email, item.Email)
                    //{
                    //    From = new MailAddress(ConfigurationManager.AppSettings["Email"].ToString()),
                    //    Subject = "order Details for " + item.CustomerName.ToUpper(),

                    //    IsBodyHtml = true,
                    //    Body = " Good Day : " + item.CustomerName.ToUpper() + ", Please find attached order information for order ID : " + item.OrderID,
                    //};
                   

                    //client.Send(msz);
               
           
            //using (var message = new MailMessage("karayusuf742@gmail.com", "karayusuf742@gmail.com"))
            //{
            //    message.Subject = "Test";
            //    message.Body = "Test at " + DateTime.Now;
            //    using (SmtpClient client = new SmtpClient
            //    {
            //        EnableSsl = true,
            //        Host = "smtp.gmail.com",
            //        Port = 587,
            //        Credentials = new NetworkCredential("noreply.codetek@gmail.com", "ctek2020")
                
            //    })
            //    {
            //         client.Send(message);
            //    }
            //}
        }
        //public interface Ijob
        //{
        //    Task IExecute(IJobExecutionContext context);
        //}
        //Task IJob.Execute(IJobExecutionContext context)
        //{
        //    throw new NotImplementedException();
        //}
    }
}