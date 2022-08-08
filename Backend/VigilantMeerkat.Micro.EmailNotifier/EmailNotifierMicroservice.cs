using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Text;
using VigilantMeerkat.Micro.Base;
using VigilantMeerkat.Micro.Base.Attribute;
using VigilantMeerkat.Micro.EmailNotifier.Models;

namespace VigilantMeerkat.Micro.EmailNotifier
{
    public class EmailNotifierMicroservice : MicroserviceBase
    {
        [MicroRoute("send", typeof(NotificationData))]
        public async void Send(NotificationData notification, MessageContext context)
        {
            var smtp = Configuration.GetSection("Smtp").Get<SmtpConfig>();

            var smtpClient = new SmtpClient(smtp.Server)
            {
                Port = smtp.Port,
                Credentials = new NetworkCredential(smtp.Login, smtp.Password),
                EnableSsl = true
            };

            smtpClient.Send(smtp.Login, notification.To, "VigilantMeerkat: Notification", notification.Content);
        }
    }
}
