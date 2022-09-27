using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Toshokan.Libraries.Shared.Utils
{
    public class BadgeUtils
    {
        public static string[] SplitForBadges(string data)
        {
            return data.Split(",");
        }
    }
}
