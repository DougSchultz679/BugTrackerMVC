using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Web;

namespace BugTracker.Models.Helpers
{
    public static class Extensions
    {
        private static ApplicationDbContext db = new ApplicationDbContext();

        public static string GetFullName(this IIdentity user)
        {
            var ClaimsUser = (ClaimsIdentity)user;
            var claim = ClaimsUser.Claims.FirstOrDefault(c => c.Type == "Name");
            if (claim != null)
            {
                return claim.Value;
            } else return null;
        }

        public static bool In(this string source, params string[] list)
        {
            if (source == null) throw new ArgumentNullException("source");
            return list.Contains(source, StringComparer.OrdinalIgnoreCase);
        }

        public static bool DoesStringArrayHaveValue(this string[] list)
        {
            foreach (var str in list)
            {
                if (string.IsNullOrEmpty(str) == false) return true;
            }
            return false;
        }

        public static string EscapeJSReservedChars(this string str)
        {
            char[] splitArr = { '\'','\"','\n','\t',':',';','=',',' };
            string[] joinArr = { "\\'","\\\"","\\n","\\t","\\:","\\;","\\=","\\," };

            for (var i = 0; i < splitArr.Count(); i++)
            {
                str = String.Join(joinArr[i], str.Split(splitArr[i]));
            }

            return str;
        }
    }
}