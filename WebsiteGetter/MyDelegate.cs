using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebsiteGetter
{
    public class MyDelegate
    {
        public delegate void sendStringDelegate(string str);
        public delegate void sendVoidDelegate();
        public delegate void sendIntDelegate(int n);
        public delegate bool getBoolDelegate();
    }
}
