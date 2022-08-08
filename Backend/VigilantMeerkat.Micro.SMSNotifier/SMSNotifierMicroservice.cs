using System;
using System.Collections.Generic;
using System.Text;
using VigilantMeerkat.Micro.Base;
using VigilantMeerkat.Micro.Base.Attribute;

namespace VigilantMeerkat.Micro.SMSNotifier
{
    public class SMSNotifierMicroservice : MicroserviceBase
    {
        [MicroRoute("send", typeof(NotificationData))]
        public async void Send(NotificationData notification, MessageContext context)
        {
            Console.WriteLine("SEND SMS");
        }
    }
}
