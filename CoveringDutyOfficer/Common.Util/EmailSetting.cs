using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure
{
    public class EmailSetting
    {
        public string Recipients { get; set; }
        public class Smtp
        {
            public string Email { get; set; }
            public string DisplayName { get; set; }
            public string Port { get; set; }
            public string Password { get; set; }
            public string Server { get; set; }
        }
    }
}
