using System;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.SignalR.Hubs;

namespace TrollChat.Web.Helpers
{
    public static class HubExtensionMethods
    {
        public static int UserId(this HubCallerContext hubContext)
        {
            var userIdentity = (ClaimsIdentity)hubContext.User.Identity;

            return Convert.ToInt32(userIdentity.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Sid).Value);
        }

        public static string UserName(this HubCallerContext hubContext)
        {
            var userIdentity = (ClaimsIdentity)hubContext.User.Identity;

            return userIdentity.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Name).Value;
        }
    }
}