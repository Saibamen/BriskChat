using System.Collections.Generic;
using System.Linq;

namespace TrollChat.Web.Helpers
{
    public static class StringSeparatorHelper
    {
        public static List<string> TransformCommaSeparatedStringToList(string input)
        {
            return input.Split(',').ToList();
        }

        public static string RemoveUserFromString(string userName, string userNames)
        {
            var list = TransformCommaSeparatedStringToList(userNames);
            var tempString = "";
            foreach (var item in list)
            {
                if (item != userName)
                {
                    tempString += item + ", ";
                }
            }
            tempString = tempString.TrimEnd(',', ' ');
            return tempString;
        }
    }
}