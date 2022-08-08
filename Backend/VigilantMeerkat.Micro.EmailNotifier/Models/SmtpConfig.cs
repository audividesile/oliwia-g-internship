using System;
using System.Collections.Generic;
using System.Text;

namespace VigilantMeerkat.Micro.EmailNotifier.Models
{
    public class SmtpConfig
    {
        public string Server { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public int Port { get; set; }
    }
}
