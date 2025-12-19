using Microsoft.AspNetCore.Identity.UI.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace KAShop.Bll.Service
{
    public class EmailSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var client = new SmtpClient("smtp.gmail.com", 587)
            {
                EnableSsl = true,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential("lojainmustafa2000@gmail.com", "ubdo zcrs vzud zuvt")
            };
            return client.SendMailAsync(

                new MailMessage(from: "lojainmustafa2000@gmail.com",
                                to: email,
                                 subject,
                                htmlMessage
                                )

                { IsBodyHtml=true}

                );
        }
    }
}
