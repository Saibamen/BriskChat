using System;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.SignalR.Hubs;

namespace TrollChat.Web.Helpers
{
    public static class HubExtensionMethods
    {
        public static Guid UserId(this HubCallerContext hubContext)
        {
            var userIdentity = (ClaimsIdentity)hubContext.User.Identity;

            return new Guid(userIdentity.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Sid).Value);
        }

        public static string UserName(this HubCallerContext hubContext)
        {
            var userIdentity = (ClaimsIdentity)hubContext.User.Identity;

            return userIdentity.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Name).Value;
        }

        public static string DomainName(this HubCallerContext hubContext)
        {
            var userIdentity = (ClaimsIdentity)hubContext.User.Identity;

            return userIdentity.Claims.FirstOrDefault(x => x.Type == "DomainName").Value;
        }

        public static Guid DomainId(this HubCallerContext hubContext)
        {
            var userIdentity = (ClaimsIdentity)hubContext.User.Identity;

            return new Guid(userIdentity.Claims.FirstOrDefault(x => x.Type == "DomainId").Value);
        }
    }
}