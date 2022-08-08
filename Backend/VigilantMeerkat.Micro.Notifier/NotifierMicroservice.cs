using Google.Protobuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VigilantMeerkat.Db;
using VigilantMeerkat.Micro.Base;
using VigilantMeerkat.Micro.Base.Attribute;

namespace VigilantMeerkat.Micro.Notifier
{
    public class NotifierMicroservice : MicroserviceBase
    {
        [MicroRoute("notify", typeof(Notification))]
        public async void Notify(Notification notification, MessageContext context)
        {
            var tokenInfo = DbStore.Get<AppDbContext>().Tokens.FirstOrDefault(x=> x.Token == Guid.Parse(notification.Token));
            var clientConfig = DbStore.Get<AppDbContext>().ClientConfigs.FirstOrDefault(x => x.ClientId == tokenInfo.Id);
            var admin = DbStore.Get<AppDbContext>().AdminConfigs.FirstOrDefault(x => x.Id == tokenInfo.AdminId);

            if (admin.DefaultNotificationType == "EMAIL" && CanSend(notification.Type, clientConfig.TriggerLevel))
            {
                var notificationData = new NotificationData
                {
                    To = admin.Email,
                    Content = notification.Message
                };

                Amqp.Call("emailnotifier", "send", context, notificationData.ToByteArray(), false);
            }
            else if (CanSend(notification.Type, clientConfig.TriggerLevel))
            {
                var notificationData = new NotificationData
                {
                    To = admin.Email,
                    Content = notification.Message
                };

                Amqp.Call("smsnotifier", "send", context, notificationData.ToByteArray(), false);
            }
        }

        private bool CanSend(string currentNotificationType, string settingNotificationType)
        {
            var levels = new List<string> { "LOG", "WARN", "ERROR" };

            return levels.IndexOf(currentNotificationType) >= levels.IndexOf(settingNotificationType);
        }
    }
}
