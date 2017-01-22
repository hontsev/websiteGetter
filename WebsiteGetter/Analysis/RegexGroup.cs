using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebsiteGetter.Analysis
{
    public class RegexGroup
    {
        public List<RegexItem> regex;
        public string target;

        public RegexGroup()
        {
            this.regex = new List<RegexItem>();
        }
    }
}
