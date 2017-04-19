using System;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.SignalR;

namespace TrollChat.Web.Helpers
{
    public static class HubExtensionMethod
    {
        public static int UserId(this Hub hub)
        {
            var userIdentity = (ClaimsIdentity)hub.Context.User.Identity;
            return Convert.ToInt32(userIdentity.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Sid).Value);
        }

        public static string Name(this Hub hub)
        {
            var userIdentity = (ClaimsIdentity)hub.Context.User.Identity;
            return userIdentity.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Name).Value;
        }
    }
}